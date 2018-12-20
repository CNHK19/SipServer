using System;

namespace SocketServers
{
	public class BufferManager
	{
		private static SmartBufferPool pool;

		public static long MaxMemoryUsage
		{
			get
			{
				return BufferManager.pool.MaxMemoryUsage;
			}
		}

		public static int MaxSize
		{
			get
			{
				return 262144;
			}
		}

		public static void Initialize(int maxMemoryUsageMb, int initialSizeMb, int extraBufferSizeMb)
		{
			BufferManager.pool = new SmartBufferPool(maxMemoryUsageMb, initialSizeMb, extraBufferSizeMb);
		}

		public static void Initialize(int maxMemoryUsageMb)
		{
			BufferManager.pool = new SmartBufferPool(maxMemoryUsageMb, maxMemoryUsageMb / 8, maxMemoryUsageMb / 16);
		}

		public static bool IsInitialized()
		{
			return BufferManager.pool != null;
		}

		public static ArraySegment<byte> Allocate(int size)
		{
			return BufferManager.pool.Allocate(size);
		}

		public static void Free(ref ArraySegment<byte> segment)
		{
			if (segment.IsValid())
			{
				BufferManager.pool.Free(segment);
				segment = default(ArraySegment<byte>);
			}
		}

		internal static void Free(ArraySegment<byte> segment)
		{
			if (segment.IsValid())
			{
				BufferManager.pool.Free(segment);
			}
		}
	}
}
