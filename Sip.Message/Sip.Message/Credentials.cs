using System;

namespace Sip.Message
{
	public struct Credentials
	{
		public BeginEndIndex NonceCountBytes;

		public BeginEndIndex MessageQop;

		public BeginEndIndex DigestUri;

		public BeginEndIndex Realm;

		public BeginEndIndex Opaque;

		public BeginEndIndex Nonce;

		public BeginEndIndex Cnonce;

		public BeginEndIndex Response;

		public BeginEndIndex Username;

		public BeginEndIndex Targetname;

		public BeginEndIndex GssapiData;

		public int NonceCount;

		public int Cnum;

		public int Crand;

		public int Version;

		public bool HasResponse;

		public bool HasGssapiData;

		public AuthSchemes AuthScheme;

		public AuthAlgorithms AuthAlgorithm;

		public void SetDefaultValue(int index)
		{
			this.AuthScheme = AuthSchemes.None;
			this.AuthAlgorithm = AuthAlgorithms.None;
			this.NonceCountBytes.SetDefaultValue(index);
			this.MessageQop.SetDefaultValue(index);
			this.DigestUri.SetDefaultValue(index);
			this.Realm.SetDefaultValue(index);
			this.Opaque.SetDefaultValue(index);
			this.Nonce.SetDefaultValue(index);
			this.Cnonce.SetDefaultValue(index);
			this.Response.SetDefaultValue(index);
			this.Username.SetDefaultValue(index);
			this.Targetname.SetDefaultValue(index);
			this.GssapiData.SetDefaultValue(index);
			this.NonceCount = -2147483648;
			this.Cnum = -2147483648;
			this.Crand = -2147483648;
			this.Version = -2147483648;
			this.HasResponse = false;
			this.HasGssapiData = false;
		}
	}
}
