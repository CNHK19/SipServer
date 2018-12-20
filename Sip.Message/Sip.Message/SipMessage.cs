using Base.Message;
using System;
using System.Text;

namespace Sip.Message
{
	public static class SipMessage
	{
		public static readonly byte[] MagicCookie;

		public static IBufferManager BufferManager
		{
			get;
			set;
		}

		static SipMessage()
		{
			SipMessage.MagicCookie = Encoding.UTF8.GetBytes("z9hG4bK");
			SipMessage.BufferManager = new BufferManager();
		}
	}
}
