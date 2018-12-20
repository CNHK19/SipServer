// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Threading.Ex;
using Messenger.Properties;
using Messenger.Windows;
using Uccapi;

namespace Messenger
{
	public partial class Programme 
		: Application
	{
		private void WaitEndpointDisable(int timeout)
		{
			for (int i = 0; i < timeout && Endpoint.IsDisabled == false; i += 25)
			{
				System.Threading.Thread.Sleep(25);
				DispatcherHelper.DoEvents();
			}
		}

		private void EnableEndpoint(object availabality)
		{
			EnableEndpoint((availabality is Uccapi.AvailabilityValues)
					? (Uccapi.AvailabilityValues)availabality : Uccapi.AvailabilityValues.Online);
		}

		private void EnableEndpoint(AvailabilityValues availabality, bool softphone)
		{
			CloseDisconnectWindow();

			var configuration = new EndpointConfiguration()
			{
				EndpointId = Settings.Default.EndpointId,

				Uri = Settings.Default.SignInAddress,

				DetectServer = Settings.Default.AutoConfigServer,

				Username = Settings.Default.Username,
				Password = Settings.Default.Password,
				Realm = Settings.Default.Realm,

				DisableImSessions = softphone,
				DisablePublicationsSubscriptions = softphone,
			};

			if (Settings.Default.UseDefaultCredential && Settings.Default.UseSpecifiedCredential)
				configuration.AuthenticationModes = EndpointConfiguration.AllAuthenticationModes;
			else if (Settings.Default.UseDefaultCredential)
				configuration.AuthenticationModes = EndpointConfiguration.DefaultAuthenticationModes;
			else
				configuration.AuthenticationModes = EndpointConfiguration.CustomAuthenticationModes;

			if (Settings.Default.AutoConfigServer == false)
			{
				configuration.SignalingServer =
					new SignalingServer()
					{
						ServerAddress = Settings.Default.ServerAddress,
						TransportMode = (TransportMode)Settings.Default.IpProtocol
					};
			}

			Endpoint.BeginLogin(configuration, availabality);
		}

		private void Endpoint_Enabled(object sender, EndpointEventArgss e)
		{
			if (e.IsOperationFailed)
				ShowDisconnectWindow(e.Items);
		}

		private void Endpoint_Disabled(object sender, EndpointEventArgs e)
		{
			if (e.IsOperationCompleteFailed)
				ShowDisconnectWindow(new EndpointEventArgs[] { e, });
		}

		private void Endpoint_IncommingAvSession(object sender, AvInvite e)
		{
			var invite = FindWindow<Invite>();

			if (invite == null)
			{
				invite = new Invite(Endpoint);
				invite.CurrentAvInvite = e;
				invite.Show();
			}
			else
			{
				if (invite.CurrentAvInvite.State != AvInviteState.Pending)
					invite.CurrentAvInvite = e;
			}
		}

		private void CloseDisconnectWindow()
		{
			var disconnect = FindWindow<Disconnect>();
			if (disconnect != null)
				disconnect.Close();
		}

		private void ShowDisconnectWindow(IEnumerable<EndpointEventArgs> errors)
		{
			CloseDisconnectWindow();

			new Disconnect(errors, Settings.Default.RestoreConnection, 30).Show();
		}
	}
}
