using System;
using System.Runtime.InteropServices;

namespace SocketServers
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct LockFreeQueueVars
	{
		[FieldOffset(64)]
		public long Head;

		[FieldOffset(128)]
		public long Tail;

		[FieldOffset(192)]
		public bool HasDequeuePredicate;

		[FieldOffset(256)]
		public int padding;
	}
}
