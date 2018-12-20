// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Messenger.Windows;
using Messenger.Properties;
using System.Windows.Controls.Primitives;

namespace Messenger
{
	partial class Programme
	{
		#region Homepage

		private void HomepageBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			using (System.Diagnostics.Process process = new System.Diagnostics.Process())
			{
				process.StartInfo.FileName = Messenger.AssemblyInfo.Homepage;
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
		}

		#endregion

		#region Feedback

		private void FeedbackBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			using (System.Diagnostics.Process process = new System.Diagnostics.Process())
			{
				process.StartInfo.FileName = Messenger.AssemblyInfo.Feedback;
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
		}

		#endregion

		#region GotoUrl, GotoEmail

		private void GotoUrlBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter != null && e.Parameter is string)
			{
				try
				{
					using (System.Diagnostics.Process process = new System.Diagnostics.Process())
					{
						process.StartInfo.FileName = e.Parameter as string;
						process.StartInfo.UseShellExecute = true;
						process.Start();
					}
				}
				catch
				{
				}
			}
		}

		private void GotoEmailBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter != null && e.Parameter is string)
			{
				try
				{
					string email = e.Parameter as string;
					if (email.StartsWith(@"mailto:", StringComparison.OrdinalIgnoreCase) == false)
						email = @"mailto:" + email;

					using (System.Diagnostics.Process process = new System.Diagnostics.Process())
					{
						process.StartInfo.FileName = email;
						process.StartInfo.UseShellExecute = true;
						process.Start();
					}
				}
				catch
				{
				}
			}
		}

		#endregion

		#region About

		private void AboutBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (false == this.ActivateWindow<About>())
				new About().Show();
		}

		#endregion

		#region Preferences

		private void PreferencesBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (false == ActivateWindow<Preferences>())
				new Preferences().Show();
		}

		#endregion

		#region UccTracing

		private void UccTracingBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (false == ActivateWindow<UccTracing>())
				new UccTracing(Endpoint).Show();
		}

		#endregion

		#region TuningWizard

		private void InvokeTuningWizardBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			WindowInteropHelper windowInterop = new WindowInteropHelper(MainWindow);

			this.Endpoint.InvokeTuningWizard((int)windowInterop.Handle, true, true, false, true);
		}

		private void ResetTuningWizardSettingsBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (MessageBox.Show("Do you realy want to reset tunning wizard settings to default values?",
				AssemblyInfo.AssemblyProduct, MessageBoxButton.YesNo,
				MessageBoxImage.Question) == MessageBoxResult.Yes)
				this.Endpoint.ResetDeviceSettings();
		}

		#endregion

		#region Login

		private void LoginBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (Settings.Default.ReviewLogin)
			{
				Settings.Default.ReviewLogin = false;
				LoginAsBinding_Executed(sender, e);
			}
			else
			{
				EnableEndpoint(e.Parameter);
			}
		}

		private void LoginAsBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (false == ActivateWindow<Login>())
			{
				Login login = new Login();
				login.LoginParameter = e.Parameter;
				login.Closed += Login_Closed;
				login.Show();
			}
		}

		private void Login_Closed(object sender, EventArgs e)
		{
			if ((sender as Login).Result == true)
				EnableEndpoint((sender as Login).LoginParameter);
		}

		private void LoginBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Endpoint.IsDisabled;
			e.Handled = true;
		}

		#endregion

		#region Logout

		private void LogoutBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Endpoint.BeginLogout();
		}

		private void LogoutBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Endpoint.IsEnabled;
			e.Handled = true;
		}

		#endregion

		#region CopyAll

		private void CopyAllBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Source is TextBoxBase)
			{
				var textBox = e.Source as TextBoxBase;

				textBox.SelectAll();
				textBox.Copy();

				e.Handled = true;
			}
		}

		private void CopyAllBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (e.Source is TextBoxBase)
			{
				e.CanExecute = true;
				e.Handled = true;
			}
		}

		#endregion

		#region ViewIncomingCalls

		private void ViewIncomingCallsBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (false == ActivateWindow<Invite>())
				new Invite(Endpoint).Show();
		}

		private void ViewIncomingCallsBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Endpoint.AvInvites.Count > 0;
		}

		#endregion
	}
}
