using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.Ssp
{
	[SuppressUnmanagedCodeSecurity]
	public class SafeCredHandle : SafeHandle
	{
		internal CredHandle Handle;

		public override bool IsInvalid
		{
			get
			{
				return this.Handle.IsInvalid;
			}
		}

		public SafeCredHandle(CredHandle credHandle) : base(IntPtr.Zero, true)
		{
			this.Handle = credHandle;
		}

		protected override bool ReleaseHandle()
		{
			return Secur32Dll.FreeCredentialsHandle(ref this.Handle) == 0;
		}
	}
}
