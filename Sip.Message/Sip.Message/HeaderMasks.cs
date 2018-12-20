using System;

namespace Sip.Message
{
	public class HeaderMasks
	{
		public const ulong Extension = 1uL;

		public const ulong ContentType = 2uL;

		public const ulong ContentEncoding = 4uL;

		public const ulong From = 8uL;

		public const ulong CallId = 16uL;

		public const ulong Supported = 32uL;

		public const ulong ContentLength = 64uL;

		public const ulong Contact = 128uL;

		public const ulong Event = 256uL;

		public const ulong Subject = 512uL;

		public const ulong To = 1024uL;

		public const ulong AllowEvents = 2048uL;

		public const ulong Via = 4096uL;

		public const ulong CSeq = 8192uL;

		public const ulong Date = 16384uL;

		public const ulong Allow = 32768uL;

		public const ulong Route = 65536uL;

		public const ulong Accept = 131072uL;

		public const ulong Server = 262144uL;

		public const ulong Require = 524288uL;

		public const ulong Warning = 1048576uL;

		public const ulong Priority = 2097152uL;

		public const ulong ReplyTo = 4194304uL;

		public const ulong CallInfo = 8388608uL;

		public const ulong Timestamp = 16777216uL;

		public const ulong AlertInfo = 33554432uL;

		public const ulong ErrorInfo = 67108864uL;

		public const ulong UserAgent = 134217728uL;

		public const ulong InReplyTo = 268435456uL;

		public const ulong MinExpires = 536870912uL;

		public const ulong RetryAfter = 1073741824uL;

		public const ulong Unsupported = 2147483648uL;

		public const ulong MaxForwards = 4294967296uL;

		public const ulong MimeVersion = 8589934592uL;

		public const ulong Organization = 17179869184uL;

		public const ulong RecordRoute = 34359738368uL;

		public const ulong Authorization = 68719476736uL;

		public const ulong ProxyRequire = 137438953472uL;

		public const ulong AcceptEncoding = 274877906944uL;

		public const ulong AcceptLanguage = 549755813888uL;

		public const ulong ContentLanguage = 1099511627776uL;

		public const ulong WwwAuthenticate = 2199023255552uL;

		public const ulong ProxyAuthenticate = 4398046511104uL;

		public const ulong SubscriptionState = 8796093022208uL;

		public const ulong AuthenticationInfo = 17592186044416uL;

		public const ulong ContentDisposition = 35184372088832uL;

		public const ulong ProxyAuthorization = 70368744177664uL;

		public const ulong ProxyAuthenticationInfo = 140737488355328uL;

		public const ulong SipEtag = 281474976710656uL;

		public const ulong SipIfMatch = 562949953421312uL;

		public const ulong Expires = 1125899906842624uL;
	}
}
