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
using Microsoft.Office.Interop.UccApi;

namespace UCCPSample
{
    public partial class UccController : _IUccInstantMessagingSessionEvents,
                                    _IUccInstantMessagingSessionParticipantEvents
    {
        private string contentType = "text/plain";

        /// <summary>
        /// Send an instance message to remote participant.
        /// </summary>
        /// <param name="message"></param>
        public void SessionSendMessage(string message)
        {
            IUccInstantMessagingSession session = this.imSession as IUccInstantMessagingSession;

            UccOperationContext context = new UccOperationContextClass();
            context.Initialize(operationId++, null);            
            session.SendMessage(contentType, message, context);

            string formatMessage = string.Format("{0}: \r\n {1}", uri, message);
            this.mainForm.WriteIMMessage(formatMessage);
        }

        /// <summary>
        /// Start composing IM message
        /// </summary>
        /// <param name="composingContentType"></param>
        /// <param name="idleTimeoutSeconds"></param>
        public void StartComposing(string composingContentType, int idleTimeoutSeconds)
        {
            IUccInstantMessagingSession session = this.imSession as IUccInstantMessagingSession;
            //*** session.StartComposing(composingContentType, idleTimeoutSeconds);
            
        }

        /// <summary>
        /// Stop composing IM message
        /// </summary>
        public void StopComposing()
        {
            IUccInstantMessagingSession session = this.imSession as IUccInstantMessagingSession;
            //*** session.StopComposing();
        }

        #region _IUccInstantMessagingSessionParticipantEvents

        /// <summary>
        /// Instance message is received, display it.
        /// </summary>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void _IUccInstantMessagingSessionParticipantEvents.OnInstantMessageReceived(
                        UccInstantMessagingSessionParticipant eventSource,
                        UccIncomingInstantMessageEvent eventData)
        {
            string formatMessage = string.Format("{0}: \r\n {1}",
                        eventData.ParticipantEndpoint.Participant.Uri,
                        eventData.Content);
            this.mainForm.WriteIMMessage(formatMessage);
        }

        /// <summary>
        /// Following event methods will be implemented lated when needed
        /// <summary>
        void _IUccInstantMessagingSessionParticipantEvents.OnComposing(
                        UccInstantMessagingSessionParticipant eventSource,
                        UccInstantMessagingComposingEvent eventData)
        {
        }

        void _IUccInstantMessagingSessionParticipantEvents.OnIdle(
                        UccInstantMessagingSessionParticipant eventSource,
                        UccInstantMessagingComposingEvent eventData)
        {
        }


        #endregion _IUccInstantMessagingSessionParticipantEvents


        #region _IUccInstantMessagingSessionEvents

        void _IUccInstantMessagingSessionEvents.OnSendMessage(
                       UccInstantMessagingSession eventSource,
                       UccSessionOperationEvent eventData)
        {
        }

        #endregion _IUccInstantMessagingSessionEvents

    }
}

