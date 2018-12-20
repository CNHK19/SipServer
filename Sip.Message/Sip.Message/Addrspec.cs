using System;

namespace Sip.Message
{
	public struct Addrspec
	{
		public BeginEndIndex Value;

		public BeginEndIndex User;

		public BeginEndIndex Maddr;

		public BeginEndIndex MsReceivedCid;

		public bool HasLr;

		public UriSchemes UriScheme;

		public Transports Transport;

		public Hostport Hostport;

		public bool IsValid
		{
			get
			{
				return this.Hostport.Host.IsValid;
			}
		}

		public void SetDefaultValue(int index)
		{
			this.Hostport.SetDefaultValue(index);
			this.UriScheme = UriSchemes.None;
			this.Transport = Transports.None;
			this.Value.SetDefaultValue(index);
			this.User.SetDefaultValue(index);
			this.Maddr.SetDefaultValue(index);
			this.MsReceivedCid.SetDefaultValue(index);
			this.HasLr = false;
		}
	}
}
