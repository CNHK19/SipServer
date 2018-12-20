using System;

namespace SocketServers
{
	public class EventArgsManager
	{
		private static ILockFreePool<ServerAsyncEventArgs> pool;

		public static int Queued
		{
			get
			{
				return EventArgsManager.pool.Queued;
			}
		}

		public static int Created
		{
			get
			{
				return EventArgsManager.pool.Created;
			}
		}

		internal static void Initialize()
		{
			EventArgsManager.pool = new LockFreePool<ServerAsyncEventArgs>((int)(BufferManager.MaxMemoryUsage / 2048L));
		}

		internal static bool IsInitialized()
		{
			return EventArgsManager.pool != null;
		}

		public static ServerAsyncEventArgs Get()
		{
			return EventArgsManager.pool.Get();
		}

		public static void Put(ref ServerAsyncEventArgs value)
		{
			EventArgsManager.pool.Put(ref value);
		}

		public static void Put(ServerAsyncEventArgs value)
		{
			EventArgsManager.pool.Put(value);
		}
	}
}
