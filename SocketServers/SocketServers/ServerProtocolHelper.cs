using System;

namespace SocketServers
{
	public static class ServerProtocolHelper
	{
		public static bool TryConvertTo(this string protocolName, out ServerProtocol protocol)
		{
			if (string.Compare(protocolName, "udp", true) == 0)
			{
				protocol = ServerProtocol.Udp;
				return true;
			}
			if (string.Compare(protocolName, "tcp", true) == 0)
			{
				protocol = ServerProtocol.Tcp;
				return true;
			}
			if (string.Compare(protocolName, "tls", true) == 0)
			{
				protocol = ServerProtocol.Tls;
				return true;
			}
			protocol = ServerProtocol.Udp;
			return false;
		}

		public static ServerProtocol ConvertTo(this string protocolName)
		{
			ServerProtocol result;
			if (!protocolName.TryConvertTo(out result))
			{
				throw new ArgumentOutOfRangeException("protocolName");
			}
			return result;
		}
	}
}
