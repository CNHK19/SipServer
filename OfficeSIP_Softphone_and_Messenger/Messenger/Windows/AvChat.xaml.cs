// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Interop;
using System.ComponentModel;
using Messenger.Properties;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for AvChat.xaml
	/// </summary>
	public partial class AvChat 
		: WindowEx
	{
		public AvChat(IAvSession session)
		{
			this.Session = session;

			this.Closed += Window_Closed;

			InitializeComponent();
			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			this.Session.Destroy();
			this.Session = null;
		}

		private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Helpers.ListGridView_UpdateColumnWidth(sender as ListView, e, 1);
		}

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//if (this.Session.VideoInChannelCount > 0)
			//    if ((sender as ListView).SelectedIndex != -1)
			//        this.ShowVideo((sender as ListView).SelectedItem as ParticipantLog);
		}

		public IAvSession Session
		{
			get;
			private set;
		}

		public Size VideoSize
		{
			get { return Settings.Default.VideoSize; }
			set { Settings.Default.VideoSize = value; }
		}

		private void ViewSessionDetails_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewSessionDetails = !ViewSessionDetails;
			e.Handled = true;
		}

		private void ViewLocalVideoBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewLocalVideo = !ViewLocalVideo;
			e.Handled = true;
		}

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
	}
}
