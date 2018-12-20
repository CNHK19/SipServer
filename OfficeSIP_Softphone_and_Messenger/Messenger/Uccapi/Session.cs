// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;
using System.ComponentModel;

namespace Uccapi
{
	public enum SessionType
	{
		ImSession,
		AvSession
	}

	abstract class Session
		: UccapiBase
		, ISession
		, _IUccSessionEvents
		, _IUccSessionParticipantCollectionEvents
		, _IUccSessionParticipantEvents
	{
		private IUccSession uccSession;

		private const int ContextParticipantUri = 1;

		public delegate void DestroyEventHandler(Session session);
		public DestroyEventHandler DestroyEvent { get; set; }

		public Session(IPresentity selfPresentity)
		{
			this.PartipantLogs = new ObservableCollection<ParticipantLog>();
			this.PartipantLogs.Add(new ParticipantLog(selfPresentity, this.PartipantLogs.Count));
		}

	
		#region UccSession

		public IUccSession UccSession
		{
			get
			{
				return this.uccSession;
			}
			set
			{
				if (this.uccSession != null)
					this.DetachUccSession(true);

				if (value != null)
					this.AttachUccSession(value);
			}
		}

		protected virtual void DetachUccSession(bool terminate)
		{
			if (this.uccSession != null)
			{
				ComEvents.UnadviseAll(this);
				if (terminate)
					this.uccSession.Terminate(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE, null);
				this.uccSession = null;

				foreach (ParticipantLog log in this.PartipantLogs)
					log.State = PartipantLogState.SessionTerminated;
			}
		}

		protected virtual void AttachUccSession(IUccSession uccSession)
		{
			foreach (IUccSessionParticipant uccParticipant in uccSession.Participants)
			{
				//if (this.FindParticipantLog(uccParticipant.Uri.Value) == null)
					//ComEvents.Advise<_IUccSessionParticipantEvents>(uccParticipant, this);
				this.OnParticipantAdded(uccSession, uccParticipant);
				this.GetPartipantLog(uccParticipant).SetState(uccParticipant.State);
			}

			foreach (ParticipantLog log in this.PartipantLogs)
				if (log.IsLocal)
					log.State = PartipantLogState.Local;

			this.uccSession = uccSession;
			ComEvents.Advise<_IUccSessionEvents>(this.uccSession, this);
			ComEvents.Advise<_IUccSessionParticipantCollectionEvents>(this.uccSession, this);

			foreach (ParticipantLog log in this.PartipantLogs)
				if (log.IsLocal == false)
				{
					bool exist = false;
					foreach (IUccSessionParticipant uccParticipant in uccSession.Participants)
						if (Helpers.IsUriEqual(log.Uri, uccParticipant.Uri.Value))
							exist = true;

					if (exist == false)
						this.AddPartipant(log.Uri);
				}
		}

		#endregion

		#region SessionType

		public static UCC_SESSION_TYPE ConvertSessionType(SessionType sessionType)
		{
			switch (sessionType)
			{
				case SessionType.ImSession:
					return UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING;
				case SessionType.AvSession:
					return UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO;
			}

			throw new NotImplementedException();
		}


		#endregion

		#region ISession [ AddPartipant, Destroy, SessionType, GetDisplayNameOrAor, SelfPresentity ]

		public bool IsTerminated()
		{
			return this.UccSession == null;
		}

		public ParticipantLog AddPartipant(string uri)
		{
			uri = Helpers.CorrectUri(uri);
			var log = this.GetPartipantLog(uri);

			if (this.UccSession == null)
				log.State = PartipantLogState.SessionTerminated;
			else
			{
				//if (Helpers.IsUriEqual(uri, this.PartipantLogs[0].Uri))
				//{
				//    log.SetState(PartipantLogState.InvalidUri, @"URI is invalid, loopback is not allowed");
				//    return log;
				//}

				foreach (IUccSessionParticipant participant in this.UccSession.Participants)
					if (Helpers.IsUriEqual(uri, participant.Uri))
						return log;

				UccUri uccUri;
				if (Helpers.TryParseUri(uri, out uccUri))
				{
					log.State = PartipantLogState.AddBegin;

					IUccSessionParticipant participant = this.UccSession.CreateParticipant(uccUri, null);

					this.AddChannel(participant);

					UccOperationContext operationContext = new UccOperationContextClass();
					operationContext.Initialize(0, new UccContextClass());
					operationContext.Context.AddProperty(ContextParticipantUri, uri);

					this.UccSession.AddParticipant(participant, operationContext);
				}
				else
					log.SetState(PartipantLogState.InvalidUri, @"URI is invalid, e.g. sip:jdoe@officesip.local");
			}

			return log;
		}

		protected virtual void AddChannel(IUccSessionParticipant participant)
		{
		}

		public void Destroy()
		{
			this.DestroyEvent(this);
		}

		public abstract SessionType SessionType 
		{ 
			get; 
		}

		public string GetDisplayNameOrAor(string uri)
		{
			ParticipantLog log = this.FindParticipantLog(uri);
			if (log != null)
				return log.DisplayNameOrAor;

			return Helpers.GetAor(uri);
		}

		public IPresentity SelfPresentity 
		{
			get
			{
				return this.PartipantLogs[0].Presentity;
			}
		}

		#endregion


		#region ParticipantLog [ PartipantLogs, GetPartipantLog, HasRemoteConnectedParticipants, HasConnectingParticipants ]

		public ObservableCollection<ParticipantLog> PartipantLogs { get; private set; }

		public ObservableCollection<ParticipantLog> ParticipantLogs { get { return PartipantLogs; } }

		public bool HasRemoteConnectedParticipants()
		{
			foreach (ParticipantLog log in this.PartipantLogs)
				if (log.IsRemoteConnected)
					return true;
			return false;
		}

		public bool HasConnectingParticipants()
		{
			foreach (ParticipantLog log in this.PartipantLogs)
				if (log.IsConnecting)
					return true;
			return false;
		}

		public ParticipantLog FindParticipantLog(string uri)
		{
			foreach (ParticipantLog log in this.PartipantLogs)
			{
				if (Helpers.IsUriEqual(log.Uri, uri))
					return log;
			}

			return null;
		}

		protected ParticipantLog GetPartipantLog(string uri)
		{
			ParticipantLog log = this.FindParticipantLog(uri);

			if (log == null)
			{
				log = new ParticipantLog(uri, this.PartipantLogs.Count);
				this.PartipantLogs.Add(log);
			}

			return log;
		}

		protected ParticipantLog GetPartipantLog(IUccSessionParticipant participant)
		{
			return this.GetPartipantLog(participant.Uri.Value);
		}

		#endregion


		#region _IUccSessionEvents [ OnAddParticipant, OnRemoveParticipant, OnTerminate ]

		void _IUccSessionEvents.OnAddParticipant(IUccSession eventSource, IUccOperationProgressEvent eventData)
		{
			ParticipantLog log = this.GetPartipantLog(GetParticipantUri(eventData.OriginalOperationContext.Context));

			if (Helpers.IsOperationCompleteFailed(eventData))
				log.SetState(PartipantLogState.AddFailed, eventData.StatusCode);
			if (Helpers.IsOperationCompleteOk(eventData))
				log.State = PartipantLogState.AddSuccess;
		}

		void _IUccSessionEvents.OnRemoveParticipant(IUccSession eventSource, IUccOperationProgressEvent eventData)
		{
			ParticipantLog log = this.GetPartipantLog(GetParticipantUri(eventData.OriginalOperationContext.Context));

			if (Helpers.IsOperationCompleteFailed(eventData))
				log.SetState(PartipantLogState.RemoveFailed, eventData.StatusCode);
			if (Helpers.IsOperationCompleteOk(eventData))
				log.State = PartipantLogState.RemoveSuccess;
		}

		void _IUccSessionEvents.OnTerminate(IUccSession eventSource, IUccOperationProgressEvent eventData)
		{
			this.DetachUccSession(false);
		}

		private string GetParticipantUri(UccContext context)
		{
			if (context.IsPropertySet(ContextParticipantUri))
				return context.get_Property(ContextParticipantUri).StringValue;

			throw new InvalidOperationException("Session.cs::GetParticipantUri(UccContext context)");
		}

		#endregion


		#region _IUccSessionParticipantCollectionEvents [ OnParticipantRemoved, OnParticipantAdded ]

		protected virtual void OnParticipantRemoved(IUccSession eventSource, IUccSessionParticipant participant)
		{
			if (participant.IsLocal == false)
				ComEvents.Unadvise<_IUccSessionParticipantEvents>(participant, this);
		}

		protected virtual void OnParticipantAdded(IUccSession eventSource, IUccSessionParticipant participant)
		{
			if (participant.IsLocal == false)
				ComEvents.Advise<_IUccSessionParticipantEvents>(participant, this);
		}

		void _IUccSessionParticipantCollectionEvents.OnParticipantRemoved(IUccSession eventSource, UccSessionParticipantCollectionEvent eventData)
		{
			this.OnParticipantRemoved(eventSource, eventData.Participant);
		}

		void _IUccSessionParticipantCollectionEvents.OnParticipantAdded(IUccSession eventSource, UccSessionParticipantCollectionEvent eventData)
		{
			this.OnParticipantAdded(eventSource, eventData.Participant);
		}

		#endregion


		#region _IUccSessionParticipantEvents [ OnStateChanged, OnAddParticipantEndpoint, OnRemoveParticipantEndpoint ]

		void _IUccSessionParticipantEvents.OnStateChanged(IUccSessionParticipant eventSource, UccSessionParticipantStateChangedEvent eventData)
		{
			this.GetPartipantLog(eventSource).SetState(eventData.NewState);
		}

		void _IUccSessionParticipantEvents.OnAddParticipantEndpoint(IUccSessionParticipant pEventSource, IUccOperationProgressEvent pEventData)
		{
		}

		void _IUccSessionParticipantEvents.OnRemoveParticipantEndpoint(IUccSessionParticipant pEventSource, IUccOperationProgressEvent pEventData)
		{
		}

		#endregion
	}
}
