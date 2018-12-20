using Base.Message;
using System;

namespace Sip.Message
{
	public static class StatusCodesConverter
	{
		public static readonly ByteArrayPart NoReason = new ByteArrayPart("Error");

		public static readonly ByteArrayPart Trying = new ByteArrayPart("Trying");

		public static readonly ByteArrayPart Ringing = new ByteArrayPart("Ringing");

		public static readonly ByteArrayPart CallIsBeingForwarded = new ByteArrayPart("Call Is Being Forwarded");

		public static readonly ByteArrayPart Queued = new ByteArrayPart("Queued");

		public static readonly ByteArrayPart SessionProgress = new ByteArrayPart("Session Progress");

		public static readonly ByteArrayPart OK = new ByteArrayPart("OK");

		public static readonly ByteArrayPart MultipleChoices = new ByteArrayPart("Multiple Choices");

		public static readonly ByteArrayPart MovedPermanently = new ByteArrayPart("Moved Permanently");

		public static readonly ByteArrayPart MovedTemporarily = new ByteArrayPart("Moved Temporarily");

		public static readonly ByteArrayPart UseProxy = new ByteArrayPart("Use Proxy");

		public static readonly ByteArrayPart AlternativeService = new ByteArrayPart("Alternative Service");

		public static readonly ByteArrayPart BadRequest = new ByteArrayPart("Bad Request");

		public static readonly ByteArrayPart Unauthorized = new ByteArrayPart("Unauthorized");

		public static readonly ByteArrayPart PaymentRequired = new ByteArrayPart("Payment Required");

		public static readonly ByteArrayPart Forbidden = new ByteArrayPart("Forbidden");

		public static readonly ByteArrayPart NotFound = new ByteArrayPart("Not Found");

		public static readonly ByteArrayPart MethodNotAllowed = new ByteArrayPart("Method Not Allowed");

		public static readonly ByteArrayPart NotAcceptable = new ByteArrayPart("Not Acceptable");

		public static readonly ByteArrayPart ProxyAuthenticationRequired = new ByteArrayPart("Proxy Authentication Required");

		public static readonly ByteArrayPart RequestTimeout = new ByteArrayPart("Request Timeout");

		public static readonly ByteArrayPart Gone = new ByteArrayPart("Gone");

		public static readonly ByteArrayPart RequestEntityTooLarge = new ByteArrayPart("Request Entity Too Large");

		public static readonly ByteArrayPart RequestURITooLarge = new ByteArrayPart("Request URI Too Large");

		public static readonly ByteArrayPart UnsupportedMediaType = new ByteArrayPart("Unsupported Media Type");

		public static readonly ByteArrayPart UnsupportedURIScheme = new ByteArrayPart("Unsupported URI Scheme");

		public static readonly ByteArrayPart BadExtension = new ByteArrayPart("Bad Extension");

		public static readonly ByteArrayPart ExtensionRequired = new ByteArrayPart("Extension Required");

		public static readonly ByteArrayPart IntervalTooBrief = new ByteArrayPart("Interval Too Brief");

		public static readonly ByteArrayPart TemporarilyUnavailable = new ByteArrayPart("Temporarily Unavailable");

		public static readonly ByteArrayPart CallLegTransactionDoesNotExist = new ByteArrayPart("Call Leg Transaction Does Not Exist");

		public static readonly ByteArrayPart LoopDetected = new ByteArrayPart("Loop Detected");

		public static readonly ByteArrayPart TooManyHops = new ByteArrayPart("Too Many Hops");

		public static readonly ByteArrayPart AddressIncomplete = new ByteArrayPart("Address Incomplete");

		public static readonly ByteArrayPart Ambiguous = new ByteArrayPart("Ambiguous");

		public static readonly ByteArrayPart BusyHere = new ByteArrayPart("Busy Here");

		public static readonly ByteArrayPart RequestTerminated = new ByteArrayPart("Request Terminated");

