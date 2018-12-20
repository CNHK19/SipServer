// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
    class ImSession
        : Session
        , IImSession
        , _IUccInstantMessagingSessionEvents
        , _IUccInstantMessagingSessionParticipantEvents
    {
        public event EventHandler<ImSessionEventArgs1> SendResult;
        public event EventHandler<ImSessionEventArgs2> IncomingMessage;

        private Dictionary<int, OutgoingMessage> sendingMessages;
        private IUccInstantMessagingSession uccImSession;

        private const int ContextMessageId = 1;

        public ImSession(IPresentity selfPresentity)
            : base(selfPresentity)
        {
            this.sendingMessages = new Dictionary<int, OutgoingMessage>();
            transfersManager = new TransfersManager(this);
        }

        public override SessionType SessionType
        {
            get { return SessionType.ImSession; }
        }

        #region UccSession

		protected override void DetachUccSession(bool terminate)
        {
            this.uccImSession = null;

            base.DetachUccSession(terminate);
        }

        protected override void AttachUccSession(IUccSession uccSession)
        {
            base.AttachUccSession(uccSession);

            this.uccImSession = (IUccInstantMessagingSession)this.UccSession;
            ComEvents.Advise<_IUccInstantMessagingSessionEvents>(this.uccImSession, this);
        }

        #endregion

        public int SendingCount
        {
            get
            {
                return this.sendingMessages.Count;
            }
        }

        protected OutgoingMessage CreateOutgoingMessage(string contentType, string content)
        {
            OutgoingMessage message = new OutgoingMessage();
            message.ContentType = contentType;
            message.Message = content;
            message.DateTime = DateTime.Now;
            message.State = OutgoingMessageState.Created;

            return message;
        }

        protected IncomingMessage CreateIncomingMessage(string fromUri, string contentType, string content)
        {
            IncomingMessage message = new IncomingMessage();
            message.ContentType = contentType;
            message.Message = content;
            message.DateTime = DateTime.Now;
            message.FromUri = fromUri;

            return message;
        }

        #region IImSession [ Send, Composing ]

        private TransfersManager transfersManager;
        public ITransfersManager TransfersManager { get { return transfersManager; }  }

        public IOutgoingMessage Send(string contentType, string content)
        {
            OutgoingMessage message = this.CreateOutgoingMessage(contentType, content);

            var error = ValidateSession();
            if (error == null)
            {
                message.State = OutgoingMessageState.Sending;
                this.sendingMessages.Add(message.Id, message);

                UccOperationContext operationContext = new UccOperationContextClass();
                operationContext.Initialize(0, new UccContextClass());
                operationContext.Context.AddProperty(ContextMessageId, message.Id);

                this.uccImSession.SendMessage(message.ContentType, message.Message, operationContext);
            }
            else
            {
                message.Error = error;
                message.State = OutgoingMessageState.Failed;
                if (SendResult != null)
                    SendResult(this, new ImSessionEventArgs1(message));
            }

            return message;
        }

        public void Composing(bool composing)
        {
            if (this.uccImSession != null)
            {
                if (composing)
                    this.uccImSession.StartComposing(null);
                else
                    this.uccImSession.StopComposing(null);
            }
        }

        private string ValidateSession()
        {
            if (this.uccImSession == null)
                return @"Session terminated";

            if (HasRemoteConnectedParticipants() == false)
                return @"No connected participants";

            return null;
        }

        #endregion


        #region _IUccSessionParticipantCollectionEvents [ OnParticipantRemoved, OnParticipantAdded ]

        protected override void OnParticipantRemoved(IUccSession eventSource, IUccSessionParticipant participant)
        {
            base.OnParticipantRemoved(eventSource, participant);

            if (participant.IsLocal == false)
                ComEvents.Unadvise<_IUccInstantMessagingSessionParticipantEvents>(participant, this);
        }

        protected override void OnParticipantAdded(IUccSession eventSource, IUccSessionParticipant participant)
        {
            base.OnParticipantAdded(eventSource, participant);

            if (participant.IsLocal == false)
                ComEvents.Advise<_IUccInstantMessagingSessionParticipantEvents>(participant, this);
        }

        #endregion


        #region _IUccInstantMessagingSessionParticipantEvents [ OnInstantMessageReceived, OnComposing, OnIdle ]

        void _IUccInstantMessagingSessionParticipantEvents.OnInstantMessageReceived(UccInstantMessagingSessionParticipant eventSource, UccIncomingInstantMessageEvent eventData)
        {
            if (eventData.ContentType == MessageContentType.FileData)
                transfersManager.ProcessTransferMessage(eventData.Content, eventData.ParticipantEndpoint.Participant.Uri.Value);  
            else if (this.IncomingMessage != null)
            {
                IncomingMessage message = this.CreateIncomingMessage(
                    eventData.ParticipantEndpoint.Participant.Uri.Value,
                    eventData.ContentType,
                    eventData.Content
                    );

                this.IncomingMessage(this, new ImSessionEventArgs2(message));
            }
        }

        void _IUccInstantMessagingSessionParticipantEvents.OnComposing(UccInstantMessagingSessionParticipant eventSource, UccInstantMessagingComposingEvent eventData)
        {
            base.GetPartipantLog(eventData.ParticipantEndpoint.Participant).IsComposing = eventSource.IsComposing;
        }

        void _IUccInstantMessagingSessionParticipantEvents.OnIdle(UccInstantMessagingSessionParticipant eventSource, UccInstantMessagingComposingEvent eventData)
        {
            base.GetPartipantLog(eventData.ParticipantEndpoint.Participant).IsComposing = eventSource.IsComposing;
        }


        #endregion

        #region _IUccInstantMessagingSessionEvents [ OnSendMessage ]

        void _IUccInstantMessagingSessionEvents.OnSendMessage(UccInstantMessagingSession eventSource, UccSessionOperationEvent eventData)
        {
            int messageId = eventData.OriginalOperationContext.Context.get_Property(ContextMessageId).NumericValue;

            OutgoingMessage message = this.sendingMessages[messageId];

            foreach (IUccSessionParticipantOperationEvent operationEvent in eventData.ParticipantResults)
            {
                message.GetParticipantResult(operationEvent.ParticipantEndpoint.Uri).Set(
                    operationEvent.Result
                    );
            }

            int successCount = 0;
            int failedCount = 0;
            foreach (IParticipantResult result in message.SendResults)
            {
                if (result.IsComplete && Helpers.IsOperationSuccess(result.StatusCode))
                    successCount++;
                if (result.IsComplete && Helpers.IsOperationFailed(result.StatusCode))
                    failedCount++;
            }

            if (successCount + failedCount == message.SendResults.Count)
            {
                this.sendingMessages.Remove(messageId);

                if (successCount == 0)
                    message.State = OutgoingMessageState.Failed;
                else if (failedCount == 0)
                    message.State = OutgoingMessageState.Success;
                else
                    message.State = OutgoingMessageState.PartialSuccess;

                if (this.SendResult != null && message.ContentType != MessageContentType.FileData )
                    this.SendResult(this, new ImSessionEventArgs1(message));
            }
        }

        #endregion _IUccInstantMessagingSessionEvents
    }
}
