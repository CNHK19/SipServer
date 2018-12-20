using System;
using System.ComponentModel;

namespace Microsoft.Win32.Ssp
{
	public class SspiException : Win32Exception
	{
		public SecurityStatus SecurityStatus
		{
			get
			{
				return Sspi.Convert(base.ErrorCode);
			}
		}

		public SspiException(int error, string function) : base(error, string.Format("SSPI error, function call {0} return 0x{1:x8}", function, error))
		{
		}
	}
}
