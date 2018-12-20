// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	public enum PartipantLogState
	{
		Null,
		Local,
		InvalidUri,

		AddBegin,
		AddFailed,
		AddSuccess,
		
		RemoveFailed,
		RemoveSuccess,
		
		SessionTerminated,
		
		// UCC_SESSION_ENTITY_STATE
		Connecting,
		Connected,
		Disconnected,
		Disconnecting,
		Idle,
	}

	public class ParticipantLog
		: UccapiBase
	{
		private PartipantLogState state = PartipantLogState.Null;
		private bool isComposing;
		private int videoInChannelCount;
		private int videoOutChannelCount;

		public ParticipantLog(string uri, int id)
		{
			this.Uri = uri;
			this.IsLocal = false;
			this.Id = id;

			this.ResetProperties();
		}

		public ParticipantLog(IPresentity selfPresentity, int id)
		{
			this.Uri = selfPresentity.Uri;
			this.Presentity = selfPresentity;
			this.IsLocal = true;
			this.Id = id;

			this.ResetProperties();
		}

		public string Uri { get; private set; }
		public int Id { get; private set; }

		public DateTime Time { get; private set; }
		public string LastError { get; private set; }
		public bool IsLocal { get; private set; }

		#region Session Realayted Properties [ IsComposing, VideoInChannelCount, VideoOutChannelCount ]

		public void ResetProperties()
		{
			switch (this.State)
			{
				case PartipantLogState.Null:
				case PartipantLogState.Local:
				case PartipantLogState.InvalidUri:
				case PartipantLogState.Disconnected:
				case PartipantLogState.AddFailed:
				case PartipantLogState.RemoveSuccess:
				case PartipantLogState.RemoveFailed:
				case PartipantLogState.SessionTerminated:
					this.IsComposing = false;
					this.VideoInChannelCount = 0;
					this.VideoOutChannelCount = 0;
					break;
			}
		}
		
		public bool IsComposing
		{
			get
			{
				return this.isComposing;
			}
			set
			{
				if (this.isComposing != value)
				{
					this.isComposing = value;
					this.OnPropertyChanged(PropertyName.IsComposing);
				}
			}
		}

		public int VideoInChannelCount 
		{
			get
			{
				return this.videoInChannelCount;
			}
			set
			{
				if (this.videoInChannelCount != value)
				{
					this.videoInChannelCount = value;
					this.OnPropertyChanged(PropertyName.VideoInChannelCount);
				}
			}
		}

		public int VideoOutChannelCount 
		{
			get
			{
				return this.videoOutChannelCount;
			}
			set
			{
				if (this.videoOutChannelCount != value)
				{
					this.videoOutChannelCount = value;
					this.OnPropertyChanged(PropertyName.VideoOutChannelCount);
				}
			}
		}

		#endregion

		public bool IsConnecting
		{
			get
			{
				return this.state == PartipantLogState.AddBegin 
					|| this.state == PartipantLogState.Connecting;
			}
		}

		public bool IsRemoteConnected
		{
			get
			{
				return (this.state == PartipantLogState.AddSuccess
					|| this.state == PartipantLogState.Connected) && this.IsLocal == false;
			}
		}

		public PartipantLogState State
		{
			set
			{
				this.state = value;
				this.Time = DateTime.Now;

				this.OnPropertyChanged(PropertyName.State);
				this.OnPropertyChanged(PropertyName.Time);
				if (this.IsLocal == false)
					this.OnPropertyChanged(PropertyName.IsRemoteConnected);

				this.ResetProperties();
			}
			get
			{
				return this.state;
			}
		}

		public void SetState(PartipantLogState state)
		{
			this.State = state;
		}

		public void SetState(PartipantLogState state, string error)
		{
			this.State = state;
			this.LastError = error;

			this.OnPropertyChanged(@"LastError");
		}

		public void SetState(PartipantLogState state, int error)
		{
			SetState(state, Errors.ToString(error));
		}

		public void SetState(UCC_SESSION_ENTITY_STATE uccState)
		{
			this.State = ConvertState(uccState);
		}

		private static PartipantLogState ConvertState(UCC_SESSION_ENTITY_STATE uccState)
		{
			switch (uccState)
			{
				case UCC_SESSION_ENTITY_STATE.UCCSES_CONNECTED:
					return PartipantLogState.Connected;
				case UCC_SESSION_ENTITY_STATE.UCCSES_CONNECTING:
					return PartipantLogState.Connecting;
				case UCC_SESSION_ENTITY_STATE.UCCSES_DISCONNECTED:
					return PartipantLogState.Disconnected;
				case UCC_SESSION_ENTITY_STATE.UCCSES_DISCONNECTING:
					return PartipantLogState.Disconnecting;
				case UCC_SESSION_ENTITY_STATE.UCCSES_IDLE:
					return PartipantLogState.Idle;
			}
			return PartipantLogState.Null;
		}

		#region Presentity

		private IPresentity presentity;

		public IPresentity Presentity
		{
			get
			{
				return this.presentity;
			}
			set
			{
				if (this.presentity != null)
					this.presentity.PropertyChanged -= Presentity_PropertyChanged;

				this.presentity = value;

				if (this.presentity != null)
					this.presentity.PropertyChanged += Presentity_PropertyChanged;
			}
		}

		private void Presentity_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == PropertyName.DisplayNameOrAor || e.PropertyName == PropertyName.Availability)
				this.OnPropertyChanged(e);
		}

		public string DisplayNameOrAor 
		{
			get
			{
				if (this.presentity != null)
					return this.presentity.DisplayNameOrAor;
				return Helpers.GetAor(this.Uri);
			}
		}

		public AvailabilityValues Availability 
		{
			get
			{
				if (this.presentity != null)
					return this.presentity.Availability;
				return AvailabilityValues.Unknown;
			}
		}

		#endregion
	}
}
