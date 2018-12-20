using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.Ssp
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SecHandle
	{
		private IntPtr dwLower;

		private IntPtr dwUpper;

		public bool IsInvalid
		{
			get
			{
				return this.dwLower == IntPtr.Zero && this.dwUpper == IntPtr.Zero;
			}
		}
	}
}
