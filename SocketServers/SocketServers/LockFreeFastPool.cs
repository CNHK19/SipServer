using System;
using System.Threading;

namespace SocketServers
{
	public class LockFreeFastPool<T> : ILockFreePool<T>, IDisposable where T : class, ILockFreePoolItem, ILockFreePoolItemIndex, IDisposable, new()
	{
		private LockFreeItem<T>[] array;

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

		public LockFreeFastPool(int size)
		{
			this.array = new LockFreeItem<T>[size];
			this.full = new LockFreeStack<T>(this.array, -1, -1);
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
			}
			else
			{
				result = Activator.CreateInstance<T>();
				result.SetDefaultValue();
				result.Index = -1;
				if (this.created < this.array.Length)
				{
					int num2 = Interlocked.Increment(ref this.created) - 1;
					if (num2 < this.array.Length)
					{
						result.Index = num2;
					}
				}
			}
			result.IsPooled = false;
			return result;
		}

		public T GetIfSpaceAvailable()
		{
			T result = default(T);
			int num = this.full.Pop();
			if (num >= 0)
			{
				result = this.array[num].Value;
				this.array[num].Value = default(T);
			}
			else
			{
				if (this.created >= this.array.Length)
				{
					return default(T);
				}
				int num2 = Interlocked.Increment(ref this.created) - 1;
				if (num2 >= this.array.Length)
				{
					return default(T);
				}
				result = Activator.CreateInstance<T>();
				result.SetDefaultValue();
				result.Index = num2;
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
			int index = value.Index;
			if (index >= 0)
			{
				value.SetDefaultValue();
				this.array[index].Value = value;
				this.full.Push(index);
				return;
			}
			value.Dispose();
		}
	}
}
