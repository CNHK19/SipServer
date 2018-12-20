using Base.Message;
using System;
using System.Text;

namespace Sip.Message
{
	public static class Converters
	{
		private static readonly byte[][] transportParams;

		private static readonly byte[][] transports;

		private static readonly byte[][] authSchemes;

		private static readonly byte[][] headerNames;

		static Converters()
		{
			Converters.transports = Converters.InitializeTransports();
			Converters.transportParams = Converters.InitializeTransportParams();
			Converters.authSchemes = Converters.InitializeAuthSchemes();
			Converters.headerNames = Converters.InitializeHeaderNames();
			Converters.Verify(Converters.transports, typeof(Transports));
			Converters.Verify(Converters.transportParams, typeof(Transports));
			Converters.Verify(Converters.authSchemes, typeof(AuthSchemes));
			Converters.Verify(Converters.headerNames, typeof(HeaderNames));
		}

		public static ByteArrayPart ToByteArrayPart(this Methods method)
		{
			switch (method)
			{
			case Methods.None:
			case Methods.Extension:
				throw new ArgumentException();
			case Methods.Ackm:
				return SipMessageWriter.C.ACK;
			case Methods.Byem:
				return SipMessageWriter.C.BYE;
			case Methods.Infom:
				return SipMessageWriter.C.INFO;
			case Methods.Referm:
				return SipMessageWriter.C.REFER;
			case Methods.Cancelm:
				return SipMessageWriter.C.CANCEL;
			case Methods.Invitem:
				return SipMessageWriter.C.INVITE;
			case Methods.Notifym:
				return SipMessageWriter.C.NOTIFY;
			case Methods.Messagem:
				return SipMessageWriter.C.MESSAGE;
			case Methods.Optionsm:
				return SipMessageWriter.C.OPTIONS;
			case Methods.Publishm:
				return SipMessageWriter.C.PUBLISH;
			case Methods.Servicem:
				return SipMessageWriter.C.SERVICE;
			case Methods.Benotifym:
				return SipMessageWriter.C.BENOTIFY;
			case Methods.Registerm:
				return SipMessageWriter.C.REGISTER;
			case Methods.Subscribem:
				return SipMessageWriter.C.SUBSCRIBE;
			default:
				throw new NotImplementedException(method.ToString());
			}
		}

		public static ByteArrayPart ToByteArrayPart(this UriSchemes scheme)
		{
			switch (scheme)
			{
			case UriSchemes.None:
			case UriSchemes.Absolute:
				throw new ArgumentException();
			case UriSchemes.Sip:
				return SipMessageWriter.C.sip;
			case UriSchemes.Sips:
				return SipMessageWriter.C.sips;
			default:
				throw new NotImplementedException();
			}
		}

		public static ByteArrayPart ToByteArrayPart(this AuthQops qop)
		{
			switch (qop)
			{
			case AuthQops.Auth:
				return SipMessageWriter.C.auth;
			case AuthQops.AuthInt:
				return SipMessageWriter.C.auth_int;
			default:
				throw new NotImplementedException();
			}
		}

		public static ByteArrayPart ToByteArrayPart(this AuthAlgorithms algorithm)
		{
			switch (algorithm)
			{
			case AuthAlgorithms.None:
			case AuthAlgorithms.Other:
				throw new ArgumentException();
			case AuthAlgorithms.Md5:
				return SipMessageWriter.C.MD5;
			case AuthAlgorithms.Md5Sess:
				return SipMessageWriter.C.MD5_sess;
			default:
				throw new NotImplementedException();
			}
		}

		public static ByteArrayPart ToByteArrayPart(this string text)
		{
			return new ByteArrayPart(text);
		}

		public static UriSchemes ToScheme(this Transports transport)
		{
			switch (transport)
			{
			case Transports.Tcp:
			case Transports.Udp:
			case Transports.Sctp:
				return UriSchemes.Sip;
			case Transports.Tls:
				return UriSchemes.Sips;
			default:
				throw new ArgumentException("Can not convert Transports to UriSchemes: " + transport.ToString());
			}
		}

		public static byte[] ToUtf8Bytes(this Transports transport)
		{
			return Converters.transports[(int)transport];
		}

		public static byte[] ToTransportParamUtf8Bytes(this Transports transport)
		{
			return Converters.transportParams[(int)transport];
		}

		public static byte[] ToUtf8Bytes(this AuthSchemes schemes)
		{
			return Converters.authSchemes[(int)schemes];
		}

		public static byte[] ToUtf8Bytes(this HeaderNames headerName)
		{
			return Converters.headerNames[(int)headerName];
		}

		private static byte[][] InitializeTransports()
		{
			byte[][] array = new byte[Enum.GetValues(typeof(Transports)).Length][];
			array[0] = new byte[0];
			array[1] = new byte[0];
			array[4] = Encoding.UTF8.GetBytes("TCP");
			array[5] = Encoding.UTF8.GetBytes("TLS");
			array[6] = Encoding.UTF8.GetBytes("UDP");
			array[2] = Encoding.UTF8.GetBytes("WS");
			array[3] = Encoding.UTF8.GetBytes("WSS");
			array[7] = Encoding.UTF8.GetBytes("SCTP");
			array[8] = Encoding.UTF8.GetBytes("TLS-SCTP");
			return array;
		}

