﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using SocketServers;
using Turn.Message;

namespace Turn.Server
{
	public partial class TurnServer
	{
		//private object syncRoot;
		private ServersManager<TurnConnection> turnServer;
		private ServersManager<PeerConnection> peerServer;

		private AllocationsPool allocations;

		private ILogger logger;

		public TurnServer(ILogger logger)
		{
			//this.syncRoot = new object();
			this.logger = logger ?? new NullLogger();
		}

		public int TurnUdpPort { get; set; }
		public int TurnTcpPort { get; set; }
		public int TurnPseudoTlsPort { get; set; }
		public Authentificater Authentificater { get; set; }
		public IPAddress PublicIp { get; set; }
		public IPAddress RealIp { get; set; }
		public int MinPort { get; set; }
		public int MaxPort { get; set; }

		private void TurnServer_TurnDataReceived(ref ServerAsyncEventArgs e)
		{
			//lock (syncRoot)
			{
				TurnMessage response = null;

				try
				{
					if (true)//(TransactionServer.GetCachedResponse(e, out response) == false)
					{
						TurnMessage request = TurnMessage.Parse(e.Buffer, e.Offset, e.BytesTransferred, TurnMessageRfc.MsTurn);

						if (Authentificater.Process(request, out response))
						{
							Allocation allocation = null;
							if (request.MsSequenceNumber != null)
								allocation = allocations.Get(request.MsSequenceNumber.ConnectionId);

							if (allocation != null)
							{
								if (request.MsSequenceNumber != null && allocation.SequenceNumber == request.MsSequenceNumber.SequenceNumber)
									response = allocation.Response;
							}

							if (response == null)
							{
								if (allocation != null)
									allocation.TouchLifetime();

								switch (request.MessageType)
								{
									case MessageType.AllocateRequest:
										response = ProcessAllocateRequest(ref allocation, request, e.LocalEndPoint, e.RemoteEndPoint);
										break;

									case MessageType.SendRequest:
										response = ProcessSendRequest(allocation, request, ref e);
										break;

									case MessageType.SetActiveDestinationRequest:
										response = ProcessSetActiveDestinationRequest(allocation, request, e.RemoteEndPoint);
										break;
								}

								if (allocation != null && response != null)
								{
									allocation.Response = response;
									allocation.SequenceNumber = response.MsSequenceNumber.SequenceNumber;
								}
							}
						}

						//TransactionServer.CacheResponse(e, response);
					}
				}
				catch (TurnMessageException ex)
				{
					response = GetErrorResponse(ex.ErrorCode, e);
				}
				catch (TurnServerException ex)
				{
					response = GetErrorResponse(ex.ErrorCode, e);
				}
				catch (Exception ex)
				{
					response = GetErrorResponse(ErrorCode.ServerError, e);

					logger.WriteError(ex.ToString());
				}

				if (response != null)
					SendTurn(response, e.LocalEndPoint, e.RemoteEndPoint);
			}
		}

		private void TurnServer_PeerDataReceived(ref ServerAsyncEventArgs e)
		{
			//lock (syncRoot)
			{
				try
				{
					Allocation allocation = allocations.GetByTurn(e.LocalEndPoint, e.RemoteEndPoint);

					if (allocation != null)
					{
						allocation.TouchLifetime();

						if (allocation.IsActiveDestinationEnabled)
						{
							e.LocalEndPoint = allocation.Alocated;
							e.RemoteEndPoint = allocation.ActiveDestination;
							e.Count = e.BytesTransferred;
							e.ConnectionId = ServerAsyncEventArgs.AnyNewConnectionId;

							peerServer.SendAsync(e);

							e = null;
						}
					}
				}
				catch (Exception ex)
				{
					logger.WriteWarning(ex.ToString());
				}
			}
		}

