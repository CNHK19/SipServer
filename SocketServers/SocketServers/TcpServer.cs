using System;

namespace SocketServers
{
	internal class TcpServer<C> : BaseTcpServer<C> where C : BaseConnection, IDisposable, new()
	{
		public TcpServer(ServersManagerConfig config) : base(config)
		{
		}

		protected override void OnNewTcpConnection(Server<C>.Connection<C> connection)
		{
			this.OnNewConnection(connection);
		}

		protected override void OnEndTcpConnection(Server<C>.Connection<C> connection)
		{
			this.OnEndConnection(connection);
		}

		protected override bool OnTcpReceived(Server<C>.Connection<C> connection, ref ServerAsyncEventArgs e)
		{
			return this.OnReceived(connection, ref e);
		}

		public override void SendAsync(ServerAsyncEventArgs e)
		{
			Server<C>.Connection<C> tcpConnection = base.GetTcpConnection(e.RemoteEndPoint);
			base.OnBeforeSend(tcpConnection, e);
			base.SendAsync(tcpConnection, e);
		}
	}
}
