using System;
using System.Net.Sockets;
using System.Threading;

namespace SocketServers
{
	internal class UdpServer<C> : Server<C> where C : BaseConnection, IDisposable, new()
	{
		private const int SIO_UDP_CONNRESET = -1744830452;

		private object sync;

		private Socket socket;

		private int queueSize;

		public UdpServer(ServersManagerConfig config)
		{
			this.sync = new object();
			this.queueSize = ((config.UdpQueueSize > 0) ? config.UdpQueueSize : 16);
		}

		public override void Start()
		{
			lock (this.sync)
			{
				this.isRunning = true;
				this.socket = new Socket(this.realEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				this.socket.Bind(this.realEndPoint);
				this.socket.IOControl(-1744830452, new byte[4], null);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.EnqueueBuffers), this.queueSize);
			}
		}

		public override void Dispose()
		{
			this.isRunning = false;
			lock (this.sync)
			{
				if (this.socket != null)
				{
					this.socket.SafeShutdownClose();
					this.socket = null;
				}
			}
		}

		public override void SendAsync(ServerAsyncEventArgs e)
		{
			base.OnBeforeSend(null, e);
			e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(base.Send_Completed);
			if (!this.socket.SendToAsync(e))
			{
				e.OnCompleted(this.socket);
			}
		}

		private void EnqueueBuffers(object stateInfo)
		{
			int num = (int)stateInfo;
			lock (this.sync)
			{
				if (this.socket != null)
				{
					int num2 = 0;
					while (num2 < num && this.isRunning)
					{
						ServerAsyncEventArgs serverAsyncEventArgs = EventArgsManager.Get();
						this.PrepareBuffer(serverAsyncEventArgs);
						if (!this.socket.ReceiveFromAsync(serverAsyncEventArgs))
						{
							serverAsyncEventArgs.OnCompleted(this.socket);
						}
						num2++;
					}
				}
			}
		}

		private void ReceiveFrom_Completed(Socket socket, ServerAsyncEventArgs e)
		{
			while (this.isRunning)
			{
				if (e.SocketError == SocketError.Success)
				{
					this.OnReceived(null, ref e);
					if (e == null)
					{
						e = EventArgsManager.Get();
					}
					this.PrepareBuffer(e);
					try
					{
						if (socket.ReceiveFromAsync(e))
						{
							e = null;
							break;
						}
						continue;
					}
					catch (ObjectDisposedException)
					{
						continue;
					}
				}
				if (this.isRunning)
				{
					this.Dispose();
					this.OnFailed(new ServerInfoEventArgs(this.realEndPoint, e.SocketError));
				}
			}
			if (e != null)
			{
				EventArgsManager.Put(e);
			}
		}

		private void PrepareBuffer(ServerAsyncEventArgs e)
		{
			e.Completed = new ServerAsyncEventArgs.CompletedEventHandler(this.ReceiveFrom_Completed);
			e.SetAnyRemote(this.realEndPoint.AddressFamily);
			e.AllocateBuffer();
		}
	}
}
