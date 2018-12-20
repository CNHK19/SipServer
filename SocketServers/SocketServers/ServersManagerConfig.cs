using System;
using System.Security.Cryptography.X509Certificates;

namespace SocketServers
{
	public class ServersManagerConfig
	{
		public int MinPort;

		public int MaxPort;

		public int UdpQueueSize;

		public int TcpMinAcceptBacklog;

		public int TcpMaxAcceptBacklog;

		public int TcpQueueSize;

		public X509Certificate2 TlsCertificate;

		public ServersManagerConfig()
		{
			this.TcpMinAcceptBacklog = 1024;
			this.TcpMaxAcceptBacklog = 2048;
			this.TcpQueueSize = 8;
		}
	}
}