		public static readonly ByteArrayPart NotAcceptableHere = new ByteArrayPart("Not AcceptableHere");

		public static readonly ByteArrayPart RequestPending = new ByteArrayPart("Request Pending");

		public static readonly ByteArrayPart Undecipherable = new ByteArrayPart("Undecipherable");

		public static readonly ByteArrayPart InternalServerError = new ByteArrayPart("Internal Server Error");

		public static readonly ByteArrayPart NotImplemented = new ByteArrayPart("Not Implemented");

		public static readonly ByteArrayPart BadGateway = new ByteArrayPart("Bad Gateway");

		public static readonly ByteArrayPart ServiceUnavailable = new ByteArrayPart("Service Unavailable");

		public static readonly ByteArrayPart ServerTimeOut = new ByteArrayPart("Server Time Out");

		public static readonly ByteArrayPart SipVersionNotSupported = new ByteArrayPart("SIP Version Not Supported");

		public static readonly ByteArrayPart MessageTooLarge = new ByteArrayPart("Message Too Large");

		public static readonly ByteArrayPart BusyEverywhere = new ByteArrayPart("Busy Everywhere");

		public static readonly ByteArrayPart Decline = new ByteArrayPart("Decline");

		public static readonly ByteArrayPart DoesNotExistAnywhere = new ByteArrayPart("Does Not Exist Anywhere");

