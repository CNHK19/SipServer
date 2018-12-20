using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.Ssp
{
	[SuppressUnmanagedCodeSecurity]
	public class SafeCtxtHandle : SafeHandle
	{
		internal CtxtHandle Handle;

		public override bool IsInvalid
		{
			get
			{
				return this.Handle.IsInvalid;
			}
		}

		public SafeCtxtHandle() : base(IntPtr.Zero, true)
		{
		}

		public SafeCtxtHandle(CtxtHandle ctxtHandle) : base(IntPtr.Zero, true)
		{
			this.Handle = ctxtHandle;
		}

		protected override bool ReleaseHandle()
		{
			return Secur32Dll.DeleteSecurityContext(ref this.Handle) == 0;
		}
	}
}
