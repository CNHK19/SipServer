// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.ComponentModel;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for Invite.xaml
	/// </summary>
	public partial class Invite 
		: WindowEx
	{
		private MediaPlayer mediaPlayer;
		private DispatcherTimer timer;
		private DispatcherTimer timer2;
		private Endpoint endpoint;

		public Invite(Endpoint endpoint1)
		{
			endpoint = endpoint1;
			endpoint.PropertyChanged += Endpoint_PropertyChanged;

			Unloaded += Invite_Unloaded;

			DataContext = this;
			InitializeComponent();

			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);

			timer2 = new DispatcherTimer();
			timer2.Tick += new EventHandler(Timer2_Tick);
			timer2.Interval = new TimeSpan(0, 0, 0, 0, 500);
			timer2.Start();
		}

		private void Invite_Unloaded(object sender, RoutedEventArgs e)
		{
			endpoint.PropertyChanged -= Endpoint_PropertyChanged;

			timer2.Stop();
			StopPlaying();
			CurrentAvInvite = null;
		}

		private void Timer2_Tick(object sender, EventArgs e)
		{
			if (FindPendingInvite() == null)
				StopPlaying();
			else
				StartPlaying();
		}

		#region MediaPlayer

		private void StopPlaying()
		{
			if (mediaPlayer != null)
			{
				timer.Stop();
				mediaPlayer.Stop();
				mediaPlayer = null;
			}
		}

		private void StartPlaying()
		{
			if (mediaPlayer == null)
			{
				try
				{
					mediaPlayer = new MediaPlayer();
					mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
					mediaPlayer.Open(new Uri(System.IO.Path.GetFullPath(Properties.Settings.Default.IncomingCallSound)));
					mediaPlayer.Play();
				}
				catch
				{
				}

				timer = new DispatcherTimer();
				timer.Interval = new TimeSpan(0, 0, 3);
				timer.Tick += Timer_Tick;
			}
		}

		private void MediaPlayer_MediaEnded(object sender, EventArgs e)
		{
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			try
			{
				timer.Stop();
				mediaPlayer.Position = TimeSpan.Zero;
				mediaPlayer.Play();
			}
			catch
			{
			}
		}

		#endregion

		public Endpoint Endpoint
		{
			get { return endpoint; }
		}

		#region CurrentAvInvite

		private AvInvite currentAvInvite;

		public AvInvite CurrentAvInvite
		{
			get { return currentAvInvite; }
			set
			{
				if (currentAvInvite != null)
					currentAvInvite.PropertyChanged -= CurrentAvInvite_PropertyChanged;

				if (value != null)
					value.PropertyChanged += CurrentAvInvite_PropertyChanged;

				Dispatcher_BeginInvoke_InvalidateRequerySuggested();

				currentAvInvite = value;
				OnPropertyChanged(@"CurrentAvInvite");
			}
		}

		#endregion

		private void Endpoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Dispatcher_BeginInvoke_InvalidateRequerySuggested();
		}

		private void CurrentAvInvite_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == @"State")
				Dispatcher_BeginInvoke_InvalidateRequerySuggested();
		}

		private AvInvite FindPendingInvite()
		{
			foreach (var invite in Endpoint.AvInvites)
				if (invite.State == AvInviteState.Pending)
					return invite;
			return null;
		}

		#region CommandBindings Event Handlers

		private void AcceptCallBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CurrentAvInvite != null && CurrentAvInvite.State == AvInviteState.Pending;
		}

		private void AcceptCallBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (Endpoint.AvSession1 != null)
				Endpoint.AvSession1.Destroy();
			CurrentAvInvite.Accept();
			Close();
		}

		private void RejectCallBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CurrentAvInvite != null && CurrentAvInvite.State == AvInviteState.Pending;
		}

		private void RejectCallBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			CurrentAvInvite.Decline();
			Close();
		}

		private void CancelBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void RedialBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Commands.Call.Execute(CurrentAvInvite.Inviter.Aor, this);
		}

		private void RedialBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (CurrentAvInvite != null && CurrentAvInvite.State != AvInviteState.Pending)
				e.CanExecute = Commands.Call.CanExecute(CurrentAvInvite.Inviter.Aor, this);
		}

		#endregion
	}
}
