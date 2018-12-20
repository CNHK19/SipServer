// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	class AvSession
		: Session
		, IAvSession
		, _IUccMediaChannelCollectionEvents
		, _IUccMediaChannelEvents
	{
		private bool isVideoEnabled;
		private int videoInChannelCount;
		private int videoOutChannelCount;
		private int audioInChannelCount;
		private int audioOutChannelCount;

		public AvSession(IPresentity selfPresentity)
			: base(selfPresentity)
		{
			this.isVideoEnabled = false;
		}

		public override SessionType SessionType
		{
			get { return SessionType.AvSession; }
		}

		#region UccSession

		protected override void DetachUccSession(bool terminate)
		{
			base.DetachUccSession(terminate);

			this.VideoInChannelCount = 0;
			this.VideoOutChannelCount = 0;
			this.AudioInChannelCount = 0;
			this.AudioOutChannelCount = 0;
		}

		protected override void AttachUccSession(IUccSession uccSession)
		{
			base.AttachUccSession(uccSession);
		}

		#endregion

		#region FindIVideoWindow, EnableVideo

		private IVideoWindow FindIVideoWindow(UCC_MEDIA_DIRECTIONS mediaDirection)
		{
			if (this.UccSession != null && this.UccSession.Participants != null)
			{
				foreach (IUccAudioVideoSessionParticipant participant in this.UccSession.Participants)
				{
					IUccVideoMediaChannel channel = this.FindMediaChannel<IUccVideoMediaChannel>(participant);

					if (channel != null)
					{
						IVideoWindow ivideoWindow = channel.get_VideoWindow(mediaDirection);

						if (ivideoWindow != null)
							return ivideoWindow;
					}
				}
			}

			return null;
		}

		public void EnableVideo()
		{
			if (!this.isVideoEnabled)
			{
				this.isVideoEnabled = true;
			}
		}

		#endregion

		#region VideoWindow

		private VideoWindowHost videoWindow;

		public VideoWindowHost VideoWindow
		{
			get
			{
				if (videoWindow == null)
				{
					videoWindow = new VideoWindowHost()
					{
						VideoWindow1 = FindIVideoWindow(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE),
						VideoWindow2 = FindIVideoWindow(UCC_MEDIA_DIRECTIONS.UCCMD_SEND),
					};
				}

				return videoWindow;
			}
		}

		#endregion

		#region VideoInChannelCount, VideoOutChannelCount, AudioInChannelCount, AudioOutChannelCount

		public int VideoInChannelCount
		{
			get { return videoInChannelCount; }
			private set
			{
				if (videoInChannelCount != value)
				{
					if (videoWindow != null)
						videoWindow.VideoWindow1 = FindIVideoWindow(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE);

					videoInChannelCount = value;
					OnPropertyChanged(PropertyName.VideoInChannelCount);
				}
			}
		}

		public int VideoOutChannelCount
		{
			get { return videoOutChannelCount; }
			private set
			{
				if (videoOutChannelCount != value)
				{
					if (videoWindow != null)
						videoWindow.VideoWindow2 = FindIVideoWindow(UCC_MEDIA_DIRECTIONS.UCCMD_SEND);

					videoOutChannelCount = value;
					OnPropertyChanged(PropertyName.VideoOutChannelCount);
				}
			}
		}

		public int AudioInChannelCount
		{
			get { return audioInChannelCount; }
			private set
			{
				if (audioInChannelCount != value)
				{
					audioInChannelCount = value;
					OnPropertyChanged(PropertyName.AudioInChannelCount);
				}
			}
		}

		public int AudioOutChannelCount
		{
			get { return audioOutChannelCount; }
			private set
			{
				if (audioOutChannelCount != value)
				{
					audioOutChannelCount = value;
					OnPropertyChanged(PropertyName.AudioOutChannelCount);
				}
			}
		}

		#endregion

		#region SendDtmf

		public bool SendDtmf(char dtmf)
		{
			try
			{
				foreach (var log in ParticipantLogs)
					if (log.IsRemoteConnected)
					{
						var participant = FindSessionParticipant<IUccAudioVideoSessionParticipant>(log.Uri);

						if (participant != null)
						{
							var channel = FindMediaChannel<IUccAudioMediaChannel>(participant);

							if (channel != null)
							{
								UCC_DTMF? uccDtmf = Convert(dtmf);
								if (uccDtmf != null)
								{
									channel.SendDtmf((UCC_DTMF)uccDtmf);
									return true;
								}
							}
						}
						break;
					}
			}
			catch(COMException)
			{
			}

			return false;
		}

		#endregion

		#region Helpers

		protected IUccMediaChannel FindMediaChannel(IUccAudioVideoSessionParticipant participant, UCC_MEDIA_TYPES mediaType)
		{
			if (participant != null)
			{
				foreach (IUccMediaChannel channel in participant.Channels)
					if ((channel.MediaType & mediaType) == mediaType)
						return channel;
			}

			return null;
		}

		protected T FindMediaChannel<T>(IUccAudioVideoSessionParticipant participant)
		{
			if (typeof(T) == typeof(IUccVideoMediaChannel))
				return (T)this.FindMediaChannel(participant, UCC_MEDIA_TYPES.UCCMT_VIDEO);

			if (typeof(T) == typeof(IUccAudioMediaChannel))
				return (T)this.FindMediaChannel(participant, UCC_MEDIA_TYPES.UCCMT_AUDIO);

			return default(T);
		}

		protected T FindSessionParticipant<T>(string uri)
		{
			if (this.UccSession != null)
			{
				foreach (IUccSessionParticipant participant in this.UccSession.Participants)
					if (Helpers.IsUriEqual(uri, participant.Uri))
					{
						if (participant is T)
							return (T)participant;
						return default(T);
					}	
			}

			return default(T);
		}

		private int GetMediaDeltaCount(UCC_MEDIA_DIRECTIONS direction, UCC_MEDIA_DIRECTIONS oldMediaDirection, UCC_MEDIA_DIRECTIONS newMediaDirection)
		{
			return (((newMediaDirection & direction) != 0) ? 1 : 0) +
				(((oldMediaDirection & direction) != 0) ? -1 : 0);
		}

		private UCC_DTMF? Convert(char dtmf)
		{
			switch (dtmf)
			{
				case '0':
					return UCC_DTMF.UCCDTMF_0;
				case '1':
					return UCC_DTMF.UCCDTMF_1;
				case '2':
					return UCC_DTMF.UCCDTMF_2;
				case '3':
					return UCC_DTMF.UCCDTMF_3;
				case '4':
					return UCC_DTMF.UCCDTMF_4;
				case '5':
					return UCC_DTMF.UCCDTMF_5;
				case '6':
					return UCC_DTMF.UCCDTMF_6;
				case '7':
					return UCC_DTMF.UCCDTMF_7;
				case '8':
					return UCC_DTMF.UCCDTMF_8;
				case '9':
					return UCC_DTMF.UCCDTMF_9;
				case 'A':
				case 'a':
					return UCC_DTMF.UCCDTMF_A;
				case 'B':
				case 'b':
					return UCC_DTMF.UCCDTMF_B;
				case 'C':
				case 'c':
					return UCC_DTMF.UCCDTMF_C;
				case 'D':
				case 'd':
					return UCC_DTMF.UCCDTMF_D;
				case '*':
					return UCC_DTMF.UCCDTMF_STAR;
				case '#':
					return UCC_DTMF.UCCDTMF_POUND;
				case 'F':
				case 'f':
					return UCC_DTMF.UCCDTMF_FLASH;
			}

			return null;
		}

		#endregion

		#region Session [ virtual AddChannel ]

		protected override void AddChannel(IUccSessionParticipant participant)
		{
			IUccAudioVideoSessionParticipant avParticipant = participant as IUccAudioVideoSessionParticipant;

			IUccMediaChannel audioChannel = avParticipant.CreateChannel(UCC_MEDIA_TYPES.UCCMT_AUDIO, null);
			audioChannel.PreferredMedia = (int)(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE | UCC_MEDIA_DIRECTIONS.UCCMD_SEND);

			avParticipant.AddChannel(audioChannel);

			if (this.isVideoEnabled)
			{
				IUccMediaChannel videoChannel = avParticipant.CreateChannel(UCC_MEDIA_TYPES.UCCMT_VIDEO, null);
				videoChannel.PreferredMedia = (int)(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE | UCC_MEDIA_DIRECTIONS.UCCMD_SEND);

				avParticipant.AddChannel(videoChannel);
			}
		}

		#endregion

		#region _IUccSessionParticipantCollectionEvents [ OnParticipantRemoved, OnParticipantAdded ]

		protected override void OnParticipantRemoved(IUccSession eventSource, IUccSessionParticipant participant)
		{
			base.OnParticipantRemoved(eventSource, participant);

			ComEvents.Unadvise<_IUccMediaChannelCollectionEvents>(participant, this);
		}

		protected override void OnParticipantAdded(IUccSession eventSource, IUccSessionParticipant participant)
		{
			base.OnParticipantAdded(eventSource, participant);

			IUccAudioVideoSessionParticipant avParticipant = participant as IUccAudioVideoSessionParticipant;

			foreach (IUccMediaChannel channel in avParticipant.Channels)
				this.OnChannelAdded(avParticipant, channel);

			ComEvents.Advise<_IUccMediaChannelCollectionEvents>(avParticipant, this);
		}

		#endregion

		#region _IUccMediaChannelCollectionEvents [ OnChannelAdded, OnChannelRemoved ]

		protected virtual void OnChannelAdded(IUccAudioVideoSessionParticipant eventSource, IUccMediaChannel channel)
		{
			this.OnNegotiatedMediaChanged(channel, 0, (UCC_MEDIA_DIRECTIONS)channel.NegotiatedMedia);

			ComEvents.Advise<_IUccMediaChannelEvents>(channel, this);
		}

		void _IUccMediaChannelCollectionEvents.OnChannelAdded(IUccAudioVideoSessionParticipant eventSource, IUccMediaChannelCollectionEvent eventData)
		{
			this.OnChannelAdded(eventSource, eventData.MediaChannel);
		}

		void _IUccMediaChannelCollectionEvents.OnChannelRemoved(IUccAudioVideoSessionParticipant eventSource, IUccMediaChannelCollectionEvent eventData)
		{
			ComEvents.Unadvise<_IUccMediaChannelEvents>(eventData.MediaChannel, this);
		}

		#endregion

		#region _IUccMediaChannelEvents [ OnNegotiatedMediaChanged ]

		protected virtual void OnNegotiatedMediaChanged(IUccMediaChannel eventSource, UCC_MEDIA_DIRECTIONS oldMediaDirection, UCC_MEDIA_DIRECTIONS newMediaDirection)
		{
			if ((eventSource.MediaType & UCC_MEDIA_TYPES.UCCMT_VIDEO) == UCC_MEDIA_TYPES.UCCMT_VIDEO)
			{
				this.VideoOutChannelCount += 
					GetMediaDeltaCount(UCC_MEDIA_DIRECTIONS.UCCMD_SEND, oldMediaDirection, newMediaDirection);
				this.VideoInChannelCount += 
					GetMediaDeltaCount(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE, oldMediaDirection, newMediaDirection);
			}

			if ((eventSource.MediaType & UCC_MEDIA_TYPES.UCCMT_AUDIO) == UCC_MEDIA_TYPES.UCCMT_AUDIO)
			{
				this.AudioOutChannelCount +=
					GetMediaDeltaCount(UCC_MEDIA_DIRECTIONS.UCCMD_SEND, oldMediaDirection, newMediaDirection);
				this.AudioInChannelCount +=
					GetMediaDeltaCount(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE, oldMediaDirection, newMediaDirection);
			}
		}

		void _IUccMediaChannelEvents.OnNegotiatedMediaChanged(IUccMediaChannel eventSource, UccMediaChannelEvent eventData)
		{
			this.OnNegotiatedMediaChanged(eventSource, (UCC_MEDIA_DIRECTIONS)eventData.OldMediaDirection, (UCC_MEDIA_DIRECTIONS)eventData.NewMediaDirection);
		}

		#endregion
	}
}
