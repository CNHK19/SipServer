using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.Ssp
{
	[SuppressUnmanagedCodeSecurity]
	public class SafeContextBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeContextBufferHandle() : base(true)
		{
		}

		public SafeContextBufferHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return Secur32Dll.FreeContextBuffer(this.handle) == 0;
		}

		public SecPkgInfo GetItem<T>(int index)
		{
			IntPtr ptr = (IntPtr)(base.DangerousGetHandle().ToInt64() + (long)(Marshal.SizeOf(typeof(T)) * index));
			return (SecPkgInfo)Marshal.PtrToStructure(ptr, typeof(T));
		}
	}
}
