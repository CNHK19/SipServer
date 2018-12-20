using System;

namespace Microsoft.Win32.Ssp
{
	public struct SecPkgContext_StreamSizes
	{
		public int cbHeader;

		public int cbTrailer;

		public int cbMaximumMessage;

		public int cBuffers;

		public int cbBlockSize;
	}
}
