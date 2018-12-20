using System;
using System.Net;

namespace Sip.Message
{
	public struct Hostport
	{
		public BeginEndIndex Host;

		public int Port;

		private IPAddress ip;

		public IPAddress IP
		{
			get
			{
				if (this.ip == IPAddress.None)
				{
					this.ip = this.Host.ToIPAddress();
				}
				return this.ip;
			}
			set
			{
				this.ip = value;
				this.Host.SetDefaultValue();
			}
		}

		public void SetDefaultValue(int index)
		{
			this.Host.SetDefaultValue(index);
			this.Port = -2147483648;
			this.OnSetDefaultValue();
		}

		private void OnSetDefaultValue()
		{
			this.ip = IPAddress.None;
		}
	}
}
