using System;

namespace Base.Message
{
	public interface IBufferManager
	{
		ArraySegment<byte> Allocate(int size);

		void Reallocate(ref ArraySegment<byte> segment, int extraSize);

		void Free(ref ArraySegment<byte> segment);
	}
}
