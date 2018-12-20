// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

/*=====================================================================
  This file is part of the Microsoft Unified Communications Code Samples.

  Copyright (C) 2007 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.UccApi;

namespace UCCPSample
{
    public partial class UccController : _IUccSessionManagerEvents,
                                    _IUccSessionEvents,
                                    _IUccSessionParticipantEvents,
                                    _IUccSessionParticipantCollectionEvents
    {
        private IUccSession imSession = null;
        private IUccSession avSession = null;

        /// <summary>
        /// Create Session with appropriate session type
        /// </summary>
        /// <param name="sessionType"></param>
        public void CreateSession(UCC_SESSION_TYPE sessionType)
        {
            // Create Session
            IUccSessionManager sessionManager = this.endpoint as IUccSessionManager;
            IUccSession session = sessionManager.CreateSession(sessionType, null);

            // Register to receive events
            if (sessionType == UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING)
            {
                this.imSession = session;
                // Advise for InstantMessagingSession Events
                Advise<_IUccInstantMessagingSessionEvents>(session, this);
            }
            else if (sessionType == UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO)
            {
                this.avSession = session;
            }

            // Advise for SessionParticipantCollectionEvents & SessionEvents
            Advise<_IUccSessionParticipantCollectionEvents>(session, this);
            Advise<_IUccSessionEvents>(session, this);
        }

        /// <summary>
        /// Create and start AV session with remote Uri
        /// </summary>
        /// <param name="remoteUri"></param>
        public void StartAVSession(string avRemoteUri)
        {
            this.avRemoteUri = avRemoteUri;

            // Create AV session
            CreateSession(UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO);

            // Create remote participant
            UccUriManager uriManager = new UccUriManager();
            IUccSessionParticipant participant = this.avSession.CreateParticipant(uriManager.ParseUri(this.avRemoteUri), null);
            IUccAudioVideoSessionParticipant avParticipant = participant as IUccAudioVideoSessionParticipant;

            // Create audio channel
            IUccMediaChannel channel = avParticipant.CreateChannel(UCC_MEDIA_TYPES.UCCMT_AUDIO, null);
            channel.PreferredMedia = (int)(UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE | UCC_MEDIA_DIRECTIONS.UCCMD_SEND);

            // Add channel
            avParticipant.AddChannel(channel);

            UccOperationContext context = new UccOperationContextClass();
            context.Initialize(operationId++,null);
            // Add remote participant
            this.avSession.AddParticipant(participant, context);
        }

        /// <summary>
        /// Create and start IM session with remote Uri
        /// </summary>
        /// <param name="remoteUri"></param>
        public void StartIMSession(string imRemoteUri)
        {
            this.imRemoteUri = imRemoteUri;

            // Create IM session
            CreateSession(UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING);

            // Create remote participant
            UccUriManager uriManager = new UccUriManager();
            IUccSessionParticipant participant = this.imSession.CreateParticipant(uriManager.ParseUri(this.imRemoteUri), null);

            UccOperationContext context = new UccOperationContextClass();
            context.Initialize(operationId++, null);
            // Add remote participant
            this.imSession.AddParticipant(participant, context);
        }

        /// <summary>
        ///  End the IM session
        /// </summary>
        public void EndIMSession()
        {
            if (this.imSession != null)
            {
                UccOperationContext context = new UccOperationContextClass();
                context.Initialize(operationId++, null);
                this.imSession.Terminate(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_NOT_SPECIFIED, context);
            }
        }

        /// <summary>
        /// End the AV session
        /// </summary>
        public void EndAVSession()
        {
            if (this.avSession != null)
            {
                UccOperationContext context = new UccOperationContextClass();
                context.Initialize(operationId++, null);
                this.avSession.Terminate(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_NOT_SPECIFIED, context);
            }
        }

        public IUccSession IMSession
        {
            get { return this.imSession; }
            set { this.imSession = value; }
        }

        public IUccSession AVSession
        {
            get { return this.avSession; }
            set { this.avSession = value; }
        }

        #region _IUccSessionManagerEvents Members

        /// <summary>
        /// Process Incoming Session request
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void _IUccSessionManagerEvents.OnIncomingSession(
                            IUccEndpoint eventSource,
                            UccIncomingSessionEvent eventData)
        {
            DialogResult result;

            // Handle incoming IM session
            if (eventData.Session.Type == UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING)
            {
                    // The App only support one active IM session. Reject incoming IM session request
                    // if there is already an active IM session.
                    if (this.imSession != null)
                    {
                        MessageBox.Show("There is an active IM session. This Application only supports one active IM session right now. " +
                                "The App is rejecting incoming session from " + eventData.Inviter.Uri + ".");
                        eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE);
                        return;
                    }

                    // Ask user he wants to accept the incoming requet.
                    result = MessageBox.Show("Accept incoming IM session from " + eventData.Inviter.Uri + "?",
                                                 "Incoming Session", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes)
                    {
                        eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE);
                        return;
                    }

                    // Register to receive events
                    this.imSession = eventData.Session;
                    this.imRemoteUri = eventData.Inviter.Uri.AddressOfRecord;
                    Advise<_IUccInstantMessagingSessionEvents>(this.imSession, this);
                    foreach (IUccSessionParticipant oneParticipant in this.imSession.Participants)
                    {
                        if (oneParticipant != null && !oneParticipant.IsLocal)
                        {
                            Advise<_IUccInstantMessagingSessionParticipantEvents>(oneParticipant, this);
                            Advise<_IUccSessionParticipantEvents>(oneParticipant, this);
                        }
                    }
            }
            // Handle incoming AV session
            else if (eventData.Session.Type == UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO)
            {
                    // Reject incoming AV session request if there is already an active av session.
                    if (this.avSession != null)
                    {
                        MessageBox.Show("There is an AV active session. We only supports one active session right now. " +
                                "The App is rejecting incoming session from " +
                                eventData.Inviter.Uri + ".");
                        eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE);
                        return;
                    }

                    result = MessageBox.Show("Accept incoming AV session from " + eventData.Inviter.Uri + "?",
                                                 "Incoming Session", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes)
                    {
                        eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE);
                        return;
                    }

                    // Register to receive events
                    this.avRemoteUri = eventData.Inviter.Uri.AddressOfRecord;
                    this.avSession = eventData.Session;

                    foreach (IUccSessionParticipant oneParticipant in this.avSession.Participants)
                    {
                        if (oneParticipant != null && !oneParticipant.IsLocal)
                        {
                            Advise<_IUccSessionParticipantEvents>(oneParticipant, this);
                        }
                    }
            }
            else
            {
                eventData.Reject(UCC_REJECT_OR_TERMINATE_REASON.UCCROTR_DECLINE);
                return;
            }

            // register for generice events
            Advise<_IUccSessionEvents>(eventData.Session, this);
            Advise<_IUccSessionParticipantCollectionEvents>(eventData.Session, this);

            // Accept incoming session
            eventData.Accept();
        }

        void _IUccSessionManagerEvents.OnOutgoingSession(
                        IUccEndpoint eventSource,
                        UccOutgoingSessionEvent eventData)
        {
        }

        #endregion _IUccsessionManagerEvents

        #region _IUccSessionEvents
        /// <summary>
        /// Handle OnAddParticipant events. Display when it fails to add the participant.
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void _IUccSessionEvents.OnAddParticipant(
                        IUccSession eventSource,
                        IUccOperationProgressEvent eventData)
        {
            if (eventData.IsComplete && eventData.StatusCode < 0)
            {
                switch (eventSource.Type)
                {
                    case UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING:
                        this.imSession = null;
                        this.mainForm.WriteStatusMessage("IM Session failed to connect to " + this.imRemoteUri);
                        break;
                    case UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO:
                        this.avSession = null;
                        this.mainForm.WriteStatusMessage("AV Session failed to connect to " + this.avRemoteUri);
                        break;
                    default:
                        break;
                }
            }
        }

        void _IUccSessionEvents.OnRemoveParticipant(
                      IUccSession eventSource,
                      IUccOperationProgressEvent eventData)
        {
        }

        void _IUccSessionEvents.OnTerminate(
                  IUccSession eventSource,
                  IUccOperationProgressEvent eventData)
        {
        }

        #endregion _IUccSessionEvents

        #region _IUccSessionParticipantCollectionEvents

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>

        void _IUccSessionParticipantCollectionEvents.OnParticipantRemoved(
                        IUccSession eventSource,
                        UccSessionParticipantCollectionEvent eventData)
        {
        }

        /// <summary>
        /// Register related events when participant has been added
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void _IUccSessionParticipantCollectionEvents.OnParticipantAdded(
                        IUccSession eventSource,
                        UccSessionParticipantCollectionEvent eventData)
        {
            if (eventData.Participant.IsLocal)
                return;

            Advise<_IUccSessionParticipantEvents>(eventData.Participant, this);
            if (eventSource.Type == UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING)
            {
                Advise<_IUccInstantMessagingSessionParticipantEvents>(eventData.Participant, this);
            }
        }

        #endregion _IUccSessionParticipantCollectionEvents

        #region _IUccSessionParticipantEvents

        /// <summary>
        /// Handle OnStateChanged events. Set the buttons in mainform to appropriate state
        /// when the session is connected or disconnected
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void _IUccSessionParticipantEvents.OnStateChanged(
                        IUccSessionParticipant eventSource,
                        UccSessionParticipantStateChangedEvent eventData)
        {
            if (eventData.NewState == UCC_SESSION_ENTITY_STATE.UCCSES_CONNECTED)
            {
                switch (eventSource.Session.Type)
                {
                    case UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING:
                        this.mainForm.SetIMOff();
                        this.mainForm.WriteStatusMessage("IM session is connected to " + eventSource.Uri);
                        break;

                    case UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO:
                        this.mainForm.SetVoIPOff();
                        this.mainForm.WriteStatusMessage("AV session is connected to " + eventSource.Uri);
                        break;

                    default:
                        break;

                }
            }

            if (eventData.NewState == UCC_SESSION_ENTITY_STATE.UCCSES_DISCONNECTED)
            {
                switch (eventSource.Session.Type)
                {
                    case UCC_SESSION_TYPE.UCCST_INSTANT_MESSAGING:
                        this.imSession = null;
                        this.mainForm.SetIMOn();
                        this.mainForm.WriteStatusMessage("IM session is disconnected.");
                        break;

                    case UCC_SESSION_TYPE.UCCST_AUDIO_VIDEO:
                        this.avSession = null;
                        this.mainForm.SetVoipOn();
                        this.mainForm.WriteStatusMessage("AV Session is disconnected");
                        break;

                    default:
                        return;
                }
            }
        }

        void _IUccSessionParticipantEvents.OnAddParticipantEndpoint(IUccSessionParticipant pEventSource, IUccOperationProgressEvent pEventData)
	{

	}

        void _IUccSessionParticipantEvents.OnRemoveParticipantEndpoint(IUccSessionParticipant pEventSource, IUccOperationProgressEvent pEventData)
	{

	}


        #endregion _IUccSessionParticipantEvents
    }
}
