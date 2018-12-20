// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	class OutgoingMessage
		: UccapiBase
		, IOutgoingMessage
	{
		private static int count = 0;

		private OutgoingMessageState state;
		private int id = ++count;

		public OutgoingMessage()
		{
			this.SendResults = new ObservableCollection<IParticipantResult>();
		}

		public string FromUri { get; set; }

		public string ContentType { get; set; }
		public string Message { get; set; }
		public DateTime DateTime { get; set; }
		
		public OutgoingMessageState State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (this.state != value)
				{
					this.state = value;
					this.OnPropertyChanged("State");
				}
			}
		}
		
		public string Error { get; set; }

		public int Id 
		{
			get
			{
				return this.id;
			}
		}

		#region ParticipantResult

		public ObservableCollection<IParticipantResult> SendResults { get; private set; }

		public ParticipantResult GetParticipantResult(string uri)
		{
			foreach (ParticipantResult result in this.SendResults)
			{
				if (String.Compare(result.Uri, uri, true) == 0)
					return result;
			}

			ParticipantResult newResult = new ParticipantResult(uri);
			this.SendResults.Add(newResult);

			return newResult;
		}

		public ParticipantResult GetParticipantResult(UccUri uccUri)
		{
			return this.GetParticipantResult(uccUri.Value);
		}

		#endregion
	}
}
