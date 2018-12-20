// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Messenger.Windows;
using Messenger.Properties;
using Uccapi;

namespace Messenger
{
	partial class Programme
	{
		#region Call

		private void CallBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			CallError = null;

			if (Endpoint.IsEnabled)
			{
				var uri = e.Parameter as string;

				if (Endpoint.AvSession1 != null)
					Endpoint.AvSession1.Destroy();

				if (Uccapi.Helpers.IsUriEqual(uri, Endpoint.Uri))
				{
					CallError = @"URI is invalid, loopback is not allowed";
					return;
				}

				if (Endpoint.IsValidUri(uri) == false)
				{
					CallError = @"URI is invalid, e.g. sip:jdoe@officesip.local";
					return;
				}

				Endpoint.CreateSession<IAvSession>();
				Endpoint.AvSession1.EnableVideo();
				Endpoint.AvSession1.AddPartipant(uri);

				UpdateOutgoingCalls(uri);
			}
		}

		private void CallBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Endpoint.IsEnabled &&
				(Endpoint.AvSession1 == null ||
					Endpoint.AvSession1.HasRemoteConnectedParticipants() == false && Endpoint.AvSession1.HasConnectingParticipants() == false);
		}

		#endregion

		#region HangUp

		private void HangUpBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Endpoint.AvSession1.Destroy();
		}

		private void HangUpBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Endpoint.IsEnabled && 
				Endpoint.AvSession1 != null && Endpoint.AvSession1.HasRemoteConnectedParticipants();
		}

		#endregion

		#region Remove All

		private void RemoveAllBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			outgoingCalls.Clear();
			SaveOutgoingCalls();
		}

		private void RemoveAllBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = outgoingCalls.Count > 0;
		}

		#endregion
	}
}