		public static ByteArrayPart GetReason(this StatusCodes statusCode)
		{
			if (statusCode <= StatusCodes.OK)
			{
				if (statusCode <= StatusCodes.Trying)
				{
					if (statusCode == StatusCodes.None)
					{
						throw new ArgumentOutOfRangeException("statusCode: " + statusCode.ToString());
					}
					if (statusCode == StatusCodes.Trying)
					{
						return StatusCodesConverter.Trying;
					}
				}
				else
				{
					switch (statusCode)
					{
					case StatusCodes.Ringing:
						return StatusCodesConverter.Ringing;
					case StatusCodes.CallIsBeingForwarded:
						return StatusCodesConverter.CallIsBeingForwarded;
					case StatusCodes.Queued:
						return StatusCodesConverter.Queued;
					case StatusCodes.SessionProgress:
						return StatusCodesConverter.SessionProgress;
					default:
						if (statusCode == StatusCodes.OK)
						{
							return StatusCodesConverter.OK;
						}
						break;
					}
				}
			}
			else if (statusCode <= StatusCodes.AlternativeService)
			{
				switch (statusCode)
				{
				case StatusCodes.MultipleChoices:
					return StatusCodesConverter.MultipleChoices;
				case StatusCodes.MovedPermanently:
					return StatusCodesConverter.MovedPermanently;
				case StatusCodes.MovedTemporarily:
					return StatusCodesConverter.MovedTemporarily;
				case (StatusCodes)303:
				case (StatusCodes)304:
					break;
				case StatusCodes.UseProxy:
					return StatusCodesConverter.UseProxy;
				default:
					if (statusCode == StatusCodes.AlternativeService)
					{
						return StatusCodesConverter.AlternativeService;
					}
					break;
				}
			}
			else
			{
				switch (statusCode)
				{
				case StatusCodes.BadRequest:
					return StatusCodesConverter.BadRequest;
				case StatusCodes.Unauthorized:
					return StatusCodesConverter.Unauthorized;
				case StatusCodes.PaymentRequired:
					return StatusCodesConverter.PaymentRequired;
				case StatusCodes.Forbidden:
					return StatusCodesConverter.Forbidden;
				case StatusCodes.NotFound:
					return StatusCodesConverter.NotFound;
				case StatusCodes.MethodNotAllowed:
					return StatusCodesConverter.MethodNotAllowed;
				case StatusCodes.NotAcceptable:
					return StatusCodesConverter.NotAcceptable;
				case StatusCodes.ProxyAuthenticationRequired:
					return StatusCodesConverter.ProxyAuthenticationRequired;
				case StatusCodes.RequestTimeout:
					return StatusCodesConverter.RequestTimeout;
				case (StatusCodes)409:
				case (StatusCodes)411:
				case (StatusCodes)412:
				case (StatusCodes)417:
				case (StatusCodes)418:
				case (StatusCodes)419:
				case (StatusCodes)422:
					break;
				case StatusCodes.Gone:
					return StatusCodesConverter.Gone;
				case StatusCodes.RequestEntityTooLarge:
					return StatusCodesConverter.RequestEntityTooLarge;
				case StatusCodes.RequestURITooLarge:
					return StatusCodesConverter.RequestURITooLarge;
				case StatusCodes.UnsupportedMediaType:
					return StatusCodesConverter.UnsupportedMediaType;
				case StatusCodes.UnsupportedURIScheme:
					return StatusCodesConverter.UnsupportedURIScheme;
				case StatusCodes.BadExtension:
					return StatusCodesConverter.BadExtension;
				case StatusCodes.ExtensionRequired:
					return StatusCodesConverter.ExtensionRequired;
				case StatusCodes.IntervalTooBrief:
					return StatusCodesConverter.IntervalTooBrief;
				default:
					switch (statusCode)
					{
					case StatusCodes.TemporarilyUnavailable:
						return StatusCodesConverter.TemporarilyUnavailable;
					case StatusCodes.CallLegTransactionDoesNotExist:
						return StatusCodesConverter.CallLegTransactionDoesNotExist;
					case StatusCodes.LoopDetected:
						return StatusCodesConverter.LoopDetected;
					case StatusCodes.TooManyHops:
						return StatusCodesConverter.TooManyHops;
					case StatusCodes.AddressIncomplete:
						return StatusCodesConverter.AddressIncomplete;
					case StatusCodes.Ambiguous:
						return StatusCodesConverter.Ambiguous;
					case StatusCodes.BusyHere:
						return StatusCodesConverter.BusyHere;
					case StatusCodes.RequestTerminated:
						return StatusCodesConverter.RequestTerminated;
					case StatusCodes.NotAcceptableHere:
						return StatusCodesConverter.NotAcceptableHere;
					case (StatusCodes)489:
					case (StatusCodes)490:
					case (StatusCodes)492:
					case (StatusCodes)494:
					case (StatusCodes)495:
					case (StatusCodes)496:
					case (StatusCodes)497:
					case (StatusCodes)498:
					case (StatusCodes)499:
					case (StatusCodes)506:
					case (StatusCodes)507:
					case (StatusCodes)508:
					case (StatusCodes)509:
					case (StatusCodes)510:
					case (StatusCodes)511:
					case (StatusCodes)512:
						break;
					case StatusCodes.RequestPending:
						return StatusCodesConverter.RequestPending;
					case StatusCodes.Undecipherable:
						return StatusCodesConverter.Undecipherable;
					case StatusCodes.InternalServerError:
						return StatusCodesConverter.InternalServerError;
					case StatusCodes.NotImplemented:
						return StatusCodesConverter.NotImplemented;
					case StatusCodes.BadGateway:
						return StatusCodesConverter.BadGateway;
					case StatusCodes.ServiceUnavailable:
						return StatusCodesConverter.ServiceUnavailable;
					case StatusCodes.ServerTimeOut:
						return StatusCodesConverter.ServerTimeOut;
					case StatusCodes.SipVersionNotSupported:
						return StatusCodesConverter.SipVersionNotSupported;
					case StatusCodes.MessageTooLarge:
						return StatusCodesConverter.MessageTooLarge;
					default:
						switch (statusCode)
						{
						case StatusCodes.BusyEverywhere:
							return StatusCodesConverter.BusyEverywhere;
						case StatusCodes.Decline:
							return StatusCodesConverter.Decline;
						case StatusCodes.DoesNotExistAnywhere:
							return StatusCodesConverter.DoesNotExistAnywhere;
						}
						break;
					}
					break;
				}
			}
			return StatusCodesConverter.NoReason;
		}
	}
}
