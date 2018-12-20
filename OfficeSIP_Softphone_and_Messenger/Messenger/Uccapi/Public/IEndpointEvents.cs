// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;
using System.ComponentModel;
using System.Timers;

namespace Uccapi
{
	public enum AvInviteState
	{
		Pending,
		Accepted,
		Declined,
		Timeout,
		Failed,
	}

	public class AvInvite
		: EventArgs
		, INotifyPropertyChanged
	{
		public delegate void CreateSessionDelegate(IUccSession uccSession);

		private AvInviteState state;
		private IPresentity inviter;
		private UccIncomingSessionEvent eventData;
		private CreateSessionDelegate createSession;
		private Timer timer;

		public AvInvite(IPresentity inviter, int timeout, UccIncomingSessionEvent eventData, CreateSessionDelegate createSession)
		{
			this.Created = DateTime.Now;
			this.state = AvInviteState.Pending;
			this.Accepted = false;

			this.timer = new Timer();
			this.timer.AutoReset = false;
			this.timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
			this.timer.Interval = 1000 * timeout;
			this.timer.Start();
			
			this.inviter = inviter;
			this.eventData = eventData;
			this.createSession = createSession;
		}

		public AvInviteState State 
		{
			get { return state; }
			private set
			{
				if (state != value)
				{
					state = value;
					OnPropertyChanged(@"State");
				}
			}
		}

		public DateTime Created
		{
			get;
			private set;
		}

		public IPresentity Inviter
		{
			get { return this.inviter; }
		}

		public void Accept()
		{
			if (State == AvInviteState.Pending)
			{
				this.Accepted = true;

				try
				{
					eventData.Accept();
					createSession(eventData.Session);

					Dispose();
					State = AvInviteState.Accepted;
				}
				catch (COMException)
				{
					Dispose();
					State = AvInviteState.Failed;
				}
			}
		}

		public void Decline()
		{
			if (State == AvInviteState.Pending)
			{
				this.Accepted = false;

				try
				{
					eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE);
				}
				catch (COMException) { }

				Dispose();
				State = AvInviteState.Declined;
			}
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (State == AvInviteState.Pending)
			{
				try
				{
					eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_TIMEOUT);
				}
				catch (COMException) { }

				Dispose();
				State = AvInviteState.Timeout;
			}
		}

		protected void Dispose()
		{
			eventData = null;
			createSession = null;

			if (timer != null)
			{
				timer.Stop();
				timer.Dispose();
				timer = null;
			}
		}

		public bool Accepted { get; private set; }


		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(String property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		#endregion INotifyPropertyChanged
	}

	public class EndpointEventArgs
		: EventArgs
	{
		private DateTime dateTime = DateTime.Now;

		public EndpointEventArgs(IUccOperationProgressEvent eventData)
		{
			this.IsOperationCompleteFailed = Helpers.IsOperationCompleteFailed(eventData);
			this.StatusCode = eventData.StatusCode;
			this.StatusText = Errors.ToString(eventData.StatusCode);
		}

		public EndpointEventArgs(IUccOperationProgressEvent eventData, AuthenticationMode authMode)
		{
			this.IsOperationCompleteFailed = Helpers.IsOperationCompleteFailed(eventData);
			this.StatusCode = eventData.StatusCode;
			this.StatusText = Errors.ToString(eventData.StatusCode);
			this.AuthMode = authMode;
		}

		public EndpointEventArgs(string statusText)
		{
			this.IsOperationCompleteFailed = true;
			this.StatusCode = -1;
			this.StatusText = statusText;
		}

		public EndpointEventArgs(COMException ex)
		{
			this.IsOperationCompleteFailed = true;
			this.StatusCode = ex.ErrorCode;
			this.StatusText = Errors.ToString(ex.ErrorCode);
		}

		public EndpointEventArgs(Exception ex)
		{
			this.IsOperationCompleteFailed = true;
			this.StatusCode = -1;
			this.StatusText = ex.Message;
		}

		public bool IsOperationCompleteFailed { get; private set; }
		public int StatusCode { get; private set; }
		public string StatusText { get; private set; }
		public AuthenticationMode? AuthMode { get; private set; }

		public DateTime DateTime
		{
			get
			{
				return this.dateTime;
			}
		}
	}

	public class EndpointEventArgss
		: EventArgs
	{
		public IList<EndpointEventArgs> Items { get; set; }

		public bool IsOperationFailed 
		{
			get
			{
				return Items[Items.Count - 1].IsOperationCompleteFailed;
			}
		}
	}

	public class UserSearchEventArgs
		: EndpointEventArgs
	{
		public UserSearchEventArgs(UccUserSearchQueryEvent eventData)
			: base(eventData)
		{
			OperationId = eventData.OriginalOperationContext.OperationId;
			MoreAvailable = eventData.MoreAvailable;
			
			Results = new SearchUserRecord[eventData.Results.Count];
			for(int i=0; i<eventData.Results.Count; i++)
			{
				var record = eventData.Results[i + 1] as IUccUserSearchResultRecord;

				Results[i] = new SearchUserRecord();

				try { Results[i].Uri = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_URI); }
				catch (COMException) { }

				try { Results[i].DisplayName = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_DISPLAYNAME); }
				catch (COMException) { }

				try { Results[i].Title = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_TITLE); }
				catch (COMException) { }

				try { Results[i].Office = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_OFFICE); }
				catch (COMException) { }

				try { Results[i].Phone = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_PHONE); }
				catch (COMException) { }

				try { Results[i].Company = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_COMPANY); }
				catch (COMException) { }

				try { Results[i].City = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_CITY); }
				catch (COMException) { }

				try { Results[i].State = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_STATE); }
				catch (COMException) { }

				try { Results[i].Country = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_COUNTRY); }
				catch (COMException) { }

				try { Results[i].Email = record.get_Value(UCC_USER_SEARCH_COLUMN.UCCUSC_EMAIL); }
				catch (COMException) { }
			}
		}

		public int OperationId { get; private set; }
		public bool MoreAvailable { get; private set; }
		public SearchUserRecord[] Results { get; private set; }
	}
}
