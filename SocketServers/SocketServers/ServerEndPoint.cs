using System;
using System.Net;

namespace SocketServers
{
	public class ServerEndPoint : IPEndPoint, IEquatable<ServerEndPoint>
	{
		public static ServerEndPoint NoneEndPoint = new ServerEndPoint(ServerProtocol.Tcp, IPAddress.None, 0);

		public ServerProtocol Protocol
		{
			get;
			set;
		}

		public ProtocolPort ProtocolPort
		{
			get
			{
				return new ProtocolPort(this.Protocol, base.Port);
			}
		}

		public ServerEndPoint(ProtocolPort protocolPort, IPAddress address) : base(address, protocolPort.Port)
		{
			this.Protocol = protocolPort.Protocol;
		}

		public ServerEndPoint(ServerProtocol protocol, IPAddress address, int port) : base(address, port)
		{
			this.Protocol = protocol;
		}

		public ServerEndPoint(ServerProtocol protocol, IPEndPoint endpoint) : base(endpoint.Address, endpoint.Port)
		{
			this.Protocol = protocol;
		}

		public new bool Equals(object x)
		{
			return x is ServerEndPoint && this.Equals(x as ServerEndPoint);
		}

		public bool Equals(ServerEndPoint p)
		{
			return this.AddressFamily == p.AddressFamily && base.Port == p.Port && base.Address.Equals(p.Address) && this.Protocol == p.Protocol;
		}

		public bool Equals(ServerProtocol protocol, IPEndPoint endpoint)
		{
			return this.AddressFamily == endpoint.AddressFamily && base.Port == endpoint.Port && base.Address.Equals(endpoint.Address) && this.Protocol == protocol;
		}

		public new string ToString()
		{
			return string.Format("{0}:{1}", this.Protocol.ToString(), base.ToString());
		}

		public ServerEndPoint Clone()
		{
			return new ServerEndPoint(this.Protocol, base.Address, base.Port);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (int)this.Protocol;
		}
	}
}