		private static byte[][] InitializeTransportParams()
		{
			byte[][] array = new byte[Enum.GetValues(typeof(Transports)).Length][];
			array[0] = new byte[0];
			array[1] = new byte[0];
			array[4] = Encoding.UTF8.GetBytes("tcp");
			array[5] = Encoding.UTF8.GetBytes("tls");
			array[6] = Encoding.UTF8.GetBytes("udp");
			array[2] = Encoding.UTF8.GetBytes("ws");
			array[3] = Encoding.UTF8.GetBytes("ws");
			array[7] = Encoding.UTF8.GetBytes("sctp");
			array[8] = Encoding.UTF8.GetBytes("sctp");
			return array;
		}

		private static byte[][] InitializeAuthSchemes()
		{
			byte[][] array = new byte[Enum.GetValues(typeof(AuthSchemes)).Length][];
			array[0] = new byte[0];
			array[2] = Encoding.UTF8.GetBytes("Digest");
			array[4] = Encoding.UTF8.GetBytes("Kerberos");
			array[1] = Encoding.UTF8.GetBytes("NTLM");
			array[3] = Encoding.UTF8.GetBytes("TLS-DSK");
			return array;
		}

		private static byte[][] InitializeHeaderNames()
		{
			byte[][] array = new byte[Enum.GetValues(typeof(HeaderNames)).Length][];
			array[0] = new byte[0];
			array[1] = Encoding.UTF8.GetBytes("Extension");
			array[2] = Encoding.UTF8.GetBytes("Content-Type");
			array[3] = Encoding.UTF8.GetBytes("Content-Encoding");
			array[4] = Encoding.UTF8.GetBytes("From");
			array[5] = Encoding.UTF8.GetBytes("Call-ID");
			array[6] = Encoding.UTF8.GetBytes("Supported");
			array[7] = Encoding.UTF8.GetBytes("Content-Length");
			array[8] = Encoding.UTF8.GetBytes("Contact");
			array[9] = Encoding.UTF8.GetBytes("Event");
			array[20] = Encoding.UTF8.GetBytes("Expires");
			array[10] = Encoding.UTF8.GetBytes("Subject");
			array[11] = Encoding.UTF8.GetBytes("To");
			array[12] = Encoding.UTF8.GetBytes("Allow-Events");
			array[13] = Encoding.UTF8.GetBytes("Via");
			array[14] = Encoding.UTF8.GetBytes("CSeq");
			array[15] = Encoding.UTF8.GetBytes("Date");
			array[16] = Encoding.UTF8.GetBytes("Allow");
			array[17] = Encoding.UTF8.GetBytes("Route");
			array[18] = Encoding.UTF8.GetBytes("Accept");
			array[19] = Encoding.UTF8.GetBytes("Server");
			array[21] = Encoding.UTF8.GetBytes("Require");
			array[22] = Encoding.UTF8.GetBytes("Warning");
			array[23] = Encoding.UTF8.GetBytes("Priority");
			array[24] = Encoding.UTF8.GetBytes("Reply-To");
			array[25] = Encoding.UTF8.GetBytes("Sip-Etag");
			array[26] = Encoding.UTF8.GetBytes("Call-Info");
			array[27] = Encoding.UTF8.GetBytes("Timestamp");
			array[28] = Encoding.UTF8.GetBytes("Alert-Info");
			array[29] = Encoding.UTF8.GetBytes("Error-Info");
			array[30] = Encoding.UTF8.GetBytes("User-Agent");
			array[31] = Encoding.UTF8.GetBytes("In-Reply-To");
			array[32] = Encoding.UTF8.GetBytes("Min-Expires");
			array[33] = Encoding.UTF8.GetBytes("Retry-After");
			array[34] = Encoding.UTF8.GetBytes("Unsupported");
			array[35] = Encoding.UTF8.GetBytes("Max-Forwards");
			array[36] = Encoding.UTF8.GetBytes("Mime-Version");
			array[37] = Encoding.UTF8.GetBytes("Organization");
			array[38] = Encoding.UTF8.GetBytes("Record-Route");
			array[39] = Encoding.UTF8.GetBytes("Sip-If-Match");
			array[40] = Encoding.UTF8.GetBytes("Authorization");
			array[41] = Encoding.UTF8.GetBytes("Proxy-Require");
			array[42] = Encoding.UTF8.GetBytes("Accept-Encoding");
			array[43] = Encoding.UTF8.GetBytes("Accept-Language");
			array[44] = Encoding.UTF8.GetBytes("Content-Language");
			array[45] = Encoding.UTF8.GetBytes("WWW-Authenticate");
			array[46] = Encoding.UTF8.GetBytes("Proxy-Authenticate");
			array[47] = Encoding.UTF8.GetBytes("Subscription-State");
			array[48] = Encoding.UTF8.GetBytes("Authentication-Info");
			array[49] = Encoding.UTF8.GetBytes("Content-Disposition");
			array[50] = Encoding.UTF8.GetBytes("Proxy-Authorization");
			array[51] = Encoding.UTF8.GetBytes("Proxy-Authentication-Info");
			return array;
		}

		private static void Verify(byte[][] values, Type type)
		{
			int length = Enum.GetValues(type).Length;
			for (int i = 0; i < length; i++)
			{
				if (values[i] == null)
				{
					throw new InvalidProgramException(string.Format("Converter value {0} not defined for type {1}", i, type.Name));
				}
			}
		}
	}
}
