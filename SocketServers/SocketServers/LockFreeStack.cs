using System;
using System.Threading;

namespace SocketServers
{
	internal class LockFreeStack<T>
	{
		private LockFreeStackVars s;

		private LockFreeItem<T>[] array;

		public int Length
		{
			get
			{
				int num = 0;
				for (int i = (int)this.s.Head; i >= 0; i = (int)this.array[i].Next)
				{
					num++;
				}
				return num;
			}
		}

		public LockFreeStack(LockFreeItem<T>[] array1, int pushFrom, int pushCount)
		{
			this.array = array1;
			this.s.Head = (long)pushFrom;
			for (int i = 0; i < pushCount - 1; i++)
			{
				this.array[i + pushFrom].Next = (long)(pushFrom + i + 1);
			}
			if (pushFrom >= 0)
			{
				this.array[pushFrom + pushCount - 1].Next = (long)((ulong)-1);
			}
		}

		public int Pop()
		{
			ulong num = (ulong)Interlocked.Read(ref this.s.Head);
			while (true)
			{
				int num2 = (int)num;
				if (num2 < 0)
				{
					break;
				}
				ulong value = (ulong)((Thread.VolatileRead(ref this.array[num2].Next) & (long)((ulong)-1)) | (long)(num & 18446744069414584320uL));
				ulong num3 = (ulong)Interlocked.CompareExchange(ref this.s.Head, (long)value, (long)num);
				if (num == num3)
				{
					return num2;
				}
				num = num3;
			}
			return -1;
		}

		public void Push(int index)
		{
			ulong num = (ulong)Interlocked.Read(ref this.s.Head);
			while (true)
			{
				this.array[index].Next = ((this.array[index].Next & -4294967296L) | (long)(num & (ulong)-1));
				ulong value = (num + 4294967296uL & 18446744069414584320uL) | (ulong)index;
				ulong num2 = (ulong)Interlocked.CompareExchange(ref this.s.Head, (long)value, (long)num);
				if (num == num2)
				{
					break;
				}
				num = num2;
			}
		}
	}
}
