using System;

namespace Microsoft.Win32.Ssp
{
	public struct SecBufferDesc
	{
		internal int ulVersion;

		internal int cBuffers;

		internal IntPtr pBuffers;
	}
}
