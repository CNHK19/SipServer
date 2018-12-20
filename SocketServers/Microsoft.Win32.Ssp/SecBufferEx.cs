using System;

namespace Microsoft.Win32.Ssp
{
	public struct SecBufferEx
	{
		public BufferType BufferType;

		public int Size;

		public int Offset;

		public object Buffer;

		public void SetBuffer(BufferType type, byte[] bytes)
		{
			this.BufferType = type;
			this.Buffer = bytes;
			this.Offset = 0;
			this.Size = bytes.Length;
		}

		public void SetBuffer(BufferType type, byte[] bytes, int offset, int size)
		{
			this.BufferType = type;
			this.Buffer = bytes;
			this.Offset = offset;
			this.Size = size;
		}

		public void SetBufferEmpty()
		{
			this.BufferType = BufferType.SECBUFFER_VERSION;
			this.Buffer = null;
			this.Offset = 0;
			this.Size = 0;
		}
	}
}
