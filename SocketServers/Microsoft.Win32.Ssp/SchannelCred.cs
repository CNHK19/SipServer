using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Win32.Ssp
{
	public struct SchannelCred
	{
		[Flags]
		public enum Flags
		{
			NoDefaultCred = 16,
			NoNameCheck = 4,
			NoSystemMapper = 2,
			ValidateAuto = 32,
			ValidateManual = 8,
			Zero = 0
		}

		public const int CurrentVersion = 4;

		public int version;

		public int cCreds;

		public IntPtr paCreds1;

		private readonly IntPtr rootStore;

		public int cMappers;

		private readonly IntPtr phMappers;

		public int cSupportedAlgs;

		private readonly IntPtr palgSupportedAlgs;

		public SchProtocols grbitEnabledProtocols;

		public int dwMinimumCipherStrength;

		public int dwMaximumCipherStrength;

		public int dwSessionLifespan;

		public SchannelCred.Flags dwFlags;

		public int reserved;

		public SchannelCred(X509Certificate certificate, SchProtocols protocols)
		{
			this = new SchannelCred(4, certificate, SchannelCred.Flags.Zero, protocols);
		}

		public SchannelCred(int version1, X509Certificate certificate, SchannelCred.Flags flags, SchProtocols protocols)
		{
			this.paCreds1 = IntPtr.Zero;
			this.rootStore = (this.phMappers = (this.palgSupportedAlgs = IntPtr.Zero));
			this.cCreds = (this.cMappers = (this.cSupportedAlgs = 0));
			this.dwMinimumCipherStrength = (this.dwMaximumCipherStrength = 0);
			this.dwSessionLifespan = (this.reserved = 0);
			this.version = version1;
			this.dwFlags = flags;
			this.grbitEnabledProtocols = protocols;
			if (certificate != null)
			{
				this.paCreds1 = certificate.Handle;
				this.cCreds = 1;
			}
		}
	}
}