		private bool PeerServer_Received(ServersManager<PeerConnection> s, BaseConnection с, ref ServerAsyncEventArgs e)
		{
			//lock (syncRoot)
			{
				try
				{
					Allocation allocation = allocations.Get(e.LocalEndPoint);

					if (allocation != null)
					{
						if (allocation.Permissions.IsPermited(e.RemoteEndPoint))
						{
							allocation.TouchLifetime();

							if (allocation.ActiveDestination.IsEqual(e.RemoteEndPoint))
							{
								if (e.LocalEndPoint.Protocol == ServerProtocol.Tcp)
								{
									TcpFramingHeader.GetBytes(e.Buffer, e.Offset, TcpFrameType.EndToEndData, e.BytesTransferred);

									e.Count = e.OffsetOffset + e.BytesTransferred;
									e.OffsetOffset = 0;
								}
								else
								{
									e.Count = e.BytesTransferred;
								}

								e.LocalEndPoint = allocation.Local;
								e.RemoteEndPoint = allocation.Reflexive;
								e.ConnectionId = ServerAsyncEventArgs.AnyConnectionId;

								turnServer.SendAsync(e);

								e = null;
							}
							else
							{
								TurnMessage message = new TurnMessage()
								{
									IsAttributePaddingDisabled = true,
									MessageType = MessageType.DataIndication,
									TransactionId = TransactionServer.GenerateTransactionId(),

									MagicCookie = new MagicCookie(),

									RemoteAddress = new RemoteAddress()
									{
										IpAddress = e.RemoteEndPoint.Address,
										Port = (UInt16)e.RemoteEndPoint.Port,
									},

									Data = new Data()
									{
										ValueRef = e.Buffer,
										ValueRefOffset = e.Offset,
										ValueRefLength = e.BytesTransferred,
									},
								};

								SendTurn(message, allocation.Local, allocation.Reflexive);
							}
						}
					}
				}
				catch (Exception ex)
				{
					logger.WriteWarning(ex.ToString());
				}
			}

			return true;
		}

		private void Allocation_Removed(Allocation allocation)
		{
			peerServer.Unbind(allocation.Alocated.ProtocolPort);
		}

		private TurnMessage ProcessAllocateRequest(ref Allocation allocation, TurnMessage request, ServerEndPoint local, IPEndPoint remote)
		{
			{
				uint lifetime = (request.Lifetime != null) ? ((request.Lifetime.Value > MaxLifetime.Seconds) ? MaxLifetime.Seconds : request.Lifetime.Value) : DefaultLifetime.Seconds;
				uint sequenceNumber = (request.MsSequenceNumber != null) ? request.MsSequenceNumber.SequenceNumber : 0;

				if (allocation != null)
				{
					allocation.Lifetime = lifetime;

					if (lifetime == 0)
						logger.WriteInformation(string.Format("Update Allocation: {2} seconds {0} <--> {1}", allocation.Alocated.ToString(), allocation.Reflexive.ToString(), lifetime));
				}
				else
				{
					if (lifetime <= 0)
						throw new TurnServerException(ErrorCode.NoBinding);

					ProtocolPort pp = new ProtocolPort() { Protocol = local.Protocol, };
					if (peerServer.Bind(ref pp) != SocketError.Success)
						throw new TurnServerException(ErrorCode.ServerError);

					allocation = new Allocation()
					{
						TransactionId = request.TransactionId,
						ConnectionId = ConnectionIdGenerator.Generate(local, remote),

						Local = local,
						Alocated = new ServerEndPoint(pp, PublicIp),
						Real = new ServerEndPoint(pp, RealIp),
						Reflexive = new IPEndPoint(remote.Address, remote.Port),

						Lifetime = lifetime,
					};

					allocations.Replace(allocation);

					logger.WriteInformation(string.Format("Allocated: {0} <--> {1}", allocation.Alocated.ToString(), allocation.Reflexive.ToString()));
				}
			}

			return new TurnMessage()
			{
				IsAttributePaddingDisabled = true,
				MessageType = MessageType.AllocateResponse,
				TransactionId = request.TransactionId,

				MagicCookie = new MagicCookie(),

				MappedAddress = new MappedAddress()
				{
					IpAddress = allocation.Alocated.Address,
					Port = (UInt16)allocation.Alocated.Port,
				},

				Lifetime = new Lifetime() { Value = allocation.Lifetime, },
				Bandwidth = new Bandwidth() { Value = 750, },

				XorMappedAddress = new XorMappedAddress(TurnMessageRfc.MsTurn)
				{
					IpAddress = remote.Address,
					Port = (UInt16)remote.Port,
				},

				Realm = new Realm(TurnMessageRfc.MsTurn)
				{
					Ignore = true,
					Value = Authentificater.Realm,
				},
				MsUsername = new MsUsername()
				{
					Ignore = true,
					Value = request.MsUsername.Value,
				},
				//MsUsername = allocation.Username,
				MessageIntegrity = new MessageIntegrity(),

				MsSequenceNumber = new MsSequenceNumber()
				{
					ConnectionId = allocation.ConnectionId,
					SequenceNumber = allocation.SequenceNumber,
				},
			};
		}

