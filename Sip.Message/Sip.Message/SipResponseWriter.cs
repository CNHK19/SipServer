using Base.Message;
using System;

namespace Sip.Message
{
	public class SipResponseWriter : SipMessageWriter
	{
		public SipResponseWriter()
		{
		}

		public SipResponseWriter(int size) : base(size)
		{
		}

		public void WriteStatusLineToTop(StatusCodes statusCode)
		{
			this.WriteStatusLineToTop(statusCode, statusCode.GetReason());
		}

		public void WriteStatusLineToTop(StatusCodes statusCode, ByteArrayPart reason)
		{
			base.WriteToTop(SipMessageWriter.C.CRLF);
			base.WriteToTop(reason, 100);
			base.WriteToTop(SipMessageWriter.C.SP);
			base.WriteToTop((uint)statusCode);
			base.WriteToTop(SipMessageWriter.C.SP);
			base.WriteToTop(SipMessageWriter.C.SIP_2_0);
		}

		public void WriteResponse(SipMessageReader request, StatusCodes statusCode)
		{
			this.WriteResponse(request, statusCode, base.GenerateTag());
		}

		public void WriteResponse(SipMessageReader request, StatusCodes statusCode, ByteArrayPart localTag)
		{
			base.WriteStatusLine(statusCode);
			this.CopyViaToFromCallIdRecordRouteCSeq(request, statusCode, localTag);
			base.WriteContentLength(0);
			base.WriteCustomHeaders();
			base.WriteCRLF();
		}

		public void CopyViaToFromCallIdRecordRouteCSeq(SipMessageReader request, StatusCodes statusCode)
		{
			this.CopyViaToFromCallIdRecordRouteCSeq(request, statusCode, base.GenerateTag());
		}

		public void CopyViaToFromCallIdRecordRouteCSeq(SipMessageReader request, StatusCodes statusCode, ByteArrayPart localTag)
		{
			for (int i = 0; i < request.Count.HeaderCount; i++)
			{
				HeaderNames headerName = request.Headers[i].HeaderName;
				switch (headerName)
				{
				case HeaderNames.From:
				{
					base.Write(SipMessageWriter.C.From__);
					int arg_111_0 = this.end + request.From.AddrSpec.Value.Begin - request.Headers[i].Value.Begin;
					BeginEndIndex value = request.From.AddrSpec.Value;
					this.fromAddrspec = new SipMessageWriter.Range(arg_111_0, value.Length);
					if (request.From.Tag.IsValid)
					{
						this.fromTag = new SipMessageWriter.Range(this.end + request.From.Tag.Begin - request.Headers[i].Value.Begin, request.From.Tag.Length);
					}
					if (request.From.Epid.IsValid)
					{
						this.fromEpid = new SipMessageWriter.Range(this.end + request.From.Epid.Begin - request.Headers[i].Value.Begin, request.From.Epid.Length);
					}
					base.Write(request.Headers[i].Value);
					base.WriteCRLF();
					break;
				}
				case HeaderNames.CallId:
					base.WriteCallId(request.CallId);
					break;
				default:
					switch (headerName)
					{
					case HeaderNames.To:
					{
						base.Write(SipMessageWriter.C.To__);
						int arg_24F_0 = this.end + request.To.AddrSpec.Value.Begin - request.Headers[i].Value.Begin;
						BeginEndIndex value2 = request.To.AddrSpec.Value;
						this.toAddrspec = new SipMessageWriter.Range(arg_24F_0, value2.Length);
						if (request.To.Tag.IsValid)
						{
							this.toTag = new SipMessageWriter.Range(this.end + request.To.Tag.Begin - request.Headers[i].Value.Begin, request.To.Tag.Length);
						}
						base.Write(request.Headers[i].Value);
						if (request.To.Tag.IsInvalid && localTag.IsValid && statusCode != StatusCodes.Trying && request.Method != Methods.Cancelm)
						{
							base.Write(SipMessageWriter.C._tag_);
							this.toTag = new SipMessageWriter.Range(this.end, localTag.Length);
							base.Write(localTag);
						}
						base.WriteCRLF();
						goto IL_329;
					}
					case HeaderNames.AllowEvents:
						goto IL_329;
					case HeaderNames.Via:
						break;
					case HeaderNames.CSeq:
						base.CSeq = request.CSeq.Value;
						base.Method = request.CSeq.Method;
						base.WriteHeader(request.Headers[i]);
						goto IL_329;
					default:
						if (headerName != HeaderNames.RecordRoute)
						{
							goto IL_329;
						}
						break;
					}
					base.WriteHeader(request.Headers[i]);
					break;
				}
				IL_329:;
			}
		}
	}
}
