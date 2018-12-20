using System;
using System.Threading;

namespace SocketServers
{
	public class LockFreePool<T> : ILockFreePool<T>, IDisposable where T : class, ILockFreePoolItem, IDisposable, new()
	{
		private LockFreeItem<T>[] array;

		private LockFreeStack<T> empty;

		private LockFreeStack<T> full;

		private int created;

		public int Queued
		{
			get
			{
				return this.full.Length;
			}
		}

		public int Created
		{
			get
			{
				return this.created;
			}
		}

		public LockFreePool(int size)
		{
			this.array = new LockFreeItem<T>[size];
			this.full = new LockFreeStack<T>(this.array, -1, -1);
			this.empty = new LockFreeStack<T>(this.array, 0, this.array.Length);
		}

		public void Dispose()
		{
			while (true)
			{
				int num = this.full.Pop();
				if (num < 0)
				{
					break;
				}
				this.array[num].Value.Dispose();
				this.array[num].Value = default(T);
			}
		}

		public T Get()
		{
			T result = default(T);
			int num = this.full.Pop();
			if (num >= 0)
			{
				result = this.array[num].Value;
				this.array[num].Value = default(T);
				this.empty.Push(num);
			}
			else
			{
				result = Activator.CreateInstance<T>();
				result.SetDefaultValue();
				Interlocked.Increment(ref this.created);
			}
			result.IsPooled = false;
			return result;
		}

		public void Put(ref T value)
		{
			this.Put(value);
			value = default(T);
		}

		public void Put(T value)
		{
			value.IsPooled = true;
			int num = this.empty.Pop();
			if (num >= 0)
			{
				value.SetDefaultValue();
				this.array[num].Value = value;
				this.full.Push(num);
				return;
			}
			value.Dispose();
		}
	}
}