		private TurnMessage ProcessSendRequest(Allocation allocation, TurnMessage request, ref ServerAsyncEventArgs e)
		{
			try
			{
				if (allocation == null)
					throw new TurnServerException(ErrorCode.NoBinding);

				if (request.Data == null || request.DestinationAddress == null)
					throw new TurnServerException(ErrorCode.BadRequest);

				allocation.Permissions.Permit(request.DestinationAddress.IpEndPoint);

				e.LocalEndPoint = allocation.Alocated;
				e.RemoteEndPoint = request.DestinationAddress.IpEndPoint;
				e.Offset = request.Data.ValueRefOffset;
				e.Count = request.Data.ValueRefLength;
				e.ConnectionId = ServerAsyncEventArgs.AnyNewConnectionId;

				peerServer.SendAsync(e);

				e = null;
			}
			catch (Exception ex)
			{
				logger.WriteWarning(ex.ToString());
			}

			// [MS-TURN] The server MUST NOT respond to a client with either 
			// a Send response or a Send error response.
			return null;
		}

		private TurnMessage ProcessSetActiveDestinationRequest(Allocation allocation, TurnMessage request, IPEndPoint reflexEndPoint)
		{
			if (allocation == null)
				throw new TurnServerException(ErrorCode.NoBinding);

			if (request.DestinationAddress == null)
				throw new TurnServerException(ErrorCode.BadRequest);

			allocation.ActiveDestination = request.DestinationAddress.IpEndPoint;
			allocation.Permissions.Permit(request.DestinationAddress.IpEndPoint);

			logger.WriteInformation(string.Format("Set Active Destination: {2} --> {0} <--> {1}", allocation.Alocated.ToString(), allocation.Reflexive.ToString(), allocation.ActiveDestination.ToString()));

			return new TurnMessage()
			{
				IsAttributePaddingDisabled = true,
				MessageType = MessageType.SetActiveDestinationResponse,
				TransactionId = request.TransactionId,

				MagicCookie = new MagicCookie(),

				Realm = new Realm(TurnMessageRfc.MsTurn)
				{
					Ignore = true,
					Value = Authentificater.Realm,
				},
				MsUsername = new MsUsername()
				{
					Ignore = true,
					Value = request.MsUsername.Value,
				},
				//MsUsername = allocation.Username,
				MessageIntegrity = new MessageIntegrity(),

				MsSequenceNumber = new MsSequenceNumber()
				{
					ConnectionId = allocation.ConnectionId,
					SequenceNumber = allocation.SequenceNumber,
				},
			};
		}

