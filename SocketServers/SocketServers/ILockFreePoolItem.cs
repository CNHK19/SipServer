using System;

namespace SocketServers
{
	public interface ILockFreePoolItem
	{
		bool IsPooled
		{
			set;
		}

		void SetDefaultValue();
	}
}
