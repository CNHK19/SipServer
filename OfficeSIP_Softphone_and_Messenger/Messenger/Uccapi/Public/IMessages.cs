// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Uccapi
{
	public interface IMessage
	{
		string FromUri { get; }
		string ContentType { get; }
		string Message { get; }
		DateTime DateTime { get; }
	}

	public interface IIncomingMessage
		: IMessage
	{
	}

	public interface IParticipantResult
		: INotifyPropertyChanged
	{
		string Uri { get; }
		bool IsComplete { get; }
		int StatusCode { get; }
		string StatusText { get; }
	}

	public interface IOutgoingMessage
		: IMessage
		, INotifyPropertyChanged
	{
		OutgoingMessageState State { get; }
		string Error { get; }
		int Id { get; }
		ObservableCollection<IParticipantResult> SendResults { get; }
	}

	public class MessageContentType
	{
		public static string Plain = @"text/plain";
		public static string Html = @"text/html";
		public static string Enriched = @"text/enriched";
        public static string FileData = @"file/data";
	}

	public enum OutgoingMessageState
	{
		Created,
		Sending,
		Success,
		PartialSuccess,
		Failed,
	}
}
