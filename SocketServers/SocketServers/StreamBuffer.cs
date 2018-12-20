using Microsoft.Win32.Ssp;
using System;

namespace SocketServers
{
	public class StreamBuffer : IDisposable
	{
		private ArraySegment<byte> segment;

		public byte[] Array
		{
			get
			{
				return this.segment.Array;
			}
		}

		public int Offset
		{
			get
			{
				return this.segment.Offset;
			}
		}

		public int Count
		{
			get;
			private set;
		}

		public int Capacity
		{
			get;
			private set;
		}

		public int FreeSize
		{
			get
			{
				return this.Capacity - this.Count;
			}
		}

		public int BytesTransferred
		{
			get
			{
				return this.Count;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.segment.Array != null && this.segment.Offset >= 0 && this.segment.Count > 0;
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.segment.Array == null || this.segment.Offset < 0 || this.segment.Count <= 0;
			}
		}

		public void AddCount(int offset)
		{
			this.Count += offset;
		}

		public bool Resize(int capacity)
		{
			if (capacity > BufferManager.MaxSize)
			{
				return false;
			}
			if (capacity < this.Count)
			{
				return false;
			}
			if (this.Capacity != capacity)
			{
				this.Capacity = capacity;
				if (capacity > this.segment.Count)
				{
					ArraySegment<byte> arraySegment = this.segment;
					this.segment = BufferManager.Allocate(capacity);
					if (arraySegment.IsValid())
					{
						if (this.Count > 0)
						{
							Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, this.segment.Array, this.segment.Offset, this.Count);
						}
						BufferManager.Free(ref arraySegment);
					}
				}
			}
			return true;
		}

		public void Free()
		{
			this.Count = 0;
			BufferManager.Free(ref this.segment);
		}

		public void Clear()
		{
			this.Count = 0;
		}

		public void Dispose()
		{
			this.Free();
		}

		public bool CopyTransferredFrom(ServerAsyncEventArgs e, int skipBytes)
		{
			return this.CopyFrom(e.Buffer, e.Offset + skipBytes, e.BytesTransferred - skipBytes);
		}

		public bool CopyFrom(ArraySegment<byte> segment1, int skipBytes)
		{
			return this.CopyFrom(segment1.Array, segment1.Offset + skipBytes, segment1.Count - skipBytes);
		}

		public bool CopyFrom(ArraySegment<byte> segmnet)
		{
			return this.CopyFrom(segmnet.Array, segmnet.Offset, segmnet.Count);
		}

		internal bool CopyFrom(SecBufferEx secBuffer)
		{
			return this.CopyFrom(secBuffer.Buffer as byte[], secBuffer.Offset, secBuffer.Size);
		}

		public bool CopyFrom(byte[] array, int offset, int count)
		{
			if (count > this.Capacity - this.Count)
			{
				return false;
			}
			if (count == 0)
			{
				return true;
			}
			this.Create();
			Buffer.BlockCopy(array, offset, this.segment.Array, this.segment.Offset + this.Count, count);
			this.Count += count;
			return true;
		}

		public void MoveToBegin(int offsetOffset)
		{
			this.MoveToBegin(offsetOffset, this.Count - offsetOffset);
		}

		public void MoveToBegin(int offsetOffset, int count)
		{
			Buffer.BlockCopy(this.segment.Array, this.segment.Offset + offsetOffset, this.segment.Array, this.segment.Offset, count);
			this.Count = count;
		}

		public ArraySegment<byte> Detach()
		{
			ArraySegment<byte> result = this.segment;
			this.segment = default(ArraySegment<byte>);
			this.Count = 0;
			return result;
		}

		private void Create()
		{
			if (this.segment.IsInvalid())
			{
				this.Count = 0;
				this.segment = BufferManager.Allocate(this.Capacity);
			}
		}
	}
}