		private TurnMessage GetErrorResponse(ErrorCode errorCode, SocketAsyncEventArgs e)
		{
			MessageType? messageType = TurnMessage.SafeGetMessageType(e.Buffer, e.Offset, e.Count);
			TransactionId id = TurnMessage.SafeGetTransactionId(e.Buffer, e.Offset, e.Count);

			if (messageType != null && id != null)
			{
				return new TurnMessage()
				{
					MessageType = ((MessageType)messageType).GetErrorResponseType(),
					TransactionId = id,
					ErrorCodeAttribute = new ErrorCodeAttribute()
					{
						ErrorCode = (int)errorCode,
						ReasonPhrase = errorCode.GetReasonPhrase(),
					},
				};
			}

			return null;
		}

		private void GetBuffer(ServerEndPoint local, IPEndPoint remote, int length, out ServerAsyncEventArgs e, out int offset)
		{
			int headerLength = (local.Protocol == ServerProtocol.Tcp) ? TcpFramingHeader.TcpFramingHeaderLength : 0;

			e = EventArgsManager.Get();

			e.ConnectionId = ServerAsyncEventArgs.AnyConnectionId;
			e.LocalEndPoint = local;
			e.RemoteEndPoint = remote;
			e.Count = headerLength + length;
			e.AllocateBuffer();

			if (headerLength > 0)
				TcpFramingHeader.GetBytes(e.Buffer, e.Offset, TcpFrameType.ControlMessage, length);

			offset = e.Offset + headerLength;
		}

		private void PeerSendAsync_Completed(Socket socket, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
				logger.WriteWarning(String.Format(@"SendPeer Failed\r\nSocket Type {0}:\r\nError: {1}", socket.SocketType.ToString(), e.ToString()));
		}

		private void SendTurn(TurnMessage message, ServerEndPoint local, IPEndPoint remote)
		{
			ServerAsyncEventArgs p;
			int offset;

			message.ComputeMessageLength();

			GetBuffer(local, remote, message.TotalMessageLength, out p, out offset);

			message.GetBytes(p.Buffer, offset, Authentificater.Key2);

			turnServer.SendAsync(p);
		}

		private void SendTurnAsync_Completed(Socket socket, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
				logger.WriteWarning(String.Format(@"SendTurn Failed\r\nSocket Type {0}:\r\nError: {1}", socket.SocketType.ToString(), e.ToString()));
		}

		public void EnableLog(string fileName)
		{
			if (string.IsNullOrEmpty(fileName) == false)
				turnServer.Logger.Enable(fileName);
			else
				turnServer.Logger.Disable();
		}

		public void Start()
		{
			//lock (syncRoot)
			{
				if (PublicIp == null)
					throw new Exception("Invalid Public IP");

				allocations = new AllocationsPool();
				allocations.Removed += Allocation_Removed;

				ServerAsyncEventArgs.DefaultOffsetOffset = TcpFramingHeader.TcpFramingHeaderLength;

				turnServer = new ServersManager<TurnConnection>(new ServersManagerConfig());
				turnServer.Bind(new ProtocolPort() { Protocol = ServerProtocol.Udp, Port = TurnUdpPort, });
				turnServer.Bind(new ProtocolPort() { Protocol = ServerProtocol.Tcp, Port = TurnTcpPort, });
				turnServer.Bind(new ProtocolPort() { Protocol = ServerProtocol.Tcp, Port = TurnPseudoTlsPort, });
				turnServer.NewConnection += TurnServer_NewConnection;
				turnServer.Received += TurnServer_Received;
				turnServer.Start();

				peerServer = new ServersManager<PeerConnection>(
					new ServersManagerConfig()
					{
						MinPort = MinPort,
						MaxPort = MaxPort,
					});
				peerServer.AddressPredicate = (i, ip, ai) => { return ai.Address.Equals(RealIp); };
				peerServer.Received += PeerServer_Received;
				peerServer.Start();
			}
		}


		public void Stop()
		{
			//lock (syncRoot)
			{
				if (allocations != null)
				{
					allocations.Clear();
					allocations.Removed -= Allocation_Removed;
					allocations = null;
				}

				if (turnServer != null)
				{
					turnServer.Dispose();
					turnServer = null;
				}

				Authentificater = null;
			}
		}
	}
}
