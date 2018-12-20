using System;
using System.Net.Sockets;

namespace SocketServers
{
	public class ServerInfoEventArgs : ServerChangeEventArgs
	{
		public SocketError SocketError
		{
			get;
			private set;
		}

		public Exception Exception
		{
			get;
			private set;
		}

		public string Error
		{
			get;
			private set;
		}

		internal ServerInfoEventArgs(ServerEndPoint serverEndPoint, SocketError error) : base(serverEndPoint)
		{
			this.SocketError = error;
		}

		internal ServerInfoEventArgs(ServerEndPoint serverEndPoint, Exception error) : base(serverEndPoint)
		{
			this.Exception = error;
		}

		internal ServerInfoEventArgs(ServerEndPoint serverEndPoint, SocketException error) : base(serverEndPoint)
		{
			this.SocketError = error.SocketErrorCode;
			this.Exception = error;
		}

		internal ServerInfoEventArgs(ServerEndPoint serverEndPoint, string error) : base(serverEndPoint)
		{
			this.Error = error;
		}

		internal ServerInfoEventArgs(ServerEndPoint serverEndPoint, string api, string function, uint error) : base(serverEndPoint)
		{
			this.Error = ServerInfoEventArgs.Format(api, function, error);
		}

		internal static string Format(string api, string function, uint error)
		{
			return string.Format("{0} error, function call {1} return 0x{2:x8}", api, function, error);
		}

		public override string ToString()
		{
			if (this.Error != null)
			{
				return this.Error;
			}
			if (this.Exception != null)
			{
				return this.Exception.ToString();
			}
			return "SocketError :" + this.SocketError.ToString();
		}
	}
}
