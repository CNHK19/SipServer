using System;

namespace Base.Message
{
	public class BufferManager : IBufferManager
	{
		public ArraySegment<byte> Allocate(int size)
		{
			return new ArraySegment<byte>(new byte[size], 0, size);
		}

		public void Reallocate(ref ArraySegment<byte> segment, int extraSize)
		{
			ArraySegment<byte> arraySegment = new ArraySegment<byte>(new byte[segment.Count + extraSize], 0, segment.Count + extraSize);
			Buffer.BlockCopy(segment.Array, 0, arraySegment.Array, 0, segment.Count);
			segment = arraySegment;
		}

		public void Free(ref ArraySegment<byte> segment)
		{
			segment = default(ArraySegment<byte>);
		}
	}
}
