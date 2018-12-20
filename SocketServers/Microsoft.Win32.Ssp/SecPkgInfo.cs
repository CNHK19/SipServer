using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.Ssp
{
	public struct SecPkgInfo
	{
		public int fCapabilities;

		public short wVersion;

		public short wRPCID;

		public int cbMaxToken;

		private IntPtr Name;

		private IntPtr Comment;

		public string GetName()
		{
			if (!(this.Name != IntPtr.Zero))
			{
				return null;
			}
			return Marshal.PtrToStringAnsi(this.Name);
		}

		public string GetComment()
		{
			if (!(this.Comment != IntPtr.Zero))
			{
				return null;
			}
			return Marshal.PtrToStringAnsi(this.Comment);
		}
	}
}
