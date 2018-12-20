// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	public class Endpoint
		: UccapiBase
		, _IUccPlatformEvents
		, _IUccEndpointEvents
		, _IUccServerSignalingSettingsEvents
		, _IUccSubscriptionEvents
		//, _IUccApplicationSessionParticipantEvents
		//, _IUccSessionDescriptionEvaluator
		, _IUccSessionManagerEvents
		// , _IUccSignalingChannelEvents
		, _IUccPublicationEvent
		, _IUccMediaEndpointEvents
		, _IUccUserSearchQueryEvents
		, INotifyPropertyChanged
	{
		public event EventHandler<EndpointEventArgss> Enabled;
		public event EventHandler<EndpointEventArgs> Disabled;
		public event EventHandler<UserSearchEventArgs> UserSearchFinished;

		public event EventHandler<AvInvite> IncommingAvSession;

		private IUccPlatform platform;
		private IUccTraceSettings traceSettings;
		private IUccEndpoint endpoint;
		private UccUri uri;
		private IUccSubscription subscription;
		private AvailabilityValues loginAvailability;
		private bool isEnabled;
		private bool isDisabled;
		private EndpointStatus status;
		private IUccSubscription selfSubscription;
		private Presentity selfPresentityMonitor;
		private SelfPresentity selfPresentity;
		private PresentitiesCollection presentities;
		private IUccSessionManager sessionManager;
		private IUccServerSignalingSettings signalingSettings;
		private IUccMediaEndpointSettings mediaEndpointSettings;
		private IUccUserSearchManager userSearchManager;
		private bool disableImSessions;

		private const int ContextInitialPublication = 1;

		public string Uri
		{
			get
			{
				if (this.uri != null)
					return this.uri.Value.ToString();
				return null;
			}
		}

		#region Construct; Initialize; Cleanup

		public Endpoint(Dispatcher dispatcher)
			: base(dispatcher)
		{
			this.presentities = new PresentitiesCollection(true);
			this.presentities.PostCollectionChanged += Presenties_CollectionChanged;
			this.presentities.CollectionChanged += Presenties_CollectionChanged2;

			this.Sessions = new ObservableCollection<ISession>();
			this.Sessions.CollectionChanged += Sessions_CollectionChanged;

			this.AvInvites = new ObservableCollection<AvInvite>();

			this.selfPresentity = new SelfPresentity();
			this.selfPresentity.PropertyChanged += SelfPresentity_PropertyChanged;

			this.selfPresentityMonitor = new Presentity();
			this.selfPresentityMonitor.PropertyChanged += SelfPresentityMonitor_PropertyChanged;

			this.Cleanup();
		}

		~Endpoint()
		{
			this.presentities.PostCollectionChanged -= Presenties_CollectionChanged;
			this.presentities.CollectionChanged -= Presenties_CollectionChanged2;

			this.Sessions.CollectionChanged -= Sessions_CollectionChanged;

			this.selfPresentity.PropertyChanged -= SelfPresentity_PropertyChanged;

			this.selfPresentityMonitor.PropertyChanged -= SelfPresentityMonitor_PropertyChanged;

			this.Sessions.Clear();
			//	this.AvInvites.Clear();

			this.Cleanup();
		}

		public void Initialize(string applicationName)
		{
			this.platform = new UccPlatform();
			this.platform.Initialize(applicationName, null);

			this.traceSettings = this.platform as IUccTraceSettings;
		}

		public void Cleanup()
		{
			this.CleanupEndpoint();

			this.traceSettings = null;
			if (this.platform != null)
				try { this.platform.Shutdown(null); }
				catch (InvalidComObjectException) { }
			this.platform = null;
		}

		public bool IsTracingEnabled
		{
			get
			{
				return this.traceSettings.TracingEnabled;
			}
			set
			{
				if (this.traceSettings.TracingEnabled != value)
				{
					if (value)
						this.traceSettings.EnableTracing();
					else
						this.traceSettings.DisableTracing();
				}
			}
		}

		private void InitializeEndpoint(EndpointConfiguration configuration, AuthenticationMode authMode)
		{
			this.CleanupEndpoint();


			// set vars
			if (Helpers.TryParseUri(Helpers.CorrectUri(configuration.Uri), out this.uri) == false)
				throw new Exception("Sign-in address is not valid Uri");
			this.selfPresentity.SetUri(this.uri.Value);
			this.selfPresentityMonitor.SetUri(this.uri);
			this.disableImSessions = configuration.DisableImSessions;

			// platform events
			ComEvents.Advise<_IUccPlatformEvents>(this.platform, this);


			// create endpoint
			this.endpoint = this.platform.CreateEndpoint(UCC_ENDPOINT_TYPE.UCCET_PRINCIPAL_SERVER_BASED, this.uri, configuration.EndpoindIdString, null);
			ComEvents.Advise<_IUccEndpointEvents>(this.endpoint, this);
			ComEvents.Advise<_IUccSessionManagerEvents>(this.endpoint, this);



			// server signaling settings
			this.signalingSettings = (IUccServerSignalingSettings)this.endpoint;
			ComEvents.Advise<_IUccServerSignalingSettingsEvents>(this.signalingSettings, this);

			this.signalingSettings.AllowedAuthenticationModes = (int)authMode.ToUccAuthenticationMode();

			UccCredential credential = authMode.IsDefaultCreditals() ?
				this.signalingSettings.CredentialCache.DefaultCredential :
				this.signalingSettings.CredentialCache.CreateCredential(configuration.Username, configuration.Password, null);

			this.signalingSettings.CredentialCache.SetCredential(configuration.Realm, credential);


			// media endpoint settings
			this.mediaEndpointSettings = (IUccMediaEndpointSettings)this.endpoint;
			ComEvents.Advise<_IUccMediaEndpointEvents>(this.endpoint, this);


			// create session manager
			this.sessionManager = (IUccSessionManager)this.endpoint;
			ComEvents.Advise<_IUccSessionManagerEvents>(this.sessionManager, this);

			// create user search manager
			this.userSearchManager = (IUccUserSearchManager)this.endpoint;
		}

		private void SetSignalingServer(SignalingServer server)
		{
			// Uccapi.chm: UCCTM_UDP - A flag indicating that the transport mode is UDP. This mode is not currently supported.
			if (server.TransportMode == TransportMode.Udp)
				throw new Exception("UDP transport is not currently supported");
			this.signalingSettings.Server = this.signalingSettings.CreateSignalingServer(server.ServerAddress, Helpers.Convert(server.TransportMode));
		}

		public void CleanupEndpoint()
		{
			foreach (var invite in this.AvInvites)
				invite.Decline();

			foreach (Session session in this.Sessions)
				session.UccSession = null;

			this.IsDisabled = true;
			this.SelfPresentity.SetAvailability(AvailabilityValues.Unknown);

			ComEvents.UnadviseAll(this);

			this.selfPresentityMonitor.DestroyUccPresentity();
			if (this.selfSubscription != null && this.selfSubscription.Presentities.Count > 0)
				this.selfSubscription.Unsubscribe(null);
			this.selfSubscription = null;

			foreach (Presentity presentity in this.Presentities)
				presentity.DestroyUccPresentity();
			if (this.subscription != null && this.subscription.Presentities.Count > 0)
				this.subscription.Unsubscribe(null);
			this.subscription = null;

			this.signalingSettings = null;
			this.mediaEndpointSettings = null;
			this.userSearchManager = null;

			this.endpoint = null;
		}

		#endregion

		#region BeginLogin, BeginLogout

		private List<EndpointEventArgs> enableErrors;
		private EndpointConfiguration configuration;

		public void BeginLogin(EndpointConfiguration configuration, AvailabilityValues loginAvailability)
		{
			enableErrors = new List<EndpointEventArgs>();

			this.configuration = configuration;
			this.loginAvailability = loginAvailability;

			InternalBeginLogin();
		}

		private void InternalBeginLogin()
		{
			try
			{
				this.InitializeEndpoint(configuration, configuration.AuthenticationModes[enableErrors.Count]);

				this.IsDisabled = false;
				this.Status = EndpointStatus.Enabling;

				if (configuration.SignalingServer != null)
				{
					this.SetSignalingServer(configuration.SignalingServer);
					this.endpoint.Enable(null);
				}
				else
				{
					this.signalingSettings.FindServer(Helpers.GetDomain(configuration.Uri), null);
				}
			}
			catch (COMException ex)
			{
				this.IsDisabled = true;
				this.OnEnabled(new EndpointEventArgs(ex));
			}
			catch (Exception ex)
			{
				this.IsDisabled = true;
				this.OnEnabled(new EndpointEventArgs(ex));
			}
		}

		public void BeginLogout()
		{
			try
			{
				this.SelfUnsubscribe();
				this.Unsubscribe();

				this.IsEnabled = false;
				this.Status = EndpointStatus.Disabling;

				if (this.endpoint != null)
					this.endpoint.Disable(null);
			}
			catch (COMException ex)
			{
				OnDisabled(new EndpointEventArgs(ex));
			}
			catch (Exception ex)
			{
				OnDisabled(new EndpointEventArgs(ex));
			}
		}

		#endregion

		#region IsEnabled; IsDisabled

		public bool IsEnabled
		{
			get { return this.isEnabled; }
			private set
			{
				if (this.isEnabled != value)
				{
					this.isEnabled = value;
					OnPropertyChanged(@"IsEnabled");
				}
				if (value)
				{
					Status = EndpointStatus.Enabled;

					if (this.isDisabled != false)
					{
						this.isDisabled = false;
						OnPropertyChanged(@"IsDisabled");
					}
				}
			}
		}

		public bool IsDisabled
		{
			get { return this.isDisabled; }
			private set
			{
				if (this.isDisabled != value)
				{
					this.isDisabled = value;
					OnPropertyChanged(@"IsDisabled");
				}
				if (value)
				{
					Status = EndpointStatus.Disabled;

					if (this.isEnabled != false)
					{
						this.isEnabled = false;
						OnPropertyChanged(@"IsEnabled");
					}
				}
			}
		}

		public EndpointStatus Status
		{
			get { return status; }
			private set
			{
				if (status != value)
				{
					status = value;
					OnPropertyChanged(@"Status");
				}
			}
		}

		#endregion

		#region Subscription

		public IPresentitiesCollection Presentities
		{
			get
			{
				return this.presentities;
			}
			set
			{
				this.presentities.Clear();
				if (value != null)
					this.presentities.AddRange(value);
			}
		}

		public bool IsValidUri(string uri)
		{
			return !Helpers.IsInvalidUri(Helpers.CorrectUri(uri));
		}

		public IPresentity CreatePresentity(string uri, string group)
		{
			return new Presentity() { Uri = uri, Group = group };
		}

		private void CreateSubscription()
		{
			IUccSubscriptionManager subscriptionMananager = (IUccSubscriptionManager)this.endpoint;

			this.subscription = subscriptionMananager.CreateSubscription(null);

			this.subscription.AddCategoryName(CategoryName.ContactCard);
			this.subscription.AddCategoryName(CategoryName.State);
			this.subscription.AddCategoryName(CategoryName.UserProperties);

			Uccapi.ComEvents.Advise<_IUccSubscriptionEvents>(this.subscription, this);
		}

		private void Unsubscribe()
		{
			foreach (Presentity presentity in this.Presentities)
				presentity.DestroyUccPresentity();

			if (this.subscription != null && this.subscription.Presentities.Count > 0)
				this.subscription.Unsubscribe(null);
		}

		private void Presenties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs a)
		{
			if (this.IsEnabled)
			{
				switch (a.Action)
				{
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Remove:
					case NotifyCollectionChangedAction.Reset:
					case NotifyCollectionChangedAction.Replace:
						this.UpdateSubscription(a.NewItems, a.OldItems);
						break;
					case NotifyCollectionChangedAction.Move:
						break;
				}
			}
		}

		private void UpdateSubscription(IEnumerable subscribe, IEnumerable unsubscribe)
		{
			bool subscriptionChanged = false;

			if (unsubscribe != null)
			{
				foreach (Presentity presentity in unsubscribe)
					if (presentity.UccPresentity != null)
					{
						this.subscription.RemovePresentity(presentity.DestroyUccPresentity());
						subscriptionChanged = true;
					}
			}

			if (subscribe != null)
			{
				foreach (Presentity presentity in subscribe)
					if (Helpers.IsUriEqual(presentity.Uri, this.uri) == false)
					{
						if (presentity.CreateUccPresentity(this.subscription))
						{
							this.subscription.AddPresentity(presentity.UccPresentity);
							subscriptionChanged = true;
						}
					}
					else
					{
						presentity.AttachPresentity(this.selfPresentityMonitor);
					}
			}

			if (subscriptionChanged)
				this.subscription.Subscribe(null);
		}

		private void Subscribe(IEnumerable presentities)
		{
			this.UpdateSubscription(presentities, null);
		}

		#endregion Subscription

		#region Publish

		public ISelfPresentity SelfPresentity
		{
			get
			{
				return this.selfPresentity;
			}
		}

		private void SelfPresentity_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.IsEnabled)
			{
				if (e.PropertyName == PropertyName.Availability)
					this.Publish();
			}
		}

		private void SelfSubscribe()
		{
			IUccSubscriptionManager subscriptionMananager = (IUccSubscriptionManager)this.endpoint;

			this.selfSubscription = subscriptionMananager.CreateSubscription(null);

			Uccapi.ComEvents.Advise<_IUccSubscriptionEvents>(this.selfSubscription, this);

			////this.selfPresentityMonitor = new Presentity();
			////this.selfPresentityMonitor.PropertyChanged += SelfPresentityMonitor_PropertyChanged;
			//////this.selfPresentityMonitor.CreateUccPresentity(this.selfSubscription, this.uri);
			////////this.selfPresentityMonitor.SetUri(this.uri);
			this.selfPresentityMonitor.CreateUccPresentity(this.selfSubscription);

			this.selfSubscription.AddPresentity(this.selfPresentityMonitor.UccPresentity);

			this.selfSubscription.AddCategoryName(CategoryName.ContactCard);
			this.selfSubscription.AddCategoryName(CategoryName.State);
			this.selfSubscription.AddCategoryName(CategoryName.UserProperties);

			this.selfSubscription.Subscribe(null);
		}

		private void SelfUnsubscribe()
		{
			if (this.selfSubscription != null)
			{
				this.selfSubscription.RemovePresentity(this.selfPresentityMonitor.DestroyUccPresentity());
				this.selfSubscription.Subscribe(null);
			}
		}

		void SelfPresentityMonitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == PropertyName.Availability)
			{
				this.SelfPresentity.PropertyChanged -= SelfPresentity_PropertyChanged;
				this.SelfPresentity.SetAvailability(this.selfPresentityMonitor.Availability);
				this.SelfPresentity.PropertyChanged += SelfPresentity_PropertyChanged;
			}
		}

		private void Publish()
		{
			UccOperationContext operationContext = new UccOperationContextClass();
			operationContext.Initialize(0, new UccContextClass());

			IUccPublicationManager publicationManager = (IUccPublicationManager)this.endpoint;

			IUccCategoryInstance categoryInstance = null;
			//this.selfPresentity.CreatePublishableStateCategoryInstance();

			if (categoryInstance == null)
			{
				categoryInstance = publicationManager.CreatePublishableCategoryInstance(
					CategoryName.State, 2, 0, UCC_CATEGORY_INSTANCE_EXPIRE_TYPE.UCCCIET_DEVICE, 0);

				operationContext.Context.AddProperty(ContextInitialPublication, true);
			}

			categoryInstance.PublicationOperation = UCC_PUBLICATION_OPERATION_TYPE.UCCPOT_ADD;

			IUccPresenceStateInstance stateInstance = (IUccPresenceStateInstance)categoryInstance;
			stateInstance.Availability = (int)this.SelfPresentity.Availability;
			stateInstance.Type = UCC_PRESENCE_STATE_TYPE.UCCPST_USER_STATE;

			IUccPublication publication = publicationManager.CreatePublication();
			ComEvents.Advise<_IUccPublicationEvent>(publication, this);
			publication.AddPublishableCategoryInstance(categoryInstance);

			publication.Publish(operationContext);
		}

		#endregion Publish

		#region Sessions

		public ObservableCollection<ISession> Sessions { get; private set; }

		private IAvSession avSession1;

		public IAvSession AvSession1
		{
			get { return avSession1; }
			private set
			{
				if (avSession1 != value)
				{
					avSession1 = value;
					OnPropertyChanged(@"AvSession1");
				}
			}
		}

		private Session CreateSession(IUccSession uccSession)
		{
			Session session;

			switch (uccSession.Type)
			{
				case UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING:
					session = new ImSession(this.selfPresentityMonitor);
					break;
				case UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO:
					session = new AvSession(this.selfPresentityMonitor);
					break;
				default:
					throw new NotImplementedException();
			}

			session.PartipantLogs.CollectionChanged += PartipantLogs_CollectionChanged;
			session.DestroyEvent = new Session.DestroyEventHandler(DestroySession);
			session.UccSession = uccSession;

			this.Sessions.Add(session);

			return session;
		}

		public T CreateSession<T>()
			where T : class
		{
			if (typeof(T) == typeof(IImSession))
				return CreateSession(SessionType.ImSession) as T;

			if (typeof(T) == typeof(IAvSession))
				return CreateSession(SessionType.AvSession) as T;

			throw new Exception();
		}

		public ISession CreateSession(SessionType sessionType)
		{
			return this.CreateSession(
				this.sessionManager.CreateSession(Session.ConvertSessionType(sessionType), null)
				);
		}

		public void RestoreSession(ISession session)
		{
			if (session.IsTerminated())
			{
				//if (session.SessionType == SessionType.ImSession)
				(session as Session).UccSession = this.sessionManager.CreateSession(
					Session.ConvertSessionType(session.SessionType), null);
			}
		}

		public ISession FindSession(SessionType sessionType, string uri)
		{
			foreach (ISession session in this.Sessions)
			{
				if (session.SessionType == sessionType && session.PartipantLogs.Count == 2)
					if (Helpers.IsUriEqual(session.PartipantLogs[1].Uri, uri))
						return session;
			}

			return null;
		}

		private void DestroySession(Session session)
		{
			session.UccSession = null;
			session.PartipantLogs.CollectionChanged -= PartipantLogs_CollectionChanged;

			this.Sessions.Remove(session);
		}


		private void Sessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (AvSession1 != null && e.OldItems != null)
				foreach (Session session in e.OldItems)
					if (AvSession1 == session)
						AvSession1 = null;

			if (AvSession1 == null && e.NewItems != null)
				foreach (Session session in e.NewItems)
					if (session.SessionType == SessionType.AvSession)
						AvSession1 = session as IAvSession;

			if (e.OldItems != null && e.Action != NotifyCollectionChangedAction.Move)
			{
				foreach (Session session in e.OldItems)
					session.UccSession = null;
			}
		}

		private Presentity FindPresentity(string uri)
		{
			if (Helpers.IsUriEqual(selfPresentityMonitor.Uri, uri))
				return selfPresentityMonitor;
			foreach (Presentity presentity in this.Presentities)
				if (Helpers.IsUriEqual(presentity.Uri, uri))
					return presentity;
			return null;
		}

		private IPresentity GetPresentity(string uri)
		{
			IPresentity presentity = this.FindPresentity(uri);
			if (presentity == null)
				presentity = new FakePresentity(uri);
			return presentity;
		}

		private void PartipantLogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (ParticipantLog log in e.NewItems)
					log.Presentity = this.FindPresentity(log.Uri);
			}
		}

		private void Presenties_CollectionChanged2(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (Presentity presentity in e.OldItems)
				{
					foreach (Session session in this.Sessions)
						foreach (ParticipantLog log in session.PartipantLogs)
							if (Helpers.IsUriEqual(presentity.Uri, log.Uri))
								log.Presentity = null;
				}
			}
			if (e.NewItems != null && e.Action != NotifyCollectionChangedAction.Move)
			{
				foreach (Presentity presentity in e.NewItems)
				{
					foreach (Session session in this.Sessions)
						foreach (ParticipantLog log in session.PartipantLogs)
							if (Helpers.IsUriEqual(presentity.Uri, log.Uri))
								log.Presentity = presentity;
				}
			}
		}

		#endregion

		#region MediaDeviceSettings

		public void InvokeTuningWizard(int hwndParent, bool showAudio, bool showAudioNonPrivate, bool showAudioNotification, bool showWebCam)
		{
			IUccMediaDeviceSettings mediaDeviceSettings = this.platform as IUccMediaDeviceSettings;

			UCC_TUNING_WIZARD_PAGE pages = 0;

			if (showAudio)
				pages |= UCC_TUNING_WIZARD_PAGE.UCCTWP_AUDIO;
			if (showAudioNonPrivate)
				pages |= UCC_TUNING_WIZARD_PAGE.UCCTWP_AUDIO_NONPRIVATE;
			if (showAudioNotification)
				pages |= UCC_TUNING_WIZARD_PAGE.UCCTWP_AUDIO_NOTIFICATION;
			if (showWebCam)
				pages |= UCC_TUNING_WIZARD_PAGE.UCCTWP_WEBCAM;

			mediaDeviceSettings.InvokeTuningWizard(hwndParent, (int)pages);
		}

		public void ResetDeviceSettings()
		{
			IUccMediaDeviceSettings mediaDeviceSettings = this.platform as IUccMediaDeviceSettings;

			mediaDeviceSettings.ResetDeviceSettings();
		}

		#endregion

		#region SearchUsers

		public int BeginUserSearch(string searchDomain, IEnumerable<string> searchTerms)
		{
			IUccUserSearchQuery query = this.userSearchManager.CreateQuery();
			ComEvents.Advise<_IUccUserSearchQueryEvents>(query, this);

			query.SearchDomain = searchDomain;

			string name = null;
			foreach (string item in searchTerms)
			{
				if (string.IsNullOrEmpty(name))
					name = item;
				else
				{
					query.set_SearchTerm(name, item);
					name = null;
				}
			}

			UccOperationContext operationContext = new UccOperationContextClass();
			operationContext.Initialize(0, new UccContextClass());

			query.Execute(operationContext);

			return operationContext.OperationId;
		}

		public void UpdateSearchUserRecords(SearchUserRecord[] records, NotifyCollectionChangedEventArgs e)
		{
			if (records != null)
			{
				if (e.OldItems != null)
					foreach (IPresentity item in e.OldItems)
					{
						foreach (SearchUserRecord record in records)
							if (Helpers.IsUriEqual(item.Uri, record.Uri))
							{
								record.InContacts = false;
								break;
							}
					}

				if (e.NewItems != null)
					foreach (IPresentity item in e.NewItems)
					{
						foreach (SearchUserRecord record in records)
							if (Helpers.IsUriEqual(item.Uri, record.Uri))
							{
								record.InContacts = true;
								break;
							}
					}
			}
		}

		#endregion

		#region OnEnabled, OnDisabled

		private void OnEnabled(EndpointEventArgs eventArgs)
		{
			enableErrors.Add(eventArgs);
			OnEnabled();
		}

		private void OnEnabled()
		{
			OnEvent<EndpointEventArgss>(Enabled, new EndpointEventArgss() { Items = enableErrors, });
		}

		private void OnDisabled(EndpointEventArgs eventArgs)
		{
			OnEvent<EndpointEventArgs>(Disabled, eventArgs);
		}

		#endregion

		#region _IUccUserSearchQueryEvents [ OnExecute ]

		void _IUccUserSearchQueryEvents.OnExecute(UccUserSearchQuery eventSource, UccUserSearchQueryEvent eventData)
		{
			if (eventData.IsComplete)
			{
				var result = new UserSearchEventArgs(eventData);
				foreach (var item in result.Results)
					item.InContacts = Presentities.Contain(item.Uri);
				OnEvent<UserSearchEventArgs>(UserSearchFinished, result);
			}
		}

		#endregion

		#region _IUccSessionManagerEvents [ OnIncomingSession, OnOutgoingSession ]

		public ObservableCollection<AvInvite> AvInvites
		{
			get;
			private set;
		}

		void _IUccSessionManagerEvents.OnIncomingSession(IUccEndpoint eventSource, UccIncomingSessionEvent eventData)
		{
			if (eventData.Session.Type == UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING)
			{
				if (disableImSessions)
				{
					eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_NOT_ACCEPTABLE);
				}
				else
				{
					ISession session = null;
					if (eventData.Session.Participants.Count == 2)
					{
						session = this.FindSession(SessionType.ImSession, eventData.Inviter.Uri.Value);
						if (session != null)
							(session as Session).UccSession = eventData.Session;
					}

					if (session == null)
						session = this.CreateSession(eventData.Session);

					eventData.Accept();
				}
			}
			else if (eventData.Session.Type == UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO)
			{
				AvInvite eventArgs =
					new AvInvite(this.GetPresentity(eventData.Inviter.Uri.Value), this.configuration.AvInviteTimeout,
						eventData, (uccSession) => { this.CreateSession(uccSession); });

				AvInvites.Add(eventArgs);

				OnEvent<AvInvite>(IncommingAvSession, eventArgs);
			}
			else
			{
				eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_NOT_ACCEPTABLE);
			}
		}

		void _IUccSessionManagerEvents.OnOutgoingSession(IUccEndpoint eventSource, UccOutgoingSessionEvent eventData)
		{
		}

		#endregion

		#region _IUccSessionDescriptionEvaluator // not used

		//bool _IUccSessionDescriptionEvaluator.Evaluate(
		//                             string ContentType,
		//                             string SessionDescription)
		//{
		//    return SessionDescription.ToLower().Trim() == "xxx-session";
		//}

		#endregion _IUccSessionDescriptionEvaluator

		#region _IUccApplicationSessionParticipantEvents // not used

		//void _IUccApplicationSessionParticipantEvents.OnIncomingInvitation(
		//    UccApplicationSessionParticipant eventSource,
		//    UccIncomingInvitationEvent eventData
		//    )
		//{
		//    System.Windows.MessageBox.Show("OnIncomingInvitation");
		//}

		//void _IUccApplicationSessionParticipantEvents.OnInvitationAccepted(
		//    UccApplicationSessionParticipant eventSource,
		//    UccInvitationAcceptedEvent eventData
		//    )
		//{
		//}

		//void _IUccApplicationSessionParticipantEvents.OnOutgoingInvitation(
		//    UccApplicationSessionParticipant eventSource,
		//    UccOutgoingInvitationEvent eventData
		//    )
		//{
		//    eventData.ContentType = "xxx-l";
		//    eventData.SessionDescription = "xxx-session";
		//}

		//void _IUccApplicationSessionParticipantEvents.OnRenegotiate(
		//    UccApplicationSessionParticipant eventSource,
		//    IUccOperationProgressEvent eventData
		//    )
		//{
		//}

		#endregion _IUccApplicationSessionParticipantEvents

		#region _IUccSignalingChannelEvents // no used // experimental

		//void _IUccSignalingChannelEvents.OnIncomingMessage(
		//    IUccSignalingChannel eventSource,
		//    IUccIncomingSignalingMessageEvent eventData
		//    )
		//{
		//    eventData.Accept();
		//    System.Windows.MessageBox.Show(eventData.Message.Body);
		//}

		//void _IUccSignalingChannelEvents.OnSendRequest(
		//    IUccSignalingChannel eventSource,
		//    IUccOperationProgressEvent eventData
		//    )
		//{
		//    if (eventData.IsComplete && eventData.StatusCode >= 0)
		//        System.Windows.MessageBox.Show("SUBS sent!");
		//    else
		//        System.Windows.MessageBox.Show("SUBS failed!");
		//}

		#endregion _IUccSignalingChannelEvents

		#region _IUccPublicationEvent

		void _IUccPublicationEvent.OnPublish(
			IUccPublication publication,
			IUccOperationProgressEvent eventData)
		{
			if (eventData.IsComplete)
			{
				ComEvents.Unadvise<_IUccPublicationEvent>(publication, this);

				if (eventData.StatusCode >= 0)
				{
					Debug.WriteLine("Publication - OK");
				}
				else
				{
					if (eventData.OriginalOperationContext.Context.IsPropertySet(ContextInitialPublication))
					{
						Debug.WriteLine(String.Format("Initial Publication Failed: {0}", Errors.ToString(eventData.StatusCode)));
					}
					else
					{
						Debug.WriteLine(String.Format("Publication Failed: {0}", Errors.ToString(eventData.StatusCode)));
					}
				}
			}
		}

		#endregion

		#region _IUccSubscriptionEvents

		void _IUccSubscriptionEvents.OnQuery(
			   UccSubscription subscription,
			   UccSubscriptionEvent eventInfo)
		{
		}

		void _IUccSubscriptionEvents.OnSubscribe(
			   UccSubscription subscription,
			   UccSubscriptionEvent eventInfo)
		{
			Debug.WriteLine("OnSubscribe");

			if (eventInfo.IsComplete)
			{
				foreach (UccPresentity presenty in eventInfo.Presentities)
				{
					IUccOperationProgressEvent progressEvent = eventInfo.GetOperationInfo(presenty);

					Debug.WriteLine(String.Format("Presentity: <{0}> {1}", presenty.Uri.AddressOfRecord.ToString(), Errors.ToString(progressEvent.StatusCode)), "OnSubscribe");
				}
			}
		}

		void _IUccSubscriptionEvents.OnUnsubscribe(
			   UccSubscription subscription,
			   UccSubscriptionEvent eventInfo)
		{
			Debug.WriteLine("OnUnsubscribe");

			if (eventInfo.IsComplete)
			{
				foreach (UccPresentity presenty in eventInfo.Presentities)
				{
					IUccOperationProgressEvent progressEvent = eventInfo.GetOperationInfo(presenty);

					Debug.WriteLine(String.Format("Presentity: <{0}> {1}", presenty.Uri.AddressOfRecord.ToString(), Errors.ToString(progressEvent.StatusCode)), "OnUnsubscribe");
				}
			}
		}

		#endregion _IUccSubscriptionEvents

		#region _IUccPlatformEvents

		void _IUccPlatformEvents.OnShutdown(UccPlatform eventSource, IUccOperationProgressEvent eventData)
		{
		}

		void _IUccPlatformEvents.OnIpAddrChange(UccPlatform eventSource, object eventData)
		{
		}

		#endregion _IUccPlatformEvents

		#region _IUccEndpointEvents [ OnEnable, OnDisable ]

		void _IUccEndpointEvents.OnEnable(IUccEndpoint eventSource, IUccOperationProgressEvent eventData)
		{
			if (eventData.IsComplete)
			{
				if (eventData.StatusCode >= 0)
				{
					this.IsEnabled = true;

					if (configuration.DisablePublicationsSubscriptions == false)
					{
						this.SelfSubscribe();
						this.SelfPresentity.SetAvailability(this.loginAvailability);
						this.CreateSubscription();
						this.Subscribe(this.Presentities);
					}

					// "sip:MRASLoc.contoso.com@contoso.com;gruu;opaque=srvr:MRAS:bMOjOHEFQiCtPh2g624vPAAA";
					this.mediaEndpointSettings.FindMediaConnectivityServers(
						String.Format(@"sip:MRASLoc.{0}@{0}", this.uri.Host), null);
				}
				else
				{
					this.CleanupEndpoint();
				}

				this.enableErrors.Add(new EndpointEventArgs(eventData, configuration.AuthenticationModes[enableErrors.Count]));

				if (((UInt32)eventData.StatusCode == Errors.UCC_E_SIP_STATUS_CLIENT_UNAUTHORIZED ||
					(UInt32)eventData.StatusCode == Errors.UCC_E_SIP_AUTHENTICATION_TYPE_NOT_SUPPORTED ||
					(UInt32)eventData.StatusCode == Errors.UCC_E_AUTHENTICATION_SERVER_UNAVAILABLE)//||
					//(UInt32)eventData.StatusCode == Errors.opaque)
						&& enableErrors.Count < configuration.AuthenticationModes.Length)
				{
					this.InternalBeginLogin();
				}
				else
				{
					this.OnEnabled();
				}
			}
		}

		void _IUccEndpointEvents.OnDisable(IUccEndpoint eventSource, IUccOperationProgressEvent eventData)
		{
			if (eventData.IsComplete)
			{
				CleanupEndpoint();
				this.IsDisabled = true;

				OnDisabled(new EndpointEventArgs(eventData));
			}
		}

		#endregion _IUccEndpointEvents

		#region _IUccServerSignalingSettingsEvents [ OnFindServer ]

		void _IUccServerSignalingSettingsEvents.OnFindServer(IUccEndpoint eventSource, UccFindServerEvent eventData)
		{
			if (eventData.SignalingServers.Count > 0)
			{
				IUccSignalingServer server = eventData.SignalingServers[1] as IUccSignalingServer;
				foreach (IUccSignalingServer server1 in eventData.SignalingServers)
				{
					if (server1.TransportMode == UCC_TRANSPORT_MODE.UCCTM_TLS)
						server = server1;
					else if (server1.TransportMode == UCC_TRANSPORT_MODE.UCCTM_TCP && server.TransportMode == UCC_TRANSPORT_MODE.UCCTM_UDP)
						server = server1;
				}

				this.SetSignalingServer(
					new SignalingServer()
					{
						ServerAddress = server.ServerAddress,
						TransportMode = Helpers.Convert(server.TransportMode)
					});

				this.endpoint.Enable(null);
			}
			else
			{
				this.CleanupEndpoint();
				this.OnEnabled(new EndpointEventArgs(@"Can not find server, try to specify server address."));
			}
		}

		#endregion

		#region _IUccMediaEndpointEvents [ OnFindMediaConnectivityServers ]

		void _IUccMediaEndpointEvents.OnFindMediaConnectivityServers(IUccMediaEndpointSettings eventSource, IUccFindMediaConnectivityServersEvent eventData)
		{
			if (Helpers.IsOperationCompleteOk(eventData))
			{
				IUccCollection mediaServerConfigs = eventData.MediaConnectivityServerConfigurations;

				foreach (IUccMediaConnectivityServerConfiguration serverConfig in mediaServerConfigs)
					this.mediaEndpointSettings.AddMediaConnectivityServerWithCredential(serverConfig);
			}
		}

		#endregion
	}
}
