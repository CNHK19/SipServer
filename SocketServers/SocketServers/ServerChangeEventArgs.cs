using System;

namespace SocketServers
{
	public class ServerChangeEventArgs : EventArgs
	{
		public ServerEndPoint ServerEndPoint
		{
			get;
			private set;
		}

		public ServerChangeEventArgs(ServerEndPoint serverEndPoint)
		{
			this.ServerEndPoint = serverEndPoint;
		}
	}
}
