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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Messenger;
using Messenger.Windows;
using Messenger.Properties;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1
		: WindowEx
	{
		private Endpoint endpoint;
		private IProgrammeData programmeData;
		private DispatcherTimer updateCommandsTimer;

		public Window1(Endpoint endpoint1, IProgrammeData programmeData1)
		{
			endpoint = endpoint1;
			endpoint.Disabled += Endpoint_Disabled;
	//		endpoint.PropertyChanged += Endpoint_PropertyChanged;

			programmeData = programmeData1;

			Topmost = Settings.Default.AlwaysOnTop;

			Settings.Default.PropertyChanged += Settings_PropertyChanged;

			updateCommandsTimer = new DispatcherTimer();
			updateCommandsTimer.Interval = new TimeSpan(0, 0, 1);
			updateCommandsTimer.Tick += new EventHandler(UpdateCommandsTimer_Tick);
			updateCommandsTimer.Start();

			InitializeComponent();
		}

		public string Title1 { get { return AssemblyInfo.AssemblyTitle; } }

		public int Top1
		{
			get { return Settings.Default.Top; }
			set { Settings.Default.Top = value; }
		}

		public int Left1
		{
			get { return Settings.Default.Left; }
			set { Settings.Default.Left = value; }
		}

		public Endpoint Endpoint
		{
			get { return endpoint; }
		}

		public IProgrammeData ProgrammeData
		{
			get { return programmeData; }
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == @"AlwaysOnTop")
				Topmost = Settings.Default.AlwaysOnTop;
		}

		private void UpdateCommandsTimer_Tick(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		#region PhoneNumber

		private string phoneNumber;

		public string PhoneNumber
		{
			get { return phoneNumber; }
			set
			{
				if (phoneNumber != value)
				{
					phoneNumber = value;
					OnPropertyChanged(@"PhoneNumber");
				}
			}
		}

		#endregion

		#region ViewDialpad

		public bool ViewDialpad
		{
			get { return Settings.Default.ViewDialpad; }
			set
			{
				if (Settings.Default.ViewDialpad != value)
				{
					Settings.Default.ViewDialpad = value;
					OnPropertyChanged(@"ViewDialpad");
				}
			}
		}

		#endregion

		#region ViewSessionDetails

		public bool ViewSessionDetails
		{
			get { return Settings.Default.ViewSessionDetails; }
			set
			{
				if (Settings.Default.ViewSessionDetails != value)
				{
					Settings.Default.ViewSessionDetails = value;
					OnPropertyChanged(@"ViewSessionDetails");
				}
			}
		}

		#endregion

		#region ViewLocalVideo

		public bool ViewLocalVideo
		{
			get { return Settings.Default.ViewLocalVideo; }
			set
			{
				if (Settings.Default.ViewLocalVideo != value)
				{
					Settings.Default.ViewLocalVideo = value;
					OnPropertyChanged(@"ViewLocalVideo");
				}
			}
		}

		#endregion

		private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if ("1234567890ABCDabcd*#".Contains(e.Text[0]))
				if (Endpoint.AvSession1 != null)
					Endpoint.AvSession1.SendDtmf(e.Text[0]);
		}

		//private void BindVideo()
		//{
		//    if (videoLarge.Child != Endpoint.AvSession1)
		//    {
		//        if (Endpoint.AvSession1 != null && Endpoint.AvSession1.VideoInChannelCount > 0)
		//        {
		//            Endpoint.AvSession1.VideoWindow.IsVideo2Visible = ViewLocalVideo;
					
		//            videoLarge.Child = Endpoint.AvSession1.VideoWindow;
		//            videoLarge.Visibility = Visibility.Visible;
		//        }
		//        else
		//        {
		//            videoLarge.Child = null;
		//            videoLarge.Visibility = Visibility.Collapsed;
		//        }
		//    }
		//}

		//private void Endpoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//    if (e.PropertyName == @"AvSession1")
		//    {
		//        if (Endpoint.AvSession1 != null)
		//            Endpoint.AvSession1.PropertyChanged += AvSession1_PropertyChanged;
		//        BindVideo();
		//    }
		//}

		//private void AvSession1_PropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//    if (e.PropertyName == @"VideoInChannelCount")
		//        BindVideo();
		//}

		private void Endpoint_Disabled(object sender, EndpointEventArgs e)
		{
			if (Endpoint.AvSession1 != null)
				Endpoint.AvSession1.Destroy();
		}

		#region CommandBindings Event Handlers

		private void PhoneDigitBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string digit = e.Parameter as string;

			PhoneNumber += digit;

			if (Endpoint.AvSession1 != null)
				Endpoint.AvSession1.SendDtmf(digit[0]);

			e.Handled = true;
		}

		private void PhoneDigitBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ViewDialpadBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewDialpad = !ViewDialpad;
			e.Handled = true;
		}

		private void ViewSessionDetailsBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewSessionDetails = !ViewSessionDetails;
			e.Handled = true;
		}

		private void ViewLocalVideoBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewLocalVideo = !ViewLocalVideo;
			e.Handled = true;

			if (Endpoint.AvSession1 != null)
				Endpoint.AvSession1.VideoWindow.IsVideo2Visible = ViewLocalVideo;
		}

		#endregion
	}
}
