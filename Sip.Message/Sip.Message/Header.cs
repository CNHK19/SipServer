using System;

namespace Sip.Message
{
	public struct Header
	{
		public BeginEndIndex Name;

		public BeginEndIndex Value;

		public HeaderNames HeaderName;

		public void SetDefaultValue(int index)
		{
			this.HeaderName = HeaderNames.None;
			this.Name.SetDefaultValue(index);
			this.Value.SetDefaultValue(index);
		}
	}
}
