using Microsoft.Win32.Ssp;
using System;

namespace SocketServers
{
	internal class SspiContext : IDisposable
	{
		public bool Connected;

		public SafeCtxtHandle Handle;

		public SecBufferDescEx SecBufferDesc5;

		public SecBufferDescEx[] SecBufferDesc2;

		public SecPkgContext_StreamSizes StreamSizes;

		public readonly StreamBuffer Buffer;

		public SspiContext()
		{
			this.Handle = new SafeCtxtHandle();
			this.SecBufferDesc5 = new SecBufferDescEx(new SecBufferEx[5]);
			this.SecBufferDesc2 = new SecBufferDescEx[]
			{
				new SecBufferDescEx(new SecBufferEx[2]),
				new SecBufferDescEx(new SecBufferEx[2])
			};
			this.Buffer = new StreamBuffer();
		}

		public void Dispose()
		{
			this.Handle.Dispose();
			this.Buffer.Dispose();
		}
	}
}
