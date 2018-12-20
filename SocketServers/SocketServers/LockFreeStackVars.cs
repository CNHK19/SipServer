using System;
using System.Runtime.InteropServices;

namespace SocketServers
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct LockFreeStackVars
	{
		[FieldOffset(0)]
		public long Head;

		[FieldOffset(64)]
		public int padding;
	}
}
