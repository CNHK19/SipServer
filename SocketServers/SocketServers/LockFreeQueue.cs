using System;
using System.Threading;

namespace SocketServers
{
	internal class LockFreeQueue<T>
	{
		private LockFreeItem<T>[] array;

		private LockFreeQueueVars q;

		private Predicate<T> dequeuePredicate;

		public Predicate<T> DequeuePredicate
		{
			get
			{
				return this.dequeuePredicate;
			}
			set
			{
				this.dequeuePredicate = value;
				this.q.HasDequeuePredicate = (this.dequeuePredicate != null);
			}
		}

		public LockFreeQueue(LockFreeItem<T>[] array1, int enqueueFromDummy, int enqueueCount)
		{
			if (enqueueCount <= 0)
			{
				throw new ArgumentOutOfRangeException("enqueueCount", "Queue must include at least one dummy element");
			}
			this.array = array1;
			this.q.Head = (long)enqueueFromDummy;
			this.q.Tail = (long)(enqueueFromDummy + enqueueCount - 1);
			for (int i = 0; i < enqueueCount - 1; i++)
			{
				this.array[i + enqueueFromDummy].Next = (long)(enqueueFromDummy + i + 1);
			}
			this.array[(int)(checked((IntPtr)this.q.Tail))].Next = (long)((ulong)-1);
		}

		public void Enqueue(int index)
		{
			LockFreeItem<T>[] expr_0C_cp_0 = this.array;
			expr_0C_cp_0[index].Next = (expr_0C_cp_0[index].Next | (long)((ulong)-1));
			ulong num;
			ulong value;
			while (true)
			{
				num = (ulong)Interlocked.Read(ref this.q.Tail);
				ulong num2 = (ulong)Interlocked.Read(ref this.array[(int)(checked((IntPtr)(num & unchecked((ulong)-1))))].Next);
				if (num == (ulong)this.q.Tail)
				{
					if ((num2 & (ulong)-1) == (ulong)-1)
					{
						value = ((num2 + 4294967296uL & 18446744069414584320uL) | (ulong)index);
						ulong num3 = (ulong)Interlocked.CompareExchange(ref this.array[(int)(checked((IntPtr)(num & unchecked((ulong)-1))))].Next, (long)value, (long)num2);
						if (num3 == num2)
						{
							break;
						}
					}
					else
					{
						value = ((num + 4294967296uL & 18446744069414584320uL) | (num2 & (ulong)-1));
						Interlocked.CompareExchange(ref this.q.Tail, (long)value, (long)num);
					}
				}
			}
			value = ((num + 4294967296uL & 18446744069414584320uL) | (ulong)index);
			Interlocked.CompareExchange(ref this.q.Tail, (long)value, (long)num);
		}

		public int Dequeue()
		{
			ulong num;
			T value2;
			while (true)
			{
				num = (ulong)Interlocked.Read(ref this.q.Head);
				ulong num2 = (ulong)Interlocked.Read(ref this.q.Tail);
				ulong num3 = (ulong)Interlocked.Read(ref this.array[(int)(checked((IntPtr)(num & unchecked((ulong)-1))))].Next);
				if (num == (ulong)this.q.Head)
				{
					if ((num & (ulong)-1) == (num2 & (ulong)-1))
					{
						if ((num3 & (ulong)-1) == (ulong)-1)
						{
							break;
						}
						ulong value = (num2 + 4294967296uL & 18446744069414584320uL) | (num3 & (ulong)-1);
						Interlocked.CompareExchange(ref this.q.Tail, (long)value, (long)num2);
					}
					else
					{
						value2 = this.array[(int)(checked((IntPtr)(num3 & unchecked((ulong)-1))))].Value;
						if (this.q.HasDequeuePredicate && !this.DequeuePredicate(value2))
						{
							return -1;
						}
						ulong value = (num + 4294967296uL & 18446744069414584320uL) | (num3 & (ulong)-1);
						ulong num4 = (ulong)Interlocked.CompareExchange(ref this.q.Head, (long)value, (long)num);
						if (num4 == num)
						{
							goto Block_5;
						}
					}
				}
			}
			return -1;
			Block_5:
			int num5 = (int)(num & (ulong)-1);
			this.array[num5].Value = value2;
			return num5;
		}
	}
}
