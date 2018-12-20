using System;

namespace SocketServers
{
	public struct ProtocolPort
	{
		public int Port;

		public ServerProtocol Protocol;

		public ProtocolPort(ServerProtocol protocol, int port)
		{
			this.Protocol = protocol;
			this.Port = port;
		}
	}
}
