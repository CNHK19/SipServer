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
	/// <summary>
	/// Softphone
	/// </summary>
	public partial class Programme : Application
	{
		private Mutex runningMutex;
		private bool isApplicationNotRunning;
		private string userId;
		public Endpoint Endpoint { get; private set; }

		public Programme()
		{
#if DEBUG
#else
			InitializeCrashHandler();
#endif
			userId = GetCommandLineArgValue(@"-userid");
			runningMutex = new Mutex(false, "Local\\" + "{18A939DF-7148-4A29-A625-D85C911D43F6}" + userId, out isApplicationNotRunning);

			if (string.IsNullOrEmpty(userId) == false)
				Messenger.Properties.Settings.ReloadSettings(userId);

			if (isApplicationNotRunning && Settings.Default.NoSplash == false)
			{
				screen = new SplashScreen(@"SplashScreen.png");
				screen.Show(false);
			}

			if (Settings.Default.SettingsUpgrated == false)
			{
				try
				{
					Settings.Default.Upgrade();
					Settings.Default.SettingsUpgrated = true;
					Settings.Default.Save();
				}
				catch
				{
				}
			}

			if (Settings.Default.EndpointId == 0)
			{
				Settings.Default.EndpointId = Environment.TickCount;
				Settings.Default.Save();
			}

			LoadOutgoingCalls();

			Settings.Default.AutoSaveSettings = true;

			InitializeUccapi();
			Exit += Programme_Exit;
		}

		private void InitializeUccapi()
		{
			Endpoint = new Uccapi.Endpoint(this.Dispatcher);

			Endpoint.Enabled += Endpoint_Enabled;
			Endpoint.Disabled += Endpoint_Disabled;
			Endpoint.IncommingAvSession += Endpoint_IncommingAvSession;

			Endpoint.Initialize(AssemblyInfo.AssemblyProduct);
		}

		private void CleanupUccapi(int timeout, bool forceCleanup)
		{
			Endpoint.Enabled -= Endpoint_Enabled;
			Endpoint.Disabled -= Endpoint_Disabled;
			Endpoint.IncommingAvSession -= Endpoint_IncommingAvSession;

			if (Endpoint.IsEnabled)
				Endpoint.BeginLogout();

			WaitEndpointDisable(timeout);

			if (forceCleanup)
				Endpoint.Cleanup();
		}

		private void EnableEndpoint(AvailabilityValues availabality)
		{
			EnableEndpoint(availabality, true);
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (isApplicationNotRunning)
			{
				CloseSplash();

				var main = new Window1(Endpoint, this);
				main.Closing += Main_Closing;
				main.Closed += Main_Closed;
				main.Show();

				if (Settings.Default.LoginAtStartup)
					Commands.Login.Execute(null, main);
			}
			else
			{
				Shutdown();
			}
		}

		private void Main_Closed(object sender, EventArgs e)
		{
			CleanupUccapi(200, false);
		}

		private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			CleanupUccapi(100, false);
		}

		private void Programme_Exit(object sender, ExitEventArgs e)
		{
			CleanupUccapi(5000, true);
		}

		private void CloseBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shutdown();
		}

		#region OutgoingCalls

		private ObservableCollection<string> outgoingCalls = new ObservableCollection<string>();
		private const int maxOutgoingCall = 20;

		private void LoadOutgoingCalls()
		{
			if (Settings.Default.OutgoingCalls != null)
				foreach (var item in Settings.Default.OutgoingCalls)
					outgoingCalls.Add(item);
		}

		private void SaveOutgoingCalls()
		{
			var outgoingCalls2 = new System.Collections.Specialized.StringCollection();
			foreach (var item in outgoingCalls)
				outgoingCalls2.Add(item);

			Settings.Default.OutgoingCalls = outgoingCalls2;
		}

		private void UpdateOutgoingCalls(string uri)
		{
			if (string.IsNullOrEmpty(uri) == false)
			{
				int i;
				for (i = 0; i < outgoingCalls.Count; i++)
					if (outgoingCalls[i] == uri)
					{
						if (i > 0)
							outgoingCalls.Move(i, 0);
						break;
					}
				if (i >= outgoingCalls.Count)
					outgoingCalls.Insert(0, uri);

				while (outgoingCalls.Count > maxOutgoingCall)
					outgoingCalls.RemoveAt(outgoingCalls.Count - 1);

				SaveOutgoingCalls();
			}
		}

		#endregion
	}
}
