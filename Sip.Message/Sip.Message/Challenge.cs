using System;

namespace Sip.Message
{
	public struct Challenge
	{
		public BeginEndIndex Qop;

		public BeginEndIndex Nonce;

		public BeginEndIndex Realm;

		public BeginEndIndex Opaque;

		public bool Stale;

		public AuthSchemes AuthScheme;

		public AuthAlgorithms AuthAlgorithm;

		public void SetDefaultValue(int index)
		{
			this.AuthScheme = AuthSchemes.None;
			this.AuthAlgorithm = AuthAlgorithms.None;
			this.Qop.SetDefaultValue(index);
			this.Nonce.SetDefaultValue(index);
			this.Realm.SetDefaultValue(index);
			this.Opaque.SetDefaultValue(index);
			this.Stale = false;
		}
	}
}
