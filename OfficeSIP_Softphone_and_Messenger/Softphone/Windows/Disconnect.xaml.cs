// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// SOFTPHONE
	/// </summary>
	public partial class Disconnect
		: WindowEx
	{
		private int restoreSeconds;
		private DispatcherTimer timer;

		public Disconnect(IEnumerable<EndpointEventArgs> errors, bool restore, int restoreSeconds)
		{
			this.Closed += Disconnect_Closed;

            this.Error = "";
            foreach (var error in errors)
				this.Error += String.Format("Uccapi Error ({2}): {0:x}, {3}{1}\r\n", error.StatusCode, error.StatusText, error.DateTime.ToString("g"), AuthModeToString(error.AuthMode));
			
            this.restoreSeconds = restoreSeconds;
			this.RestoreEnabled = restore;

			if (this.RestoreEnabled)
			{
				timer = new DispatcherTimer();
				timer.Interval = new TimeSpan(0, 0, 1);
				timer.Tick += Timer_Tick;
				timer.Start();
			}

			DataContext = this;
			InitializeComponent();

			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
		}

		private void Disconnect_Closed(object sender, EventArgs e)
		{
			if (timer != null)
				timer.Stop();
		}

		public string Error { get; private set; }
		public bool RestoreEnabled { get; private set; }

		private string AuthModeToString(AuthenticationMode? authMode)
		{
			if (authMode != null)
			{
				switch (authMode)
				{
					case AuthenticationMode.KerberosNtlmDefaultCreditals:
						return @"KERBEROS + NTLM, Default Creditals, ";
					case AuthenticationMode.KerberosDefaultCreditals:
						return @"KERBEROS, Default Creditals, ";
					case AuthenticationMode.NtlmDefaultCreditals:
						return @"NTLM, Default Creditals, ";
					case AuthenticationMode.NtlmDigestCustomCreditals:
						return @"NTLM + DIGEST, User Defined Creditals, ";
					case AuthenticationMode.NtlmCustomCreditals:
						return @"NTLM, User Defined Creditals, ";
					case AuthenticationMode.DigestCustomCreditals:
						return @"DIGEST, User Defined Creditals, ";
				}
			}

			return @"";
		}

		#region RestoreSeconds

		public int RestoreSeconds 
		{
			get
			{
				return this.restoreSeconds;
			}
			private set
			{
				if (this.restoreSeconds != value)
				{
					this.restoreSeconds = value;
					this.OnPropertyChanged(@"RestoreSeconds");
				}
			}
		}

		#endregion

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (this.RestoreSeconds <= 1)
			{
				timer.Stop();
				Commands.Login.Execute(null, this);
			}
			else
			{
				this.RestoreSeconds--;
			}
		}

		private void CancelBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
