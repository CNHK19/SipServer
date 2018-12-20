using System;
using System.Net;

namespace SocketServers
{
	public class BaseConnection
	{
		public ServerEndPoint LocalEndPoint
		{
			get;
			internal set;
		}

		public IPEndPoint RemoteEndPoint
		{
			get;
			internal set;
		}

		public int Id
		{
			get;
			internal set;
		}

		public void Dispose()
		{
		}
	}
}
