using Base.Message;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sip.Message
{
	public class SipMessageWriter : ByteArrayWriter, IDisposable
	{
		protected struct Range
		{
			public readonly int Offset;

			public readonly int Length;

			public Range(int offset, int length)
			{
				this.Offset = offset;
				this.Length = length;
			}

			public Range(int offset, ByteArrayPart part)
			{
				this.Offset = offset;
				this.Length = part.Length;
			}

			public ByteArrayPart ToByteArrayPart(byte[] bytes, int offset)
			{
				if (this.Length == 0)
				{
					return ByteArrayPart.Invalid;
				}
				return new ByteArrayPart
				{
					Bytes = bytes,
					Begin = offset + this.Offset,
					End = offset + this.Offset + this.Length
				};
			}
		}

		public static class C
		{
			public static readonly ByteArrayPart From = new ByteArrayPart("From");

			public static readonly ByteArrayPart From__ = new ByteArrayPart("From: ");

			public static readonly ByteArrayPart To = new ByteArrayPart("To");

			public static readonly ByteArrayPart To__ = new ByteArrayPart("To: ");

			public static readonly ByteArrayPart Call_ID = new ByteArrayPart("Call-ID");

			public static readonly ByteArrayPart Call_ID__ = new ByteArrayPart("Call-ID: ");

			public static readonly ByteArrayPart CSeq = new ByteArrayPart("CSeq");

			public static readonly ByteArrayPart Max_Forwards = new ByteArrayPart("Max-Forwards");

			public static readonly ByteArrayPart Expires = new ByteArrayPart("Expires");

			public static readonly ByteArrayPart Min_Expires = new ByteArrayPart("Min-Expires");

			public static readonly ByteArrayPart Content_Length = new ByteArrayPart("Content-Length");

			public static readonly ByteArrayPart Contact = new ByteArrayPart("Contact");

			public static readonly ByteArrayPart Contact___ = new ByteArrayPart("Contact: <");

			public static readonly ByteArrayPart WWW_Authenticate = new ByteArrayPart("WWW-Authenticate");

			public static readonly ByteArrayPart Proxy_Authenticate = new ByteArrayPart("Proxy-Authenticate");

			public static readonly ByteArrayPart Authentication_Info = new ByteArrayPart("Authentication-Info");

			public static readonly ByteArrayPart Proxy_Authentication_Info = new ByteArrayPart("Proxy-Authentication-Info");

			public static readonly ByteArrayPart Authorization = new ByteArrayPart("Authorization");

			public static readonly ByteArrayPart Proxy_Authorization = new ByteArrayPart("Proxy-Authorization");

			public static readonly ByteArrayPart Date = new ByteArrayPart("Date");

			public static readonly ByteArrayPart Unsupported = new ByteArrayPart("Unsupported");

			public static readonly ByteArrayPart Route = new ByteArrayPart("Route");

			public static readonly ByteArrayPart RecordRoute = new ByteArrayPart("Record-Route");

			public static readonly ByteArrayPart Via = new ByteArrayPart("Via");

			public static readonly ByteArrayPart Via__SIP_2_0_ = new ByteArrayPart("Via: SIP/2.0/");

			public static readonly ByteArrayPart Content_Type = new ByteArrayPart("Content-Type");

			public static readonly ByteArrayPart Content_Type__ = new ByteArrayPart("Content-Type: ");

			public static readonly ByteArrayPart Supported = new ByteArrayPart("Supported");

			public static readonly ByteArrayPart Require = new ByteArrayPart("Require");

			public static readonly ByteArrayPart Subscription_State = new ByteArrayPart("Subscription-State");

			public static readonly ByteArrayPart Subscription_State__ = new ByteArrayPart("Subscription-State: ");

			public static readonly ByteArrayPart Content_Transfer_Encoding__binary__ = new ByteArrayPart("Content-Transfer-Encoding: binary\r\n");

			public static readonly ByteArrayPart SIP_ETag__ = new ByteArrayPart("SIP-ETag: ");

			public static readonly ByteArrayPart x_Error_Details = new ByteArrayPart("x-Error-Details");

			public static readonly ByteArrayPart Event__presence = new ByteArrayPart("Event: presence");

			public static readonly ByteArrayPart Event__registration = new ByteArrayPart("Event: registration");

			public static readonly ByteArrayPart Allow__ = new ByteArrayPart("Allow: ");

			public static readonly ByteArrayPart Supported__ms_benotify__ = new ByteArrayPart("Supported: ms-benotify\r\n");

			public static readonly ByteArrayPart At = new ByteArrayPart("@");

			public static readonly ByteArrayPart __Digest_username__ = new ByteArrayPart(": Digest username=\"");

			public static readonly ByteArrayPart ___realm__ = new ByteArrayPart("\", realm=\"");

			public static readonly ByteArrayPart ___qop__ = new ByteArrayPart("\", qop=");

			public static readonly ByteArrayPart __algorithm_ = new ByteArrayPart(", algorithm=");

			public static readonly ByteArrayPart __uri__ = new ByteArrayPart(", uri=\"");

			public static readonly ByteArrayPart ___nonce__ = new ByteArrayPart("\", nonce=\"");

			public static readonly ByteArrayPart __nc_ = new ByteArrayPart("\", nc=");

			public static readonly ByteArrayPart __cnonce__ = new ByteArrayPart(", cnonce=\"");

			public static readonly ByteArrayPart ___opaque__ = new ByteArrayPart("\", opaque=\"");

			public static readonly ByteArrayPart ___response__ = new ByteArrayPart("\", response=\"");

			public static readonly ByteArrayPart ACK = new ByteArrayPart("ACK");

			public static readonly ByteArrayPart BENOTIFY = new ByteArrayPart("BENOTIFY");

			public static readonly ByteArrayPart BYE = new ByteArrayPart("BYE");

			public static readonly ByteArrayPart CANCEL = new ByteArrayPart("CANCEL");

			public static readonly ByteArrayPart INFO = new ByteArrayPart("INFO");

			public static readonly ByteArrayPart INVITE = new ByteArrayPart("INVITE");

			public static readonly ByteArrayPart MESSAGE = new ByteArrayPart("MESSAGE");

			public static readonly ByteArrayPart NOTIFY = new ByteArrayPart("NOTIFY");

			public static readonly ByteArrayPart OPTIONS = new ByteArrayPart("OPTIONS");

			public static readonly ByteArrayPart REFER = new ByteArrayPart("REFER");

			public static readonly ByteArrayPart REGISTER = new ByteArrayPart("REGISTER");

			public static readonly ByteArrayPart SERVICE = new ByteArrayPart("SERVICE");

			public static readonly ByteArrayPart SUBSCRIBE = new ByteArrayPart("SUBSCRIBE");

			public static readonly ByteArrayPart PUBLISH = new ByteArrayPart("PUBLISH");

			public static readonly ByteArrayPart SIP_2_0 = new ByteArrayPart("SIP/2.0");

			public static readonly ByteArrayPart SP = new ByteArrayPart(' ');

			public static readonly ByteArrayPart CRLF = new ByteArrayPart("\r\n");

			public static readonly ByteArrayPart DQUOTE = new ByteArrayPart('"');

			public static readonly ByteArrayPart HCOLON = new ByteArrayPart(':');

			public static readonly ByteArrayPart LAQUOT = new ByteArrayPart('<');

			public static readonly ByteArrayPart RAQUOT = new ByteArrayPart('>');

			public static readonly ByteArrayPart EQUAL = new ByteArrayPart('=');

			public static readonly ByteArrayPart SEMI = new ByteArrayPart(';');

			public static readonly ByteArrayPart COMMA = new ByteArrayPart(',');

			public static readonly ByteArrayPart SLASH = new ByteArrayPart('/');

			public static readonly ByteArrayPart BACKSLASH = new ByteArrayPart('\\');

			public static readonly ByteArrayPart CommaSpace = new ByteArrayPart(", ");

			public static readonly ByteArrayPart tag = new ByteArrayPart("tag");

			public static readonly ByteArrayPart _tag_ = new ByteArrayPart(";tag=");

			public static readonly ByteArrayPart received = new ByteArrayPart("received");

			public static readonly ByteArrayPart ms_received_port = new ByteArrayPart("ms-received-port");

			public static readonly ByteArrayPart ms_received_cid = new ByteArrayPart("ms-received-cid");

			public static readonly ByteArrayPart _ms_received_cid_ = new ByteArrayPart(";ms-received-cid=");

			public static readonly ByteArrayPart expires = new ByteArrayPart("expires");

			public static readonly ByteArrayPart _expires_ = new ByteArrayPart(";expires=");

			public static readonly ByteArrayPart Digest = new ByteArrayPart("Digest");

			public static readonly ByteArrayPart NTLM = new ByteArrayPart("NTLM");

			public static readonly ByteArrayPart Kerberos = new ByteArrayPart("Kerberos");

			public static readonly ByteArrayPart realm = new ByteArrayPart("realm");

			public static readonly ByteArrayPart nonce = new ByteArrayPart("nonce");

			public static readonly ByteArrayPart qop = new ByteArrayPart("qop");

			public static readonly ByteArrayPart auth = new ByteArrayPart("auth");

			public static readonly ByteArrayPart auth_int = new ByteArrayPart("auth-int");

			public static readonly ByteArrayPart algorithm = new ByteArrayPart("algorithm");

			public static readonly ByteArrayPart MD5 = new ByteArrayPart("MD5");

			public static readonly ByteArrayPart MD5_sess = new ByteArrayPart("MD5-sess");

			public static readonly ByteArrayPart stale = new ByteArrayPart("stale");

			public static readonly ByteArrayPart _true = new ByteArrayPart("true");

			public static readonly ByteArrayPart _false = new ByteArrayPart("false");

			public static readonly ByteArrayPart opaque = new ByteArrayPart("opaque");

			public static readonly ByteArrayPart targetname = new ByteArrayPart("targetname");

			public static readonly ByteArrayPart version = new ByteArrayPart("version");

			public static readonly ByteArrayPart snum = new ByteArrayPart("snum");

			public static readonly ByteArrayPart _snum__ = new ByteArrayPart(",snum=\"");

			public static readonly ByteArrayPart srand = new ByteArrayPart("srand");

			public static readonly ByteArrayPart _srand__ = new ByteArrayPart(",srand=\"");

			public static readonly ByteArrayPart rspauth = new ByteArrayPart("rspauth");

			public static readonly ByteArrayPart _rspauth__ = new ByteArrayPart(",rspauth=\"");

			public static readonly ByteArrayPart gssapi_data = new ByteArrayPart("gssapi-data");

			public static readonly ByteArrayPart lr = new ByteArrayPart("lr");

			public static readonly ByteArrayPart branch = new ByteArrayPart("branch");

			public static readonly ByteArrayPart _branch_ = new ByteArrayPart(";branch=");

			public static readonly ByteArrayPart _branch_z9hG4bK = new ByteArrayPart(";branch=z9hG4bK");

			public static readonly ByteArrayPart z9hG4bK_NO_TRANSACTION = new ByteArrayPart("z9hG4bK.N0.TRAN5ACT10N");

			public static readonly ByteArrayPart epid = new ByteArrayPart("epid");

			public static readonly ByteArrayPart _epid_ = new ByteArrayPart(";epid=");

			public static readonly ByteArrayPart transport = new ByteArrayPart("transport");

			public static readonly ByteArrayPart maddr = new ByteArrayPart("maddr");

			public static readonly ByteArrayPart active = new ByteArrayPart("active");

			public static readonly ByteArrayPart pending = new ByteArrayPart("pending");

			public static readonly ByteArrayPart terminated = new ByteArrayPart("terminated");

			public static readonly ByteArrayPart __sip_instance___ = new ByteArrayPart(";+sip.instance=\"<");

			public static readonly ByteArrayPart udp = new ByteArrayPart("udp");

			public static readonly ByteArrayPart tcp = new ByteArrayPart("tcp");

			public static readonly ByteArrayPart sip = new ByteArrayPart("sip");

			public static readonly ByteArrayPart sips = new ByteArrayPart("sips");

			public static readonly ByteArrayPart eventlist = new ByteArrayPart("eventlist");

			public static readonly ByteArrayPart ms_benotify = new ByteArrayPart("ms-benotify");

			public static readonly ByteArrayPart _________0 = new ByteArrayPart("         0");

			public static readonly ByteArrayPart Content_Type__multipart_related_type___ = new ByteArrayPart("Content-Type: multipart/related;type=\"");

			public static readonly ByteArrayPart __boundary_OFFICESIP2011VITALIFOMINE__ = new ByteArrayPart("\";boundary=OFFICESIP2011VITALIFOMINE\r\n");

			public static readonly ByteArrayPart __OFFICESIP2011VITALIFOMINE__1 = new ByteArrayPart("--OFFICESIP2011VITALIFOMINE\r\n");

			public static readonly ByteArrayPart __OFFICESIP2011VITALIFOMINE__2 = new ByteArrayPart("--OFFICESIP2011VITALIFOMINE--");

			public static readonly byte[] i = SipMessageWriter.C.Create("i");

			public static readonly byte[] _invalid = SipMessageWriter.C.Create(".invalid");

			public static byte[] Create(string text)
			{
				return Encoding.UTF8.GetBytes(text);
			}
		}

		protected SipMessageWriter.Range callId;

		protected SipMessageWriter.Range fromAddrspec;

		protected SipMessageWriter.Range fromTag;

		protected SipMessageWriter.Range toAddrspec;

		protected SipMessageWriter.Range toTag;

		protected SipMessageWriter.Range fromEpid;

		protected SipMessageWriter.Range toEpid;

		private int contentLengthEnd = -1;

		public event Action<SipMessageWriter> WriteCustomHeadersEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.WriteCustomHeadersEvent = (Action<SipMessageWriter>)Delegate.Combine(this.WriteCustomHeadersEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.WriteCustomHeadersEvent = (Action<SipMessageWriter>)Delegate.Remove(this.WriteCustomHeadersEvent, value);
			}
		}

		public Methods Method
		{
			get;
			protected set;
		}

		public int StatusCode
		{
			get;
			private set;
		}

		public int CSeq
		{
			get;
			protected set;
		}

		public int Expires
		{
			get;
			private set;
		}

		public bool IsRequest
		{
			get
			{
				return this.StatusCode == 0;
			}
		}

		public bool IsResponse
		{
			get
			{
				return this.StatusCode > 0;
			}
		}

		public ByteArrayPart CallId
		{
			get
			{
				return this.callId.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart FromAddrspec
		{
			get
			{
				return this.fromAddrspec.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart FromTag
		{
			get
			{
				return this.fromTag.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart ToAddrspec
		{
			get
			{
				return this.toAddrspec.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart ToTag
		{
			get
			{
				return this.toTag.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart FromEpid
		{
			get
			{
				return this.fromEpid.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart ToEpid
		{
			get
			{
				return this.toEpid.ToByteArrayPart(base.Buffer, base.Segment.Offset);
			}
		}

		public ByteArrayPart Epid
		{
			get
			{
				if (!this.IsRequest)
				{
					return this.FromEpid;
				}
				return this.ToEpid;
			}
		}

		public SipMessageWriter() : this(128, 2048)
		{
		}

		public SipMessageWriter(int size) : this(128, size)
		{
		}

		public SipMessageWriter(int reservAtBegin, int size) : base(reservAtBegin, SipMessage.BufferManager.Allocate(size))
		{
			this.InitializeProperties();
		}

		~SipMessageWriter()
		{
			SipMessage.BufferManager.Free(ref this.segment);
		}

		public void Dispose()
		{
			SipMessage.BufferManager.Free(ref this.segment);
			GC.SuppressFinalize(this);
		}

		protected override void Reallocate(ref ArraySegment<byte> segment, int extraSize)
		{
			SipMessage.BufferManager.Reallocate(ref segment, extraSize);
		}

		public void InitializeProperties()
		{
			this.Method = Methods.None;
			this.StatusCode = 0;
			this.CSeq = 0;
			this.Expires = -2147483648;
			this.callId = default(SipMessageWriter.Range);
			this.fromAddrspec = default(SipMessageWriter.Range);
			this.fromTag = default(SipMessageWriter.Range);
			this.toAddrspec = default(SipMessageWriter.Range);
			this.toTag = default(SipMessageWriter.Range);
			this.fromEpid = default(SipMessageWriter.Range);
			this.toEpid = default(SipMessageWriter.Range);
		}

		public void WriteCustomHeaders()
		{
			Action<SipMessageWriter> writeCustomHeadersEvent = this.WriteCustomHeadersEvent;
			if (writeCustomHeadersEvent != null)
			{
				writeCustomHeadersEvent(this);
			}
		}

		public void WriteHeader(HeaderNames name, ByteArrayPart value)
		{
			base.Write(name.ToUtf8Bytes());
			base.Write(SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, value, SipMessageWriter.C.CRLF);
		}

		public void WriteHeader(Header header)
		{
			base.Write(header.Name, SipMessageWriter.C.HCOLON, header.Value, SipMessageWriter.C.CRLF);
		}

		public void WriteHeader(SipMessageReader reader, int index)
		{
			base.Write(reader.Headers[index].Name, SipMessageWriter.C.HCOLON);
			int begin = reader.Headers[index].Value.Begin;
			HeaderNames headerName = reader.Headers[index].HeaderName;
			switch (headerName)
			{
			case HeaderNames.From:
				this.fromAddrspec = this.CreateRange(reader.From.AddrSpec.Value, begin);
				this.fromTag = this.CreateRange(reader.From.Tag, begin);
				this.fromEpid = this.CreateRange(reader.From.Epid, begin);
				break;
			case HeaderNames.CallId:
				this.callId = this.CreateRange(reader.CallId, begin);
				break;
			default:
				if (headerName != HeaderNames.To)
				{
					if (headerName == HeaderNames.CSeq)
					{
						this.Method = reader.CSeq.Method;
						this.CSeq = reader.CSeq.Value;
					}
				}
				else
				{
					this.toAddrspec = this.CreateRange(reader.To.AddrSpec.Value, begin);
					this.toTag = this.CreateRange(reader.To.Tag, begin);
					this.toEpid = this.CreateRange(reader.To.Epid, begin);
				}
				break;
			}
			base.Write(reader.Headers[index].Value, SipMessageWriter.C.CRLF);
		}

		public void WriteToHeader(SipMessageReader reader, int index, ByteArrayPart epid1)
		{
			if (reader.To.Epid.IsNotEmpty || epid1.Length <= 0)
			{
				this.WriteHeader(reader, index);
				return;
			}
			base.Write(SipMessageWriter.C.To, SipMessageWriter.C.HCOLON);
			int begin = reader.Headers[index].Value.Begin;
			this.toAddrspec = this.CreateRange(reader.To.AddrSpec.Value, begin);
			this.toTag = this.CreateRange(reader.To.Tag, begin);
			base.Write(reader.Headers[index].Value);
			base.Write(SipMessageWriter.C._epid_);
			this.toEpid = new SipMessageWriter.Range(this.end, epid1.Length);
			base.Write(epid1);
			base.Write(SipMessageWriter.C.CRLF);
		}

		private SipMessageWriter.Range CreateRange(ByteArrayPart part, int headerBegin)
		{
			if (part.IsValid)
			{
				return new SipMessageWriter.Range(this.end + part.Begin - headerBegin, part.Length);
			}
			return default(SipMessageWriter.Range);
		}

		public void WriteHeaderName(ByteArrayPart name, bool sp)
		{
			base.Write(name, SipMessageWriter.C.HCOLON);
			if (sp)
			{
				base.Write(SipMessageWriter.C.SP);
			}
		}

		public void WriteStatusLine(int statusCode, ByteArrayPart responsePhrase)
		{
			this.StatusCode = statusCode;
			base.Write(SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.SP, statusCode, SipMessageWriter.C.SP);
			if (responsePhrase.IsValid)
			{
				base.Write(responsePhrase);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteRequestLine(Methods method, ByteArrayPart requestUri)
		{
			this.Method = method;
			base.Write(method.ToByteArrayPart(), SipMessageWriter.C.SP, requestUri, SipMessageWriter.C.SP, SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.CRLF);
		}

		public void WriteRequestLine(Methods method, UriSchemes scheme, ByteArrayPart user, ByteArrayPart domain)
		{
			this.Method = method;
			base.Write(method.ToByteArrayPart(), SipMessageWriter.C.SP);
			base.Write(scheme.ToByteArrayPart(), SipMessageWriter.C.HCOLON, user, SipMessageWriter.C.At, domain);
			base.Write(SipMessageWriter.C.SP, SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.CRLF);
		}

		public void WriteRequestLine(Methods method, Transports transport, IPEndPoint endPoint)
		{
			this.Method = method;
			base.Write(method.ToByteArrayPart(), SipMessageWriter.C.SP, SipMessageWriter.C.sip, SipMessageWriter.C.HCOLON);
			base.Write(endPoint);
			base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.transport, SipMessageWriter.C.EQUAL);
			base.Write(transport.ToTransportParamUtf8Bytes());
			base.Write(SipMessageWriter.C.SP, SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.CRLF);
		}

		public void WriteCRLF()
		{
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteContact(ByteArrayPart addrSpec, ByteArrayPart sipInstance, int expires)
		{
			base.Write(SipMessageWriter.C.Contact___, addrSpec, SipMessageWriter.C.RAQUOT);
			if (sipInstance.IsValid)
			{
				base.Write(SipMessageWriter.C.__sip_instance___, sipInstance, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.DQUOTE);
			}
			base.Write(SipMessageWriter.C._expires_, expires);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteContact(ByteArrayPart hostport, Transports transport)
		{
			base.Write(SipMessageWriter.C.Contact, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, hostport);
			if (transport != Transports.None)
			{
				base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.transport, SipMessageWriter.C.EQUAL, (transport == Transports.Udp) ? SipMessageWriter.C.udp : SipMessageWriter.C.tcp);
			}
			base.Write(SipMessageWriter.C.RAQUOT, SipMessageWriter.C.CRLF);
		}

		public void WriteContact(IPEndPoint endPoint, Transports transport)
		{
			this.WriteContact(endPoint, transport, ByteArrayPart.Invalid);
		}

		public void WriteContact(IPEndPoint endPoint, Transports transport, ByteArrayPart sipInstance)
		{
			this.WriteContact(ByteArrayPart.Invalid, endPoint, transport, sipInstance);
		}

		public void WriteContact(ByteArrayPart user, IPEndPoint endPoint, Transports transport, ByteArrayPart sipInstance)
		{
			base.Write(SipMessageWriter.C.Contact, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, SipMessageWriter.C.sip, SipMessageWriter.C.HCOLON);
			if (user.IsValid)
			{
				base.Write(user, SipMessageWriter.C.At);
			}
			this.Write(endPoint, transport);
			if (transport != Transports.None)
			{
				base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.transport, SipMessageWriter.C.EQUAL);
				base.Write(transport.ToTransportParamUtf8Bytes());
			}
			base.Write(SipMessageWriter.C.RAQUOT);
			if (sipInstance.IsValid)
			{
				base.Write(SipMessageWriter.C.__sip_instance___, sipInstance, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.DQUOTE);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteExpires(int expires)
		{
			this.Expires = expires;
			base.Write(SipMessageWriter.C.Expires, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, expires, SipMessageWriter.C.CRLF);
		}

		public void WriteMinExpires(int expires)
		{
			base.Write(SipMessageWriter.C.Min_Expires, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, expires, SipMessageWriter.C.CRLF);
		}

		public void WriteContentLength(int length)
		{
			base.Write(SipMessageWriter.C.Content_Length, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, length, SipMessageWriter.C.CRLF);
		}

		public void WriteCseq(int number, Methods method)
		{
			this.CSeq = number;
			this.Method = method;
			base.Write(SipMessageWriter.C.CSeq, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, number, SipMessageWriter.C.SP, method.ToByteArrayPart(), SipMessageWriter.C.CRLF);
		}

		public void WriteTo(Header header, ByteArrayPart tag, ByteArrayPart ctag)
		{
			if (header.Name.IsValid)
			{
				base.Write(header.Name, SipMessageWriter.C.HCOLON);
				if (ctag.IsValid)
				{
					base.Write(new ByteArrayPart
					{
						Bytes = header.Value.Bytes,
						Begin = header.Value.Begin,
						End = ctag.Begin
					}, tag, new ByteArrayPart
					{
						Bytes = header.Value.Bytes,
						Begin = ctag.End,
						End = header.Value.End
					});
				}
				else
				{
					base.Write(header.Value);
					if (tag.IsValid)
					{
						base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.tag, SipMessageWriter.C.EQUAL, tag);
					}
				}
				base.Write(SipMessageWriter.C.CRLF);
			}
		}

		public void WriteFrom(Header header, ByteArrayPart tag)
		{
			if (header.Name.IsValid)
			{
				base.Write(header.Name, SipMessageWriter.C.HCOLON, header.Value);
				if (tag.IsValid)
				{
					base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.tag, SipMessageWriter.C.EQUAL, tag);
				}
				base.Write(SipMessageWriter.C.CRLF);
			}
		}

		public void WriteTo(Header header, ByteArrayPart epid1)
		{
			if (header.Name.IsValid)
			{
				base.Write(header.Name, SipMessageWriter.C.HCOLON, header.Value);
				if (epid1.IsValid)
				{
					base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.epid, SipMessageWriter.C.EQUAL);
					this.toEpid = new SipMessageWriter.Range(this.end, epid1.Length);
					base.Write(epid1);
				}
				base.Write(SipMessageWriter.C.CRLF);
			}
		}

		public void WriteVia(Transports transport, ByteArrayPart host, int port, ByteArrayPart branch)
		{
			base.Write(SipMessageWriter.C.Via, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.SLASH);
			base.Write(transport.ToUtf8Bytes());
			base.Write(SipMessageWriter.C.SP, host, SipMessageWriter.C.HCOLON);
			base.Write(port);
			base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.branch, SipMessageWriter.C.EQUAL, branch, SipMessageWriter.C.CRLF);
		}

		public void WriteAuthenticateDigest(bool proxy, ByteArrayPart realm, int nonce1, int nonce2, int nonce3, int nonce4, bool authint, bool stale, int opaque)
		{
			base.Write(proxy ? SipMessageWriter.C.Proxy_Authenticate : SipMessageWriter.C.WWW_Authenticate, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.Digest, SipMessageWriter.C.SP);
			base.Write(SipMessageWriter.C.realm, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE, realm, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.nonce, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			base.WriteAsHex8(nonce1);
			base.WriteAsHex8(nonce2);
			base.WriteAsHex8(nonce3);
			base.WriteAsHex8(nonce4);
			base.Write(SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.qop, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.auth);
			if (authint)
			{
				base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.auth_int);
			}
			base.Write(SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.algorithm, SipMessageWriter.C.EQUAL, SipMessageWriter.C.MD5, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.stale, SipMessageWriter.C.EQUAL, stale ? SipMessageWriter.C._true : SipMessageWriter.C._false, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.opaque, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			base.WriteAsHex8(opaque);
			base.Write(SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteAuthenticationInfo(bool proxy, AuthSchemes scheme, ByteArrayPart targetname, ByteArrayPart realm, int opaque, int snum, int srand, ArraySegment<byte> rspauth)
		{
			base.Write(proxy ? SipMessageWriter.C.Proxy_Authentication_Info : SipMessageWriter.C.Authentication_Info, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, (scheme == AuthSchemes.Ntlm) ? SipMessageWriter.C.NTLM : SipMessageWriter.C.Kerberos, SipMessageWriter.C.SP);
			base.Write(SipMessageWriter.C.targetname, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			if (scheme == AuthSchemes.Kerberos)
			{
				base.Write(SipMessageWriter.C.sip, SipMessageWriter.C.SLASH);
			}
			base.Write(targetname, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.realm, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE, realm, SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.opaque, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			base.WriteAsHex8(opaque);
			base.Write(SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.qop, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.auth, SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C._snum__, snum, SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C._srand__);
			base.WriteAsHex8(srand);
			base.Write(SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C._rspauth__);
			base.WriteAsHex(rspauth);
			base.Write(SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteAuthenticateMs(bool proxy, AuthSchemes scheme, ByteArrayPart targetname, ByteArrayPart realm, int opaque)
		{
			base.Write(proxy ? SipMessageWriter.C.Proxy_Authenticate : SipMessageWriter.C.WWW_Authenticate, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, (scheme == AuthSchemes.Ntlm) ? SipMessageWriter.C.NTLM : SipMessageWriter.C.Kerberos, SipMessageWriter.C.SP);
			base.Write(SipMessageWriter.C.targetname, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			if (scheme == AuthSchemes.Kerberos)
			{
				base.Write(SipMessageWriter.C.sip, SipMessageWriter.C.SLASH);
			}
			base.Write(targetname, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.realm, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE, realm, SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.version, SipMessageWriter.C.EQUAL, 3);
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.opaque, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			base.WriteAsHex8(opaque);
			base.Write(SipMessageWriter.C.DQUOTE, SipMessageWriter.C.CRLF);
		}

		public void WriteAuthenticateMs(bool proxy, AuthSchemes scheme, ByteArrayPart targetname, ByteArrayPart realm, int opaque, ArraySegment<byte> gssapiData)
		{
			base.Write(proxy ? SipMessageWriter.C.Proxy_Authenticate : SipMessageWriter.C.WWW_Authenticate, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, (scheme == AuthSchemes.Ntlm) ? SipMessageWriter.C.NTLM : SipMessageWriter.C.Kerberos, SipMessageWriter.C.SP);
			base.Write(SipMessageWriter.C.targetname, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			if (scheme == AuthSchemes.Kerberos)
			{
				base.Write(SipMessageWriter.C.sip, SipMessageWriter.C.SLASH);
			}
			base.Write(targetname, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.realm, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE, realm, SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.version, SipMessageWriter.C.EQUAL, 3);
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.opaque, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			base.WriteAsHex8(opaque);
			base.Write(SipMessageWriter.C.DQUOTE, SipMessageWriter.C.COMMA);
			base.Write(SipMessageWriter.C.gssapi_data, SipMessageWriter.C.EQUAL, SipMessageWriter.C.DQUOTE);
			base.WriteAsBase64(gssapiData);
			base.Write(SipMessageWriter.C.DQUOTE, SipMessageWriter.C.CRLF);
		}

		public void WriteDate(DateTime date)
		{
			base.Write(SipMessageWriter.C.Date, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, date.ToString("R").ToByteArrayPart(), SipMessageWriter.C.CRLF);
		}

		public void WriteMaxForwards(int hop)
		{
			base.Write(SipMessageWriter.C.Max_Forwards, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, hop, SipMessageWriter.C.CRLF);
		}

		public void WriteUnsupported(ByteArrayPart value)
		{
			base.Write(SipMessageWriter.C.Unsupported, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, value);
		}

		public void WriteUnsupportedValue(ByteArrayPart value)
		{
			base.Write(SipMessageWriter.C.COMMA, SipMessageWriter.C.SP, value);
		}

		public void WriteRoute(ByteArrayPart value)
		{
			base.Write(SipMessageWriter.C.Route, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, value, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.CRLF);
		}

		public void WriteRecordRoute(UriSchemes scheme, ByteArrayPart host, int port)
		{
			base.Write(SipMessageWriter.C.RecordRoute, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, scheme.ToByteArrayPart(), SipMessageWriter.C.HCOLON, host, SipMessageWriter.C.HCOLON);
			base.Write(port);
			base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.lr, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.CRLF);
		}

		public void WriteRecordRoute(Transports transport, IPEndPoint endpoint, ArraySegment<byte> msReceivedCid)
		{
			UriSchemes scheme = (transport == Transports.Tls) ? UriSchemes.Sips : UriSchemes.Sip;
			base.Write(SipMessageWriter.C.RecordRoute, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, scheme.ToByteArrayPart(), SipMessageWriter.C.HCOLON);
			this.Write(endpoint, transport);
			base.Write(SipMessageWriter.C._ms_received_cid_);
			base.Write(msReceivedCid);
			base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.lr, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.CRLF);
		}

		public void WriteRecordRoute(Transports transport, IPEndPoint endpoint)
		{
			UriSchemes scheme = (transport == Transports.Tls) ? UriSchemes.Sips : UriSchemes.Sip;
			base.Write(SipMessageWriter.C.RecordRoute, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, scheme.ToByteArrayPart(), SipMessageWriter.C.HCOLON);
			this.Write(endpoint, transport);
			base.Write(SipMessageWriter.C.SEMI, SipMessageWriter.C.lr, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.CRLF);
		}

		public void WriteContentType(ByteArrayPart type, ByteArrayPart subtype, ByteArrayPart parameters)
		{
			base.Write(SipMessageWriter.C.Content_Type, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP);
			base.Write(type);
			base.Write(SipMessageWriter.C.SLASH);
			base.Write(subtype);
			base.Write(parameters);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteSupported(ByteArrayPart options)
		{
			base.Write(SipMessageWriter.C.Supported, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, options, SipMessageWriter.C.CRLF);
		}

		public void WriteRequire(ByteArrayPart requires)
		{
			base.Write(SipMessageWriter.C.Require, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, requires, SipMessageWriter.C.CRLF);
		}

		public void WriteSubscriptionState(ByteArrayPart substate, int expires)
		{
			base.Write(SipMessageWriter.C.Subscription_State, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, substate, SipMessageWriter.C.SEMI, SipMessageWriter.C.expires, SipMessageWriter.C.EQUAL);
			base.Write(expires);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteStatusLine(StatusCodes statusCode)
		{
			this.StatusCode = (int)statusCode;
			base.Write(SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.SP, (int)statusCode, SipMessageWriter.C.SP, statusCode.GetReason(), SipMessageWriter.C.CRLF);
		}

		public void WriteStatusLine(StatusCodes statusCode, ByteArrayPart reason)
		{
			this.StatusCode = (int)statusCode;
			base.Write(SipMessageWriter.C.SIP_2_0, SipMessageWriter.C.SP, (int)statusCode, SipMessageWriter.C.SP, reason, SipMessageWriter.C.CRLF);
		}

		public void WriteHeaderWithTag(Header header, ByteArrayPart tag)
		{
			base.Write(header.Name, SipMessageWriter.C.HCOLON, header.Value, SipMessageWriter.C.SEMI, SipMessageWriter.C.tag, SipMessageWriter.C.EQUAL, tag, SipMessageWriter.C.CRLF);
		}

		public ByteArrayPart GenerateTag()
		{
			return new ByteArrayPart(Guid.NewGuid().ToString().Replace("-", "").ToLower());
		}

		public void WriteContentLength()
		{
			base.Write(SipMessageWriter.C.Content_Length, SipMessageWriter.C.HCOLON, SipMessageWriter.C._________0);
			this.contentLengthEnd = this.end;
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void RewriteContentLength(int value)
		{
			if (this.contentLengthEnd < 0)
			{
				throw new InvalidOperationException("WriteContentLength must be called before RewriteContentLength");
			}
			base.ReversWrite((uint)value, ref this.contentLengthEnd);
			this.contentLengthEnd = -1;
		}

		public void RewriteContentLength()
		{
			if (this.contentLengthEnd < 0)
			{
				throw new InvalidOperationException("WriteContentLength must be called before RewriteContentLength");
			}
			base.ReversWrite((uint)(this.end - this.contentLengthEnd - SipMessageWriter.C.CRLF.Length * 2), ref this.contentLengthEnd);
			this.contentLengthEnd = -1;
		}

		public void WriteXErrorDetails(byte[] details)
		{
			base.Write(SipMessageWriter.C.x_Error_Details, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP);
			base.Write(details);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteXErrorDetails(ByteArrayPart details)
		{
			base.Write(SipMessageWriter.C.x_Error_Details, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, details, SipMessageWriter.C.CRLF);
		}

		public void WriteXErrorDetails(ByteArrayPart details1, byte[] details2)
		{
			base.Write(SipMessageWriter.C.x_Error_Details, SipMessageWriter.C.HCOLON, SipMessageWriter.C.SP, details1);
			if (details2 != null && details2.Length > 0)
			{
				base.Write(SipMessageWriter.C.CommaSpace);
				base.Write(details2);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteTo(ByteArrayPart uri, ByteArrayPart tag)
		{
			base.Write(SipMessageWriter.C.To__, SipMessageWriter.C.LAQUOT, uri, SipMessageWriter.C.RAQUOT);
			if (tag.IsValid)
			{
				base.Write(SipMessageWriter.C._tag_, tag);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteTo2(ByteArrayPart user, ByteArrayPart domain, ByteArrayPart tag)
		{
			base.Write(SipMessageWriter.C.To__, SipMessageWriter.C.LAQUOT, SipMessageWriter.C.sip, SipMessageWriter.C.HCOLON, user, SipMessageWriter.C.At, domain, SipMessageWriter.C.RAQUOT);
			if (tag.IsValid)
			{
				base.Write(SipMessageWriter.C._tag_, tag);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteToRaw(ByteArrayPart user, ByteArrayPart domain, ByteArrayPart raw)
		{
			base.Write(SipMessageWriter.C.To__, SipMessageWriter.C.LAQUOT, SipMessageWriter.C.sip, SipMessageWriter.C.HCOLON, user, SipMessageWriter.C.At, domain, SipMessageWriter.C.RAQUOT);
			base.Write(raw, SipMessageWriter.C.CRLF);
		}

		public void WriteTo(ByteArrayPart uri, ByteArrayPart tag, ByteArrayPart epid1)
		{
			base.Write(SipMessageWriter.C.To__, SipMessageWriter.C.LAQUOT);
			this.toAddrspec = new SipMessageWriter.Range(this.end, uri.Length);
			base.Write(uri, SipMessageWriter.C.RAQUOT, SipMessageWriter.C._tag_);
			this.toTag = new SipMessageWriter.Range(this.end, tag.Length);
			base.Write(tag);
			if (epid1.IsNotEmpty)
			{
				base.Write(SipMessageWriter.C._epid_);
				this.toEpid = new SipMessageWriter.Range(this.end, epid1.Length);
				base.Write(epid1);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteTo(ByteArrayPart uri)
		{
			base.Write(SipMessageWriter.C.To__, SipMessageWriter.C.LAQUOT);
			this.toAddrspec = new SipMessageWriter.Range(this.end, uri.Length);
			base.Write(uri, SipMessageWriter.C.RAQUOT, SipMessageWriter.C.CRLF);
		}

		public void WriteFrom(ByteArrayPart uri, ByteArrayPart tag)
		{
			this.WriteFrom(uri, tag, ByteArrayPart.Invalid);
		}

		public void WriteFrom(ByteArrayPart uri, ByteArrayPart tag, ByteArrayPart epid)
		{
			base.Write(SipMessageWriter.C.From__, SipMessageWriter.C.LAQUOT);
			this.fromAddrspec = new SipMessageWriter.Range(this.end, uri.Length);
			base.Write(uri, SipMessageWriter.C.RAQUOT, SipMessageWriter.C._tag_);
			this.fromTag = new SipMessageWriter.Range(this.end, tag.Length);
			base.Write(tag);
			if (epid.IsValid)
			{
				base.Write(SipMessageWriter.C._epid_, epid);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteFrom(ByteArrayPart uri, int tag)
		{
			base.Write(SipMessageWriter.C.From__, SipMessageWriter.C.LAQUOT);
			this.fromAddrspec = new SipMessageWriter.Range(this.end, uri.Length);
			base.Write(uri, SipMessageWriter.C.RAQUOT, SipMessageWriter.C._tag_);
			this.fromTag = new SipMessageWriter.Range(this.end, 8);
			base.WriteAsHex8(tag);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteFromRaw(ByteArrayPart displayName, ByteArrayPart uri, ByteArrayPart raw)
		{
			base.Write(SipMessageWriter.C.From__, SipMessageWriter.C.DQUOTE, displayName, SipMessageWriter.C.DQUOTE, SipMessageWriter.C.SP, SipMessageWriter.C.LAQUOT, uri, SipMessageWriter.C.RAQUOT, raw);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteCallId(ByteArrayPart callIdValue)
		{
			base.Write(SipMessageWriter.C.Call_ID__);
			this.callId = new SipMessageWriter.Range(this.end, callIdValue.Length);
			base.Write(callIdValue, SipMessageWriter.C.CRLF);
		}

		public void WriteCallId(IPAddress localAddreess, int random)
		{
			base.Write(SipMessageWriter.C.Call_ID__);
			int end = this.end;
			base.WriteAsHex8(random);
			base.Write(SipMessageWriter.C.At);
			base.Write(localAddreess);
			this.callId = new SipMessageWriter.Range(end, this.end);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteEventPresence()
		{
			base.Write(SipMessageWriter.C.Event__presence, SipMessageWriter.C.CRLF);
		}

		public void WriteEventRegistration()
		{
			base.Write(SipMessageWriter.C.Event__registration, SipMessageWriter.C.CRLF);
		}

		public void WriteSubscriptionState(int expires)
		{
			base.Write(SipMessageWriter.C.Subscription_State__);
			if (expires > 0)
			{
				base.Write(SipMessageWriter.C.active, SipMessageWriter.C._expires_, expires);
			}
			else
			{
				base.Write(SipMessageWriter.C.terminated);
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteSubscriptionState(SubscriptionStates state, int expires)
		{
			base.Write(SipMessageWriter.C.Subscription_State__);
			switch (state)
			{
			case SubscriptionStates.Active:
				base.Write(SipMessageWriter.C.active, SipMessageWriter.C._expires_, expires);
				break;
			case SubscriptionStates.Pending:
				base.Write(SipMessageWriter.C.pending, SipMessageWriter.C._expires_, expires);
				break;
			case SubscriptionStates.Terminated:
				base.Write(SipMessageWriter.C.terminated);
				break;
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteContentType(ByteArrayPart type, ByteArrayPart subtype)
		{
			base.Write(SipMessageWriter.C.Content_Type__, type, SipMessageWriter.C.SLASH, subtype, SipMessageWriter.C.CRLF);
		}

		public void WriteContentType(ByteArrayPart value)
		{
			base.Write(SipMessageWriter.C.Content_Type__, value, SipMessageWriter.C.CRLF);
		}

		public void WriteVia(Transports transport, IPEndPoint endpoint, int branch)
		{
			base.Write(SipMessageWriter.C.Via__SIP_2_0_);
			base.Write(transport.ToUtf8Bytes());
			base.Write(SipMessageWriter.C.SP);
			this.Write(endpoint, transport);
			base.Write(SipMessageWriter.C._branch_z9hG4bK);
			base.WriteAsHex8(branch);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteVia(Transports transport, IPEndPoint endpoint)
		{
			base.Write(SipMessageWriter.C.Via__SIP_2_0_);
			base.Write(transport.ToUtf8Bytes());
			base.Write(SipMessageWriter.C.SP);
			this.Write(endpoint, transport);
			base.Write(SipMessageWriter.C._branch_);
			base.Write(SipMessageWriter.C.z9hG4bK_NO_TRANSACTION);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteVia(Transports transport, IPEndPoint endpoint, int branch, ArraySegment<byte> msRecivedCid)
		{
			base.Write(SipMessageWriter.C.Via__SIP_2_0_);
			base.Write(transport.ToUtf8Bytes());
			base.Write(SipMessageWriter.C.SP);
			this.Write(endpoint, transport);
			base.Write(SipMessageWriter.C._branch_z9hG4bK);
			base.WriteAsHex8(branch);
			base.Write(SipMessageWriter.C._ms_received_cid_);
			base.Write(msRecivedCid);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void Write(IPEndPoint endpoint, Transports transport)
		{
			if (transport != Transports.Ws && transport != Transports.Wss)
			{
				base.Write(endpoint);
				return;
			}
			base.Write(SipMessageWriter.C.i);
			base.Write((uint)endpoint.GetHashCode());
			base.Write(SipMessageWriter.C._invalid);
		}

		public void WriteDigestAuthorization(HeaderNames header, ByteArrayPart username, ByteArrayPart realm, AuthQops qop, AuthAlgorithms algorithm, ByteArrayPart uri, ByteArrayPart nonce, int nc, int cnonce, ByteArrayPart opaque, byte[] response)
		{
			if (header == HeaderNames.Authorization)
			{
				base.Write(SipMessageWriter.C.Authorization);
			}
			else
			{
				if (header != HeaderNames.ProxyAuthorization)
				{
					throw new ArgumentException("HeaderNames");
				}
				base.Write(SipMessageWriter.C.Proxy_Authorization);
			}
			base.Write(SipMessageWriter.C.__Digest_username__);
			base.Write(username);
			base.Write(SipMessageWriter.C.___realm__);
			base.Write(realm);
			if (qop != AuthQops.None)
			{
				base.Write(SipMessageWriter.C.___qop__);
				base.Write(qop.ToByteArrayPart());
			}
			else
			{
				base.Write(SipMessageWriter.C.DQUOTE);
			}
			base.Write(SipMessageWriter.C.__algorithm_);
			base.Write(algorithm.ToByteArrayPart());
			base.Write(SipMessageWriter.C.__uri__);
			base.Write(uri);
			base.Write(SipMessageWriter.C.___nonce__);
			base.Write(nonce);
			if (qop != AuthQops.None)
			{
				base.Write(SipMessageWriter.C.__nc_);
				base.Write(nc);
				base.Write(SipMessageWriter.C.__cnonce__);
				base.WriteAsHex8(cnonce);
			}
			base.Write(SipMessageWriter.C.___opaque__);
			if (opaque.IsValid)
			{
				base.Write(opaque);
			}
			base.Write(SipMessageWriter.C.___response__);
			base.Write(response);
			base.Write(SipMessageWriter.C.DQUOTE);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteSupportedMsBenotify()
		{
			base.Write(SipMessageWriter.C.Supported__ms_benotify__);
		}

		public void WriteAllow(Methods[] methods)
		{
			base.Write(SipMessageWriter.C.Allow__);
			base.Write(methods[0].ToByteArrayPart());
			for (int i = 1; i < methods.Length; i++)
			{
				base.Write(SipMessageWriter.C.CommaSpace);
				base.Write(methods[i].ToByteArrayPart());
			}
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteContentTransferEncodingBinary()
		{
			base.Write(SipMessageWriter.C.Content_Transfer_Encoding__binary__);
		}

		public void WriteSipEtag(int value)
		{
			base.Write(SipMessageWriter.C.SIP_ETag__);
			base.WriteAsHex8(value);
			base.Write(SipMessageWriter.C.CRLF);
		}

		public void WriteContentTypeMultipart(ByteArrayPart contentType)
		{
			base.Write(SipMessageWriter.C.Content_Type__multipart_related_type___, contentType, SipMessageWriter.C.__boundary_OFFICESIP2011VITALIFOMINE__);
		}

		public void WriteBoundary()
		{
			base.Write(SipMessageWriter.C.__OFFICESIP2011VITALIFOMINE__1);
		}

		public void WriteBoundaryEnd()
		{
			base.Write(SipMessageWriter.C.__OFFICESIP2011VITALIFOMINE__2);
		}
	}
}
