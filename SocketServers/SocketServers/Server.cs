using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketServers
{
	internal abstract class Server<C> : IDisposable where C : BaseConnection, IDisposable, new()
	{
		internal class Connection<C2> : IDisposable where C2 : IDisposable
		{
			internal class CyclicBuffer : IDisposable
			{
				private bool disposed;

				private int size;

				private volatile int dequeueIndex;

				private ServerAsyncEventArgs[] queue;

				public volatile int SequenceNumber;

				public CyclicBuffer(int size1)
				{
					this.disposed = false;
					this.size = size1;
					this.dequeueIndex = 0;
					this.SequenceNumber = 0;
					this.queue = new ServerAsyncEventArgs[this.size];
				}

				public void Dispose()
				{
					this.disposed = true;
					ServerAsyncEventArgs serverAsyncEventArgs = null;
					for (int i = 0; i < this.queue.Length; i++)
					{
						if (this.queue[i] != null)
						{
							serverAsyncEventArgs = Interlocked.Exchange<ServerAsyncEventArgs>(ref this.queue[i], null);
						}
						if (serverAsyncEventArgs != null)
						{
							EventArgsManager.Put(ref serverAsyncEventArgs);
						}
					}
				}

				public void Put(ServerAsyncEventArgs e)
				{
					int num = e.SequenceNumber % this.size;
					Interlocked.Exchange<ServerAsyncEventArgs>(ref this.queue[num], e);
					if (this.disposed && Interlocked.Exchange<ServerAsyncEventArgs>(ref this.queue[num], null) != null)
					{
						EventArgsManager.Put(e);
					}
				}

				public ServerAsyncEventArgs GetCurrent()
				{
					return Interlocked.Exchange<ServerAsyncEventArgs>(ref this.queue[this.dequeueIndex], null);
				}

				public void Next()
				{
					Interlocked.Exchange(ref this.dequeueIndex, (this.dequeueIndex + 1) % this.size);
				}
			}

			private static int connectionCount;

			private SspiContext sspiContext;

			private int closeCount;

			public readonly int Id;

			public readonly Socket Socket;

			public readonly bool IsSocketAccepted;

			public readonly SpinLock SpinLock;

			public readonly Server<C>.Connection<C2>.CyclicBuffer ReceiveQueue;

			public readonly IPEndPoint RemoteEndPoint;

			public C2 UserConnection;

			internal bool IsClosed
			{
				get
				{
					return Thread.VolatileRead(ref this.closeCount) > 0;
				}
			}

			public SspiContext SspiContext
			{
				get
				{
					if (this.sspiContext == null)
					{
						this.sspiContext = new SspiContext();
					}
					return this.sspiContext;
				}
			}

			public Connection(Socket socket, bool isSocketAccepted, int receivedQueueSize)
			{
				this.Id = this.NewConnectionId();
				this.ReceiveQueue = new Server<C>.Connection<C2>.CyclicBuffer(receivedQueueSize);
				this.SpinLock = new SpinLock();
				this.Socket = socket;
				this.IsSocketAccepted = isSocketAccepted;
				this.RemoteEndPoint = (socket.RemoteEndPoint as IPEndPoint);
			}

			internal bool Close()
			{
				bool flag = Interlocked.Increment(ref this.closeCount) == 1;
				if (flag)
				{
					this.ReceiveQueue.Dispose();
					if (this.sspiContext != null)
					{
						this.sspiContext.Dispose();
					}
					if (this.UserConnection != null)
					{
						this.UserConnection.Dispose();
					}
				}
				return flag;
			}

			void IDisposable.Dispose()
			{
				if (this.Close())
				{
					this.Socket.SafeShutdownClose();
				}
			}

			private int NewConnectionId()
			{
				int num;
				do
				{
					num = Interlocked.Increment(ref Server<C>.Connection<C2>.connectionCount);
				}
				while (num == -1 || num == -2);
				return num;
			}
		}

		protected volatile bool isRunning;

		protected ServerEndPoint realEndPoint;

		private ServerEndPoint fakeEndPoint;

		private long ip4Mask;

		private long ip4Subnet;

		public ServerEventHandlerVal<Server<C>, ServerInfoEventArgs> Failed;

		public ServerEventHandlerRef<Server<C>, C, ServerAsyncEventArgs, bool> Received;

		public ServerEventHandlerVal<Server<C>, ServerAsyncEventArgs> Sent;

		public ServerEventHandlerVal<Server<C>, C, ServerAsyncEventArgs> BeforeSend;

		public ServerEventHandlerVal<Server<C>, C> NewConnection;

		public ServerEventHandlerVal<Server<C>, C> EndConnection;

		public ServerEndPoint LocalEndPoint
		{
			get
			{
				return this.realEndPoint;
			}
		}

		public ServerEndPoint FakeEndPoint
		{
			get
			{
				return this.fakeEndPoint;
			}
		}

		public Server()
		{
		}

		public abstract void Start();

		public abstract void Dispose();

		public abstract void SendAsync(ServerAsyncEventArgs e);

		protected void Send_Completed(Socket socket, ServerAsyncEventArgs e)
		{
			this.Sent(this, e);
		}

		protected virtual bool OnReceived(Server<C>.Connection<C> c, ref ServerAsyncEventArgs e)
		{
			e.LocalEndPoint = this.GetLocalEndpoint(e.RemoteEndPoint.Address);
			return this.Received(this, (c != null) ? c.UserConnection : default(C), ref e);
		}

		protected virtual void OnFailed(ServerInfoEventArgs e)
		{
			this.Failed(this, e);
		}

		protected virtual void OnNewConnection(Server<C>.Connection<C> connection)
		{
			C userConnection = Activator.CreateInstance<C>();
			userConnection.LocalEndPoint = this.GetLocalEndpoint(connection.RemoteEndPoint.Address);
			userConnection.RemoteEndPoint = connection.RemoteEndPoint;
			userConnection.Id = connection.Id;
			connection.UserConnection = userConnection;
			this.NewConnection(this, connection.UserConnection);
		}

		protected virtual void OnEndConnection(Server<C>.Connection<C> connection)
		{
			this.EndConnection(this, connection.UserConnection);
		}

		protected void OnBeforeSend(Server<C>.Connection<C> connection, ServerAsyncEventArgs e)
		{
			this.BeforeSend(this, (connection == null) ? default(C) : connection.UserConnection, e);
		}

		public static Server<C> Create(ServerEndPoint real, IPEndPoint ip4fake, IPAddress ip4mask, ServersManagerConfig config)
		{
			Server<C> server;
			if (real.Protocol == ServerProtocol.Tcp)
			{
				server = new TcpServer<C>(config);
			}
			else if (real.Protocol == ServerProtocol.Udp)
			{
				server = new UdpServer<C>(config);
			}
			else
			{
				if (real.Protocol != ServerProtocol.Tls)
				{
					throw new InvalidOperationException("Protocol is not supported.");
				}
				server = new SspiTlsServer<C>(config);
			}
			server.realEndPoint = real.Clone();
			if (ip4fake != null)
			{
				if (ip4mask == null)
				{
					throw new ArgumentNullException("ip4mask");
				}
				server.fakeEndPoint = new ServerEndPoint(server.realEndPoint.Protocol, ip4fake);
				server.ip4Mask = Server<C>.GetIPv4Long(ip4mask);
				server.ip4Subnet = (Server<C>.GetIPv4Long(real.Address) & server.ip4Mask);
			}
			return server;
		}

		public ServerEndPoint GetLocalEndpoint(IPAddress addr)
		{
			if (this.fakeEndPoint != null && !IPAddress.IsLoopback(addr))
			{
				long iPv4Long = Server<C>.GetIPv4Long(addr);
				if ((iPv4Long & this.ip4Mask) != this.ip4Subnet)
				{
					return this.fakeEndPoint;
				}
			}
			return this.realEndPoint;
		}

		private static long GetIPv4Long(IPAddress address)
		{
			return address.Address;
		}
	}
}
