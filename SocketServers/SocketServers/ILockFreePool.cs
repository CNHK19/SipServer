using System;

namespace SocketServers
{
	public interface ILockFreePool<T> : IDisposable
	{
		int Queued
		{
			get;
		}

		int Created
		{
			get;
		}

		T Get();

		void Put(ref T value);

		void Put(T value);
	}
}
