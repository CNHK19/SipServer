using System;
using System.Net.Sockets;

namespace SocketServers
{
	internal static class SocketEx
	{
		public static void SafeShutdownClose(this Socket socket)
		{
			try
			{
				try
				{
					if (socket.Connected)
					{
						socket.Shutdown(SocketShutdown.Both);
					}
				}
				catch
				{
				}
				socket.Close();
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public static void SendAsync(this Socket socket, ServerAsyncEventArgs e, ServerAsyncEventArgs.CompletedEventHandler handler)
		{
			e.Completed = handler;
			if (!socket.SendAsync(e))
			{
				e.OnCompleted(socket);
			}
		}

		public static void ConnectAsync(this Socket socket, ServerAsyncEventArgs e, ServerAsyncEventArgs.CompletedEventHandler handler)
		{
			e.Completed = handler;
			if (!socket.ConnectAsync(e))
			{
				e.OnCompleted(socket);
			}
		}

		public static void AcceptAsync(this Socket socket, ServerAsyncEventArgs e, ServerAsyncEventArgs.CompletedEventHandler handler)
		{
			e.Completed = handler;
			if (!socket.AcceptAsync(e))
			{
				e.OnCompleted(socket);
			}
		}
	}
}
