using System;

namespace Sip.Message
{
	public struct Fromto
	{
		public BeginEndIndex Tag;

		public BeginEndIndex Epid;

		public Addrspec AddrSpec2;

		public Addrspec AddrSpec1;

		public Addrspec AddrSpec
		{
			get
			{
				if (this.AddrSpec1.Hostport.Host.IsValid)
				{
					return this.AddrSpec1;
				}
				return this.AddrSpec2;
			}
		}

		public void SetDefaultValue(int index)
		{
			this.AddrSpec2.SetDefaultValue(index);
			this.AddrSpec1.SetDefaultValue(index);
			this.Tag.SetDefaultValue(index);
			this.Epid.SetDefaultValue(index);
		}
	}
}
