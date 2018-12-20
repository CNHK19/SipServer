using Microsoft.Win32.Ssp;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace SocketServers
{
	internal class SspiTlsServer<C> : BaseTcpServer<C> where C : BaseConnection, IDisposable, new()
	{
		private X509Certificate certificate;

		private SafeCredHandle credential;

		private int maxTokenSize;

		public SspiTlsServer(ServersManagerConfig config) : base(config)
		{
			this.certificate = config.TlsCertificate;
		}

		public override void Start()
		{
			long num;
			Sspi.AcquireCredentialsHandle(CredentialUse.SECPKG_CRED_INBOUND, new SchannelCred(this.certificate, SchProtocols.TlsServer), out this.credential, out num);
			this.GetMaxTokenSize();
			base.Start();
		}

		public override void Dispose()
		{
			this.credential.Dispose();
			base.Dispose();
		}

		public override void SendAsync(ServerAsyncEventArgs e)
		{
			try
			{
				Server<C>.Connection<C> tcpConnection = base.GetTcpConnection(e.RemoteEndPoint);
				base.OnBeforeSend(tcpConnection, e);
				if (tcpConnection == null)
				{
					e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed);
					e.SocketError = SocketError.NotConnected;
					e.OnCompleted(null);
				}
				else
				{
					SspiContext sspiContext = tcpConnection.SspiContext;
					SecPkgContext_StreamSizes streamSizes = sspiContext.StreamSizes;
					int count = e.Count;
					if (e.OffsetOffset < streamSizes.cbHeader)
					{
						throw new NotImplementedException("Ineffective way not implemented. Need to move buffer for SECBUFFER_STREAM_HEADER.");
					}
					e.OffsetOffset -= streamSizes.cbHeader;
					e.Count = streamSizes.cbHeader + count + streamSizes.cbTrailer;
					e.ReAllocateBuffer(true);
					SecBufferDescEx secBufferDescEx = new SecBufferDescEx(new SecBufferEx[]
					{
						new SecBufferEx
						{
							BufferType = BufferType.SECBUFFER_STREAM_HEADER,
							Buffer = e.Buffer,
							Size = streamSizes.cbHeader,
							Offset = e.Offset
						},
						new SecBufferEx
						{
							BufferType = BufferType.SECBUFFER_DATA,
							Buffer = e.Buffer,
							Size = count,
							Offset = e.Offset + streamSizes.cbHeader
						},
						new SecBufferEx
						{
							BufferType = BufferType.SECBUFFER_STREAM_TRAILER,
							Buffer = e.Buffer,
							Size = streamSizes.cbTrailer,
							Offset = e.Offset + streamSizes.cbHeader + count
						},
						new SecBufferEx
						{
							BufferType = BufferType.SECBUFFER_VERSION
						}
					});
					Sspi.EncryptMessage(ref sspiContext.Handle, ref secBufferDescEx, 0u, null);
					e.Count = secBufferDescEx.Buffers[0].Size + secBufferDescEx.Buffers[1].Size + secBufferDescEx.Buffers[2].Size;
					e.ReAllocateBuffer(true);
					base.SendAsync(tcpConnection, e);
				}
			}
			catch (SspiException error)
			{
				e.SocketError = SocketError.Fault;
				this.OnFailed(new ServerInfoEventArgs(this.realEndPoint, error));
			}
		}

		protected override void OnNewTcpConnection(Server<C>.Connection<C> connection)
		{
			connection.SspiContext.Connected = false;
			connection.SspiContext.Buffer.Resize(this.maxTokenSize);
		}

		protected override void OnEndTcpConnection(Server<C>.Connection<C> connection)
		{
			if (connection.SspiContext.Connected)
			{
				connection.SspiContext.Connected = false;
				this.OnEndConnection(connection);
			}
		}

		protected override bool OnTcpReceived(Server<C>.Connection<C> connection, ref ServerAsyncEventArgs e)
		{
			bool connected = connection.SspiContext.Connected;
			bool flag;
			if (connection.SspiContext.Connected)
			{
				flag = this.DecryptData(ref e, connection);
			}
			else
			{
				flag = this.Handshake(e, connection);
			}
			while (flag && connected != connection.SspiContext.Connected && connection.SspiContext.Buffer.IsValid)
			{
				connected = connection.SspiContext.Connected;
				ServerAsyncEventArgs ie = null;
				if (connection.SspiContext.Connected)
				{
					flag = this.DecryptData(ref ie, connection);
				}
				else
				{
					flag = this.Handshake(ie, connection);
				}
			}
			return flag;
		}

		private bool DecryptData(ref ServerAsyncEventArgs e, Server<C>.Connection<C> connection)
		{
			SspiContext sspiContext = connection.SspiContext;
			SecBufferDescEx secBufferDesc = sspiContext.SecBufferDesc5;
			if (sspiContext.Buffer.IsValid && e != null && !sspiContext.Buffer.CopyTransferredFrom(e, 0))
			{
				return false;
			}
			SecurityStatus securityStatus2;
			while (true)
			{
				secBufferDesc.Buffers[0].BufferType = BufferType.SECBUFFER_DATA;
				if (sspiContext.Buffer.IsValid)
				{
					this.SetSecBuffer(ref secBufferDesc.Buffers[0], sspiContext);
				}
				else
				{
					this.SetSecBuffer(ref secBufferDesc.Buffers[0], e);
				}
				secBufferDesc.Buffers[1].SetBufferEmpty();
				secBufferDesc.Buffers[2].SetBufferEmpty();
				secBufferDesc.Buffers[3].SetBufferEmpty();
				secBufferDesc.Buffers[4].SetBufferEmpty();
				SecurityStatus securityStatus = Sspi.SafeDecryptMessage(ref sspiContext.Handle, ref secBufferDesc, 0u, null);
				int bufferIndex = secBufferDesc.GetBufferIndex(BufferType.SECBUFFER_EXTRA, 0);
				int bufferIndex2 = secBufferDesc.GetBufferIndex(BufferType.SECBUFFER_DATA, 0);
				securityStatus2 = securityStatus;
				if (securityStatus2 != SecurityStatus.SEC_E_OK)
				{
					break;
				}
				if (bufferIndex2 < 0)
				{
					return false;
				}
				if (sspiContext.Buffer.IsInvalid)
				{
					if (bufferIndex >= 0 && !sspiContext.Buffer.CopyFrom(secBufferDesc.Buffers[bufferIndex]))
					{
						return false;
					}
					e.Offset = secBufferDesc.Buffers[bufferIndex2].Offset;
					e.BytesTransferred = secBufferDesc.Buffers[bufferIndex2].Size;
					e.SetMaxCount();
					if (!this.OnReceived(connection, ref e))
					{
						return false;
					}
				}
				else
				{
					ArraySegment<byte> buffer = sspiContext.Buffer.Detach();
					if (bufferIndex >= 0 && !sspiContext.Buffer.CopyFrom(secBufferDesc.Buffers[bufferIndex]))
					{
						return false;
					}
					ServerAsyncEventArgs serverAsyncEventArgs = EventArgsManager.Get();
					base.PrepareEventArgs(connection, serverAsyncEventArgs);
					serverAsyncEventArgs.AttachBuffer(buffer);
					serverAsyncEventArgs.Offset = secBufferDesc.Buffers[bufferIndex2].Offset;
					serverAsyncEventArgs.BytesTransferred = secBufferDesc.Buffers[bufferIndex2].Size;
					serverAsyncEventArgs.SetMaxCount();
					bool flag = this.OnReceived(connection, ref serverAsyncEventArgs);
					if (serverAsyncEventArgs != null)
					{
						EventArgsManager.Put(serverAsyncEventArgs);
					}
					if (!flag)
					{
						return false;
					}
				}
				if (bufferIndex < 0)
				{
					return true;
				}
			}
			return securityStatus2 != SecurityStatus.SEC_I_RENEGOTIATE && securityStatus2 == (SecurityStatus)2148074264u && (!sspiContext.Buffer.IsInvalid || sspiContext.Buffer.CopyTransferredFrom(e, 0));
		}

		private bool Handshake(ServerAsyncEventArgs ie, Server<C>.Connection<C> connection)
		{
			int num = 0;
			ServerAsyncEventArgs serverAsyncEventArgs = null;
			SspiContext sspiContext = connection.SspiContext;
			SecBufferDescEx secBufferDescEx = sspiContext.SecBufferDesc2[0];
			SecBufferDescEx secBufferDescEx2 = sspiContext.SecBufferDesc2[1];
			bool result;
			try
			{
				if (sspiContext.Buffer.IsValid && ie != null && !sspiContext.Buffer.CopyTransferredFrom(ie, 0))
				{
					result = false;
				}
				else
				{
					while (true)
					{
						secBufferDescEx.Buffers[0].BufferType = BufferType.SECBUFFER_TOKEN;
						if (sspiContext.Buffer.IsValid)
						{
							this.SetSecBuffer(ref secBufferDescEx.Buffers[0], sspiContext);
						}
						else
						{
							this.SetSecBuffer(ref secBufferDescEx.Buffers[0], ie);
						}
						secBufferDescEx.Buffers[1].SetBufferEmpty();
						if (serverAsyncEventArgs == null)
						{
							serverAsyncEventArgs = EventArgsManager.Get();
						}
						serverAsyncEventArgs.AllocateBuffer();
						secBufferDescEx2.Buffers[0].BufferType = BufferType.SECBUFFER_TOKEN;
						secBufferDescEx2.Buffers[0].Size = serverAsyncEventArgs.Count;
						secBufferDescEx2.Buffers[0].Buffer = serverAsyncEventArgs.Buffer;
						secBufferDescEx2.Buffers[0].Offset = serverAsyncEventArgs.Offset;
						secBufferDescEx2.Buffers[1].SetBufferEmpty();
						int contextReq = 98332;
						SafeCtxtHandle handle = sspiContext.Handle.IsInvalid ? new SafeCtxtHandle() : sspiContext.Handle;
						long num2;
						SecurityStatus securityStatus = Sspi.SafeAcceptSecurityContext(ref this.credential, ref sspiContext.Handle, ref secBufferDescEx, contextReq, TargetDataRep.SECURITY_NATIVE_DREP, ref handle, ref secBufferDescEx2, out num, out num2);
						if (sspiContext.Handle.IsInvalid)
						{
							sspiContext.Handle = handle;
						}
						SecurityStatus securityStatus2 = securityStatus;
						if (securityStatus2 == (SecurityStatus)2148074264u)
						{
							break;
						}
						if (securityStatus2 != (SecurityStatus)2148074273u)
						{
							if ((securityStatus == SecurityStatus.SEC_I_CONTINUE_NEEDED || securityStatus == SecurityStatus.SEC_E_OK || (Sspi.Failed(securityStatus) && (num & 32768) != 0)) && secBufferDescEx2.Buffers[0].Size > 0)
							{
								serverAsyncEventArgs.Count = secBufferDescEx2.Buffers[0].Size;
								serverAsyncEventArgs.CopyAddressesFrom(ie);
								serverAsyncEventArgs.LocalEndPoint = base.GetLocalEndpoint(ie.RemoteEndPoint.Address);
								base.SendAsync(connection, serverAsyncEventArgs);
								serverAsyncEventArgs = null;
							}
							int bufferIndex = secBufferDescEx.GetBufferIndex(BufferType.SECBUFFER_EXTRA, 0);
							if (bufferIndex < 0)
							{
								sspiContext.Buffer.Free();
							}
							else if (sspiContext.Buffer.IsInvalid)
							{
								if (!sspiContext.Buffer.CopyTransferredFrom(ie, ie.BytesTransferred - secBufferDescEx.Buffers[bufferIndex].Size))
								{
									goto Block_21;
								}
							}
							else
							{
								sspiContext.Buffer.MoveToBegin(sspiContext.Buffer.BytesTransferred - secBufferDescEx.Buffers[bufferIndex].Size, secBufferDescEx.Buffers[bufferIndex].Size);
							}
							SecurityStatus securityStatus3 = securityStatus;
							if (securityStatus3 == SecurityStatus.SEC_E_OK)
							{
								goto IL_2FF;
							}
							if (securityStatus3 != SecurityStatus.SEC_I_CONTINUE_NEEDED)
							{
								goto Block_23;
							}
							if (bufferIndex < 0)
							{
								goto Block_25;
							}
						}
						else
						{
							if (serverAsyncEventArgs.Count >= this.maxTokenSize)
							{
								goto IL_1DC;
							}
							serverAsyncEventArgs.Count = this.maxTokenSize;
							serverAsyncEventArgs.ReAllocateBuffer(false);
						}
					}
					if (sspiContext.Buffer.IsInvalid && !sspiContext.Buffer.CopyTransferredFrom(ie, 0))
					{
						result = false;
						return result;
					}
					result = true;
					return result;
					IL_1DC:
					result = false;
					return result;
					Block_21:
					result = false;
					return result;
					Block_23:
					result = false;
					return result;
					IL_2FF:
					if (Sspi.SafeQueryContextAttributes(ref sspiContext.Handle, out sspiContext.StreamSizes) != SecurityStatus.SEC_E_OK)
					{
						result = false;
						return result;
					}
					sspiContext.Connected = true;
					this.OnNewConnection(connection);
					result = true;
					return result;
					Block_25:
					result = true;
				}
			}
			finally
			{
				if (serverAsyncEventArgs != null)
				{
					EventArgsManager.Put(ref serverAsyncEventArgs);
				}
			}
			return result;
		}

		private void SetSecBuffer(ref SecBufferEx secBuffer, ServerAsyncEventArgs e)
		{
			secBuffer.Buffer = e.Buffer;
			secBuffer.Offset = e.Offset;
			secBuffer.Size = e.BytesTransferred;
		}

		public void SetSecBuffer(ref SecBufferEx secBuffer, SspiContext context)
		{
			secBuffer.Buffer = context.Buffer.Array;
			secBuffer.Offset = context.Buffer.Offset;
			secBuffer.Size = context.Buffer.BytesTransferred;
		}

		private void GetMaxTokenSize()
		{
			int num;
			SafeContextBufferHandle safeContextBufferHandle;
			if (Sspi.EnumerateSecurityPackages(out num, out safeContextBufferHandle) != SecurityStatus.SEC_E_OK)
			{
				throw new Win32Exception("Failed to EnumerateSecurityPackages");
			}
			for (int i = 0; i < num; i++)
			{
				SecPkgInfo item = safeContextBufferHandle.GetItem<SecPkgInfo>(i);
				if (string.Compare(item.GetName(), "Schannel", true) == 0)
				{
					this.maxTokenSize = item.cbMaxToken;
					break;
				}
			}
			if (this.maxTokenSize == 0)
			{
				throw new Exception("Failed to retrive cbMaxToken for Schannel");
			}
		}
	}
}
