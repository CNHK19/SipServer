using System;

namespace Sip.Message
{
	internal static class HeaderMasksHelper
	{
		public static bool IsMasked(this HeaderNames name, ulong mask)
		{
			return (mask & name.ToMask()) > 0uL;
		}

		public static ulong ToMask(this HeaderNames name)
		{
			switch (name)
			{
			case HeaderNames.None:
				return 0uL;
			case HeaderNames.Extension:
				return 1uL;
			case HeaderNames.ContentType:
				return 2uL;
			case HeaderNames.ContentEncoding:
				return 4uL;
			case HeaderNames.From:
				return 8uL;
			case HeaderNames.CallId:
				return 16uL;
			case HeaderNames.Supported:
				return 32uL;
			case HeaderNames.ContentLength:
				return 64uL;
			case HeaderNames.Contact:
				return 128uL;
			case HeaderNames.Event:
				return 256uL;
			case HeaderNames.Subject:
				return 512uL;
			case HeaderNames.To:
				return 1024uL;
			case HeaderNames.AllowEvents:
				return 2048uL;
			case HeaderNames.Via:
				return 4096uL;
			case HeaderNames.CSeq:
				return 8192uL;
			case HeaderNames.Date:
				return 16384uL;
			case HeaderNames.Allow:
				return 32768uL;
			case HeaderNames.Route:
				return 65536uL;
			case HeaderNames.Accept:
				return 131072uL;
			case HeaderNames.Server:
				return 262144uL;
			case HeaderNames.Expires:
				return 1125899906842624uL;
			case HeaderNames.Require:
				return 524288uL;
			case HeaderNames.Warning:
				return 1048576uL;
			case HeaderNames.Priority:
				return 2097152uL;
			case HeaderNames.ReplyTo:
				return 4194304uL;
			case HeaderNames.SipEtag:
				return 281474976710656uL;
			case HeaderNames.CallInfo:
				return 8388608uL;
			case HeaderNames.Timestamp:
				return 16777216uL;
			case HeaderNames.AlertInfo:
				return 33554432uL;
			case HeaderNames.ErrorInfo:
				return 67108864uL;
			case HeaderNames.UserAgent:
				return 134217728uL;
			case HeaderNames.InReplyTo:
				return 268435456uL;
			case HeaderNames.MinExpires:
				return 536870912uL;
			case HeaderNames.RetryAfter:
				return 1073741824uL;
			case HeaderNames.Unsupported:
				return (ulong)-2147483648;
			case HeaderNames.MaxForwards:
				return 4294967296uL;
			case HeaderNames.MimeVersion:
				return 8589934592uL;
			case HeaderNames.Organization:
				return 17179869184uL;
			case HeaderNames.RecordRoute:
				return 34359738368uL;
			case HeaderNames.SipIfMatch:
				return 562949953421312uL;
			case HeaderNames.Authorization:
				return 68719476736uL;
			case HeaderNames.ProxyRequire:
				return 137438953472uL;
			case HeaderNames.AcceptEncoding:
				return 274877906944uL;
			case HeaderNames.AcceptLanguage:
				return 549755813888uL;
			case HeaderNames.ContentLanguage:
				return 1099511627776uL;
			case HeaderNames.WwwAuthenticate:
				return 2199023255552uL;
			case HeaderNames.ProxyAuthenticate:
				return 4398046511104uL;
			case HeaderNames.SubscriptionState:
				return 8796093022208uL;
			case HeaderNames.AuthenticationInfo:
				return 17592186044416uL;
			case HeaderNames.ContentDisposition:
				return 35184372088832uL;
			case HeaderNames.ProxyAuthorization:
				return 70368744177664uL;
			case HeaderNames.ProxyAuthenticationInfo:
				return 140737488355328uL;
			default:
				throw new ArgumentOutOfRangeException(name.ToString());
			}
		}
	}
}
