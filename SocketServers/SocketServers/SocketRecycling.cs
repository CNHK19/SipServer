using System;
using System.Net.Sockets;

namespace SocketServers
{
	public class SocketRecycling : IDisposable
	{
		private LockFreeItem<Socket>[] array;

		private LockFreeStack<Socket> empty;

		private LockFreeStack<Socket> full4;

		private LockFreeStack<Socket> full6;

		private bool isEnabled;

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
		}

		public int RecyclingCount
		{
			get
			{
				if (!this.isEnabled)
				{
					return 0;
				}
				return this.full4.Length + this.full6.Length;
			}
		}

		public SocketRecycling(int maxSocket)
		{
			if (maxSocket > 0)
			{
				this.isEnabled = true;
				this.array = new LockFreeItem<Socket>[maxSocket];
				this.empty = new LockFreeStack<Socket>(this.array, 0, maxSocket);
				this.full4 = new LockFreeStack<Socket>(this.array, -1, -1);
				this.full6 = new LockFreeStack<Socket>(this.array, -1, -1);
			}
		}

		public void Dispose()
		{
			if (this.isEnabled)
			{
				this.isEnabled = false;
				int num;
				while ((num = this.full4.Pop()) >= 0)
				{
					this.array[num].Value.Close();
					this.empty.Push(num);
				}
				while ((num = this.full6.Pop()) >= 0)
				{
					this.array[num].Value.Close();
					this.empty.Push(num);
				}
			}
		}

		public Socket Get(AddressFamily family)
		{
			if (this.isEnabled)
			{
				int num = this.GetFull(family).Pop();
				if (num >= 0)
				{
					Socket value = this.array[num].Value;
					this.array[num].Value = null;
					this.empty.Push(num);
					return value;
				}
			}
			return null;
		}

		public bool Recycle(Socket socket, AddressFamily family)
		{
			if (this.isEnabled)
			{
				int num = this.empty.Pop();
				if (num >= 0)
				{
					this.array[num].Value = socket;
					this.GetFull(family).Push(num);
					return true;
				}
			}
			return false;
		}

		private LockFreeStack<Socket> GetFull(AddressFamily family)
		{
			if (family == AddressFamily.InterNetwork)
			{
				return this.full4;
			}
			if (family == AddressFamily.InterNetworkV6)
			{
				return this.full6;
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
