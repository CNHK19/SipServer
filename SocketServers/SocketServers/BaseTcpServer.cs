using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketServers
{
	internal abstract class BaseTcpServer<C> : Server<C> where C : BaseConnection, IDisposable, new()
	{
		private readonly object sync;

		private Socket listener;

		private ThreadSafeDictionary<EndPoint, Server<C>.Connection<C>> connections;

		private readonly int receiveQueueSize;

		private bool socketReuseEnabled;

		private readonly int maxAcceptBacklog;

		private readonly int minAcceptBacklog;

		private int acceptBacklog;

		public BaseTcpServer(ServersManagerConfig config)
		{
			this.sync = new object();
			this.receiveQueueSize = config.TcpQueueSize;
			this.minAcceptBacklog = config.TcpMinAcceptBacklog;
			this.maxAcceptBacklog = config.TcpMaxAcceptBacklog;
			this.socketReuseEnabled = (this.minAcceptBacklog < this.maxAcceptBacklog);
		}

		public override void Start()
		{
			lock (this.sync)
			{
				this.isRunning = true;
				this.connections = new ThreadSafeDictionary<EndPoint, Server<C>.Connection<C>>();
				this.listener = new Socket(this.realEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
				this.listener.Bind(this.realEndPoint);
				this.listener.Listen(0);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.EnqueueAsyncAccepts), null);
			}
		}

		private void EnqueueAsyncAccepts(object stateInfo)
		{
			lock (this.sync)
			{
				while (true)
				{
					int num = Thread.VolatileRead(ref this.acceptBacklog);
					if (!this.isRunning || num >= this.minAcceptBacklog)
					{
						break;
					}
					if (Interlocked.CompareExchange(ref this.acceptBacklog, num + 1, num) == num)
					{
						ServerAsyncEventArgs serverAsyncEventArgs = EventArgsManager.Get();
						serverAsyncEventArgs.FreeBuffer();
						this.listener.AcceptAsync(serverAsyncEventArgs, new ServerAsyncEventArgs.CompletedEventHandler(this.Accept_Completed));
					}
				}
			}
		}

		public override void Dispose()
		{
			this.isRunning = false;
			lock (this.sync)
			{
				if (this.listener != null)
				{
					this.connections.ForEach(new Action<Server<C>.Connection<C>>(this.EndTcpConnection));
					this.connections.Clear();
					this.listener.Close();
					this.listener = null;
				}
			}
		}

		protected abstract void OnNewTcpConnection(Server<C>.Connection<C> connection);

		protected abstract void OnEndTcpConnection(Server<C>.Connection<C> connection);

		protected abstract bool OnTcpReceived(Server<C>.Connection<C> connection, ref ServerAsyncEventArgs e);

		protected void SendAsync(Server<C>.Connection<C> connection, ServerAsyncEventArgs e)
		{
			if (connection == null)
			{
				if (e.ConnectionId == -1)
				{
					try
					{
						Socket socket = new Socket(this.realEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
						socket.Bind(this.realEndPoint);
						socket.ConnectAsync(e, new ServerAsyncEventArgs.CompletedEventHandler(this.Connect_Completed));
						return;
					}
					catch (SocketException ex)
					{
						e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed);
						e.SocketError = ex.SocketErrorCode;
						e.OnCompleted(null);
						return;
					}
				}
				e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed);
				e.SocketError = SocketError.NotConnected;
				e.OnCompleted(null);
				return;
			}
			if (e.ConnectionId == -1 || e.ConnectionId == -2 || e.ConnectionId == connection.Id)
			{
				connection.Socket.SendAsync(e, new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed));
				return;
			}
			e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed);
			e.SocketError = SocketError.NotConnected;
			e.OnCompleted(null);
		}

		private void Connect_Completed(Socket socket1, ServerAsyncEventArgs e)
		{
			bool flag = false;
			Server<C>.Connection<C> connection = null;
			if (e.SocketError == SocketError.Success)
			{
				connection = this.CreateConnection(socket1, e.SocketError);
				flag = (connection != null);
			}
			else
			{
				while (e.SocketError == SocketError.AddressAlreadyInUse && !this.connections.TryGetValue(e.RemoteEndPoint, out connection))
				{
					Thread.Sleep(0);
					if (socket1.ConnectAsync(e))
					{
						return;
					}
					if (e.SocketError == SocketError.Success)
					{
						connection = this.CreateConnection(socket1, e.SocketError);
						flag = (connection != null);
					}
				}
			}
			if (e.SocketError == SocketError.Success && flag)
			{
				this.NewTcpConnection(connection);
			}
			e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed);
			e.OnCompleted(socket1);
		}

		private Server<C>.Connection<C> CreateConnection(Socket socket, SocketError error)
		{
			Server<C>.Connection<C> connection = null;
			if (this.isRunning && error == SocketError.Success)
			{
				connection = new Server<C>.Connection<C>(socket, true, this.receiveQueueSize);
				Server<C>.Connection<C> connection2 = this.connections.Replace(connection.RemoteEndPoint, connection);
				if (connection2 != null)
				{
					this.EndTcpConnection(connection2);
				}
			}
			else if (socket != null)
			{
				socket.SafeShutdownClose();
			}
			return connection;
		}

		private void Accept_Completed(Socket none, ServerAsyncEventArgs e)
		{
			Socket acceptSocket = e.AcceptSocket;
			SocketError socketError = e.SocketError;
			while (true)
			{
				int num = Thread.VolatileRead(ref this.acceptBacklog);
				if (this.isRunning && num <= this.minAcceptBacklog)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this.acceptBacklog, num - 1, num) == num)
				{
					goto Block_3;
				}
			}
			e.AcceptSocket = null;
			this.listener.AcceptAsync(e, new ServerAsyncEventArgs.CompletedEventHandler(this.Accept_Completed));
			goto IL_66;
			Block_3:
			EventArgsManager.Put(e);
			IL_66:
			Server<C>.Connection<C> connection = this.CreateConnection(acceptSocket, socketError);
			if (connection != null)
			{
				this.NewTcpConnection(connection);
			}
		}

		private void NewTcpConnection(Server<C>.Connection<C> connection)
		{
			this.OnNewTcpConnection(connection);
			ServerAsyncEventArgs serverAsyncEventArgs;
			for (int i = 0; i < this.receiveQueueSize; i++)
			{
				serverAsyncEventArgs = EventArgsManager.Get();
				if (!this.TcpReceiveAsync(connection, serverAsyncEventArgs))
				{
					connection.ReceiveQueue.Put(serverAsyncEventArgs);
				}
			}
			serverAsyncEventArgs = connection.ReceiveQueue.GetCurrent();
			if (serverAsyncEventArgs != null)
			{
				this.Receive_Completed(connection.Socket, serverAsyncEventArgs);
			}
		}

		private void EndTcpConnection(Server<C>.Connection<C> connection)
		{
			if (connection.Close())
			{
				this.OnEndTcpConnection(connection);
				if (connection.Socket.Connected)
				{
					try
					{
						connection.Socket.Shutdown(SocketShutdown.Both);
					}
					catch (SocketException)
					{
					}
				}
				if (connection.IsSocketAccepted && this.socketReuseEnabled)
				{
					try
					{
						try
						{
							ServerAsyncEventArgs serverAsyncEventArgs = EventArgsManager.Get();
							serverAsyncEventArgs.FreeBuffer();
							serverAsyncEventArgs.DisconnectReuseSocket = true;
							serverAsyncEventArgs.Completed = new ServerAsyncEventArgs.CompletedEventHandler(this.Disconnect_Completed);
							if (!connection.Socket.DisconnectAsync(serverAsyncEventArgs))
							{
								serverAsyncEventArgs.OnCompleted(connection.Socket);
							}
						}
						catch (SocketException)
						{
						}
					}
					catch (NotSupportedException)
					{
						this.socketReuseEnabled = false;
					}
				}
				if (!this.socketReuseEnabled)
				{
					connection.Socket.Close();
				}
			}
		}

		private void Disconnect_Completed(Socket socket, ServerAsyncEventArgs e)
		{
			int num;
			do
			{
				num = Thread.VolatileRead(ref this.acceptBacklog);
				if (!this.isRunning || num >= this.maxAcceptBacklog)
				{
					goto IL_5B;
				}
			}
			while (Interlocked.CompareExchange(ref this.acceptBacklog, num + 1, num) != num);
			e.AcceptSocket = socket;
			try
			{
				this.listener.AcceptAsync(e, new ServerAsyncEventArgs.CompletedEventHandler(this.Accept_Completed));
				return;
			}
			catch
			{
				EventArgsManager.Put(e);
				return;
			}
			IL_5B:
			EventArgsManager.Put(e);
		}

		private void Receive_Completed(Socket socket, ServerAsyncEventArgs e)
		{
			try
			{
				Server<C>.Connection<C> connection;
				this.connections.TryGetValue(e.RemoteEndPoint, out connection);
				if (connection != null && connection.Socket == socket && connection.Id == e.ConnectionId)
				{
					while (true)
					{
						if (e != null)
						{
							connection.ReceiveQueue.Put(e);
							e = null;
						}
						e = connection.ReceiveQueue.GetCurrent();
						if (e == null)
						{
							goto IL_D4;
						}
						bool flag = true;
						if (this.isRunning && e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
						{
							flag = !this.OnTcpReceived(connection, ref e);
						}
						if (flag)
						{
							break;
						}
						connection.ReceiveQueue.Next();
						if (e == null)
						{
							e = EventArgsManager.Get();
						}
						else
						{
							e.SetDefaultValue();
						}
						if (this.TcpReceiveAsync(connection, e))
						{
							e = null;
						}
					}
					this.connections.Remove(connection.RemoteEndPoint, connection);
					this.EndTcpConnection(connection);
				}
				IL_D4:;
			}
			finally
			{
				if (e != null)
				{
					EventArgsManager.Put(ref e);
				}
			}
		}

		private bool TcpReceiveAsync(Server<C>.Connection<C> connection, ServerAsyncEventArgs e)
		{
			this.PrepareEventArgs(connection, e);
			try
			{
				connection.SpinLock.Enter();
				e.SequenceNumber = connection.ReceiveQueue.SequenceNumber;
				try
				{
					if (!connection.IsClosed)
					{
						bool result = connection.Socket.ReceiveAsync(e);
						connection.ReceiveQueue.SequenceNumber++;
						return result;
					}
				}
				finally
				{
					connection.SpinLock.Exit();
				}
			}
			catch (ObjectDisposedException)
			{
			}
			EventArgsManager.Put(ref e);
			return true;
		}

		protected void PrepareEventArgs(Server<C>.Connection<C> connection, ServerAsyncEventArgs e)
		{
			e.ConnectionId = connection.Id;
			e.RemoteEndPoint = connection.RemoteEndPoint;
			e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(this.Receive_Completed);
		}

		protected Server<C>.Connection<C> GetTcpConnection(IPEndPoint remote)
		{
			Server<C>.Connection<C> connection = null;
			if (this.connections.TryGetValue(remote, out connection) && !connection.Socket.Connected)
			{
				this.connections.Remove(remote, connection);
				this.EndTcpConnection(connection);
				connection = null;
			}
			return connection;
		}
	}
}
