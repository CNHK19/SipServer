// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
    public class Uccapi// : _IUccPlatformEvents
    {
        /*
        private UccPlatform platform;

        public void Login(string appName)
        {
            CreatePlatform(appName);
        }

        /// <summary>
        /// Create platform
        /// </summary>
        /// <param name="appName"></param>
        private void CreatePlatform(string appName)
        {
            this.platform = new UccPlatform();

            Advise<_IUccPlatformEvents>(this.platform, this);

            this.platform.Initialize(appName, null);
        }

        /// <summary>
        /// Shutdown platform
        /// </summary>
        private void ShutdownPlatform()
        {

        }

        private void CreateEndPoint(UccUri uri, string signinName, string password, string domain)
        {
            // Create endpoint
            this.endpoint = this.platform.CreateEndpoint(UCC_ENDPOINT_TYPE.UCCET_PRINCIPAL_SERVER_BASED, uri, null, null);

            // Configure the endpoint
            IUccServerSignalingSettings settings = (IUccServerSignalingSettings)this.endpoint;

            // Add the credentials -- note: "*" means any realm
            UccCredential credential = settings.CredentialCache.CreateCredential(signName, password, domain);
            settings.CredentialCache.SetCredential("*", credential);

            // Set the server to use
            settings.Server = settings.CreateSignalingServer(serverName, (transport == "TCP") ? UCC_TRANSPORT_MODE.UCCTM_TCP : UCC_TRANSPORT_MODE.UCCTM_TLS);


            // Set the allowed authentication modes
            settings.AllowedAuthenticationModes = (int)UCC_AUTHENTICATION_MODES.UCCAM_DIGEST;

            // Register this client to receive event
            // notifications when the login session changes.
            Advise<_IUccEndpointEvents>(this.endpoint, this);
            Advise<_IUccSessionManagerEvents>(this.endpoint, this);
        }




        static void Advise<T>(object source, T sink)
        {
            IConnectionPointContainer container = (IConnectionPointContainer)source;
            
            Guid guid = typeof(T).GUID;
            IConnectionPoint point;
            container.FindConnectionPoint(ref guid, out point);

            int cookie;
            point.Advise(sink, out cookie);
        }

        void _IUccPlatformEvents.OnShutdown(UccPlatform pEventSource, IUccOperationProgressEvent pEventData)
        {
            if (pEventData.IsComplete && pEventData.StatusCode >= 0)
            {
                //Set platform reference to null
                this.platform = null;
            }
        }

        void _IUccPlatformEvents.OnIpAddrChange(UccPlatform eventSource, object eventData)
        {
        }
        */
    }
}
