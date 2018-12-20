using System;
using System.Threading;

namespace SocketServers
{
	public class SmartBufferPool
	{
		public const long Kb = 1024L;

		public const long Mb = 1048576L;

		public const long Gb = 1073741824L;

		public const int MinSize = 1024;

		public const int MaxSize = 262144;

		public readonly long MaxMemoryUsage;

		public readonly long InitialMemoryUsage;

		public readonly long ExtraMemoryUsage;

		public readonly long MaxBuffersCount;

		private byte[][] buffers;

		private long indexOffset;

		private LockFreeItem<long>[] array;

		private LockFreeStack<long> empty;

		private LockFreeStack<long>[] ready;

		public SmartBufferPool(int maxMemoryUsageMb, int initialSizeMb, int extraBufferSizeMb)
		{
			this.InitialMemoryUsage = (long)initialSizeMb * 1048576L;
			this.ExtraMemoryUsage = (long)extraBufferSizeMb * 1048576L;
			this.MaxBuffersCount = ((long)maxMemoryUsageMb * 1048576L - this.InitialMemoryUsage) / this.ExtraMemoryUsage;
			this.MaxMemoryUsage = this.InitialMemoryUsage + this.ExtraMemoryUsage * this.MaxBuffersCount;
			this.array = new LockFreeItem<long>[this.MaxMemoryUsage / 1024L];
			this.empty = new LockFreeStack<long>(this.array, 0, this.array.Length);
			int num = 0;
			while (262144 >> num >= 1024)
			{
				num++;
			}
			this.ready = new LockFreeStack<long>[num];
			for (int i = 0; i < this.ready.Length; i++)
			{
				this.ready[i] = new LockFreeStack<long>(this.array, -1, -1);
			}
			this.buffers = new byte[this.MaxBuffersCount][];
			this.buffers[0] = SmartBufferPool.NewBuffer(this.InitialMemoryUsage);
		}

		public ArraySegment<byte> Allocate(int size)
		{
			if (size > 262144)
			{
				throw new ArgumentOutOfRangeException("Too large size");
			}
			size = 1024 << this.GetBitOffset(size);
			int num;
			int num2;
			if (!this.GetAllocated(size, out num, out num2))
			{
				while (true)
				{
					long num3 = Interlocked.Read(ref this.indexOffset);
					num2 = (int)num3;
					num = (int)(num3 >> 32);
					while (this.buffers[num] == null)
					{
						Thread.Sleep(0);
					}
					if (this.buffers[num].Length - num2 < size)
					{
						if (num + 1 >= this.buffers.Length)
						{
							break;
						}
						if (Interlocked.CompareExchange(ref this.indexOffset, (long)(num + 1) << 32, num3) == num3)
						{
							this.buffers[num + 1] = SmartBufferPool.NewBuffer(this.ExtraMemoryUsage);
						}
					}
					if (Interlocked.CompareExchange(ref this.indexOffset, num3 + (long)size, num3) == num3)
					{
						goto IL_C4;
					}
				}
				throw new OutOfMemoryException("Source: BufferManager");
			}
			IL_C4:
			return new ArraySegment<byte>(this.buffers[num], num2, size);
		}

		public void Free(ArraySegment<byte> segment)
		{
			int num = 0;
			while (num < this.buffers.Length && this.buffers[num] != segment.Array)
			{
				num++;
			}
			if (num >= this.buffers.Length)
			{
				throw new ArgumentException("SmartBufferPool.Free, segment.Array is invalid");
			}
			int num2 = this.empty.Pop();
			this.array[num2].Value = ((long)num << 32) + (long)segment.Offset;
			this.ready[this.GetBitOffset(segment.Count)].Push(num2);
		}

		private bool GetAllocated(int size, out int index, out int offset)
		{
			int num = this.ready[this.GetBitOffset(size)].Pop();
			if (num >= 0)
			{
				index = (int)(this.array[num].Value >> 32);
				offset = (int)this.array[num].Value;
				this.empty.Push(num);
				return true;
			}
			index = -1;
			offset = -1;
			return false;
		}

		private int GetBitOffset(int size)
		{
			int num = 0;
			while (size >> num > 1024)
			{
				num++;
			}
			return num;
		}

		private static byte[] NewBuffer(long size)
		{
			return new byte[size];
		}
	}
}
