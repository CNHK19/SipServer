using System;

namespace Microsoft.Win32.Ssp
{
	internal struct SecBuffer
	{
		internal int cbBuffer;

		internal int BufferType;

		internal IntPtr pvBuffer;

		public SecBuffer(BufferType type, int count, IntPtr buffer)
		{
			this.BufferType = (int)type;
			this.cbBuffer = count;
			this.pvBuffer = buffer;
		}
	}
}
