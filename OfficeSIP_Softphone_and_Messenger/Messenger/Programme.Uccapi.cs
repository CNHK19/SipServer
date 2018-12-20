// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using System.Configuration;
using System.ComponentModel;
using Microsoft.Office.Interop.UccApi;
using Messenger.Windows;
using Messenger.Helpers;
using Messenger;
using Messenger.Properties;
using Uccapi;

namespace Messenger
{
    public partial class Programme
    {
		private DispatcherTimer autoAwayTimer;
		private bool autoAwayEnabled;

		public bool ignoryPresentitiesChanges;
        public Endpoint Endpoint { get; private set; }

        private void InitializeUccapi()
        {
			Endpoint = new Endpoint(null);

			Endpoint.Enabled += Endpoint_Enabled;
            Endpoint.Disabled += Endpoint_Disabled;
			Endpoint.IncommingAvSession += Endpoint_IncommingAvSession;
			Endpoint.Presentities.CollectionChanged += Presentities_CollectionChanged;
			Endpoint.Presentities.ItemPropertyChanged += Presentities_ItemPropertyChanged;

			Endpoint.Initialize(AssemblyInfo.AssemblyProduct);

			this.ignoryPresentitiesChanges = true;
			Endpoint.Presentities = presentitiesStorage.Presentities;
			this.ignoryPresentitiesChanges = false;
		}

		private void CleanupUccapi(int timeout, bool forceCleanup)
		{
			if (Endpoint != null)
			{
				Endpoint.Enabled -= Endpoint_Enabled;
				Endpoint.Disabled -= Endpoint_Disabled;
				Endpoint.IncommingAvSession -= Endpoint_IncommingAvSession;
				Endpoint.Presentities.CollectionChanged -= Presentities_CollectionChanged;
				Endpoint.Presentities.ItemPropertyChanged -= Presentities_ItemPropertyChanged;

				if (Endpoint.IsEnabled)
					Endpoint.BeginLogout();

				WaitEndpointDisable(timeout);

				if (forceCleanup)
					Endpoint.Cleanup();
			}
		}

		private void EnableEndpoint(AvailabilityValues availabality)
		{
			EnableEndpoint(availabality, false);
		}

		private void StartAutoAwayTimer()
		{
			autoAwayEnabled = false;

			autoAwayTimer = new DispatcherTimer();
			autoAwayTimer.Interval = new TimeSpan(0, 0, 1);
			autoAwayTimer.Tick += new EventHandler(AutoAwayTimer_Tick);
			autoAwayTimer.Start();
		}

		private void Presentities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.ignoryPresentitiesChanges == false)
			{
				presentitiesStorage.Presentities = this.Endpoint.Presentities;
				presentitiesStorage.Save();
			}
		}

		private void Presentities_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.ignoryPresentitiesChanges == false)
			{
				if (Uccapi.PresentitiesCollectionXmlSerializer.IsPropertySerializable(e.PropertyName))
				{
					presentitiesStorage.Presentities = this.Endpoint.Presentities;
					presentitiesStorage.Save();
				}
			}
		}

		private void AutoAwayTimer_Tick(object sender, EventArgs e)
		{
			if (Settings.Default.AutoAway)
			{
				if (autoAwayEnabled)
				{
					if (LastInputTime.GetLastInputTime() <= Settings.Default.AutoAwaySeconds)
					{
						autoAwayEnabled = false;
						Endpoint.SelfPresentity.SetAvailability(AvailabilityValues.Online);
					}
				}
				else
				{
					if (Endpoint.SelfPresentity.Availability == AvailabilityValues.Online)
						if (LastInputTime.GetLastInputTime() > Settings.Default.AutoAwaySeconds)
						{
							autoAwayEnabled = true;
							Endpoint.SelfPresentity.SetAvailability(AvailabilityValues.Away);
						}
				}
			}
		}
	}
}
