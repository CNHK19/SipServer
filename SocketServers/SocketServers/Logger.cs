using Pcap;
using System;
using System.IO;

namespace SocketServers
{
	public class Logger : IDisposable
	{
		private object sync;

		private PcapWriter writer;

		public bool IsEnabled
		{
			get;
			private set;
		}

		public Logger()
		{
			this.sync = new object();
		}

		internal void Dispose()
		{
			((IDisposable)this).Dispose();
		}

		void IDisposable.Dispose()
		{
			if (this.writer != null)
			{
				this.writer.Dispose();
			}
		}

		public void Enable(string filename)
		{
			this.Enable(File.Create(filename));
		}

		public void Enable(Stream stream)
		{
			lock (this.sync)
			{
				if (this.IsEnabled)
				{
					this.Disable();
				}
				this.writer = new PcapWriter(stream);
				this.IsEnabled = true;
			}
		}

		public void Disable()
		{
			lock (this.sync)
			{
				this.IsEnabled = false;
				if (this.writer != null)
				{
					this.writer.Dispose();
				}
				this.writer = null;
			}
		}

		public void Flush()
		{
			try
			{
				PcapWriter pcapWriter = this.writer;
				if (pcapWriter != null)
				{
					pcapWriter.Flush();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public void WriteComment(string comment)
		{
			try
			{
				PcapWriter pcapWriter = this.writer;
				if (pcapWriter != null)
				{
					pcapWriter.WriteComment(comment);
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		internal void Write(ServerAsyncEventArgs e, bool incomingOutgoing)
		{
			try
			{
				PcapWriter pcapWriter = this.writer;
				if (pcapWriter != null)
				{
					pcapWriter.Write(e.Buffer, e.Offset, incomingOutgoing ? e.BytesTransferred : e.Count, this.Convert(e.LocalEndPoint.Protocol), incomingOutgoing ? e.RemoteEndPoint : e.LocalEndPoint, incomingOutgoing ? e.LocalEndPoint : e.RemoteEndPoint);
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private Protocol Convert(ServerProtocol source)
		{
			if (source == ServerProtocol.Udp)
			{
				return Protocol.Udp;
			}
			if (source == ServerProtocol.Tls)
			{
				return Protocol.Tls;
			}
			return Protocol.Tcp;
		}
	}
}
