// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// SOFTPHONE
	/// </summary>
	public partial class UccTracing 
		: WindowEx
	{
		private DirectoryInfo reportsDirecory;
		private Endpoint endpoint;

		public UccTracing(Endpoint endpoint1)
		{
			endpoint = endpoint1;
			EnableTracing = endpoint.IsTracingEnabled;

			StringBuilder path = new StringBuilder();
			if (Helpers.SHGetSpecialFolderPath(IntPtr.Zero, path, Helpers.CSIDL_PROFILE, false))
				path.Append(@"\Tracing");

			reportsDirecory = new DirectoryInfo(path.ToString());

			InitializeComponent();
		}

		private void ListView_Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (Commands.View.CanExecute(null, (sender as IInputElement)))
				Commands.View.Execute(null, (sender as IInputElement));
			e.Handled = true;
		}

		private void Privacy_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.MessageBox.Show(
				@"The tracing files can contain private user information including, but not limited to, the user's contact list, presence and contact information of users, and information about conversations:" + Environment.NewLine + Environment.NewLine +
				@"1. Contact list. The contact list of the user as obtained from the server is logged if logging is enabled. This contains contacts, groups, and distribution groups that are part of a user's contact list." + Environment.NewLine + Environment.NewLine +
				@"2. Presence and contact information of users. Presence information (availability and activity) of the user and the user's contacts as well as contact information such as phone number, company, title, and office location are logged if logging is enabled." + Environment.NewLine + Environment.NewLine +
				@"3. Conversation information. For any IM, voice, or video conversations/conferences the SIP URIs of the participants are exchanged in the SIP messages. These get recorded when the SIP traffic is logged if logging is enabled. The content of a conversation (such as instant messages and audio/video exchanged) are not logged."
				);
		}

		public bool EnableTracing { get; set; }
		public string SelectedReportFullName { get; set; }

		public FileInfo[] Reports
		{
			get
			{
				return reportsDirecory.GetFiles("*.uccapilog");
			}
		}

		#region CommandBindings Event Handlers

		private void OkBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			endpoint.IsTracingEnabled = EnableTracing;
			Close();
		}

		private void OkBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = EnableTracing != endpoint.IsTracingEnabled;
		}

		private void CancelBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void ViewBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				Process.Start(SelectedReportFullName);
			}
			catch
			{
				try
				{
					Process.Start(Path.GetDirectoryName(SelectedReportFullName));
				}
				catch
				{ 
				}
			}
		}

		private void ViewBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !string.IsNullOrEmpty(SelectedReportFullName);
		}

		private void RefreshBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			reportsDirecory.Refresh();
			OnPropertyChanged(@"Reports");
		}

		private void OpenFolderBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				Process.Start(reportsDirecory.FullName);
			}
			catch
			{
			}
		}

		private void FolderActionBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = reportsDirecory.Exists;
		}

		#endregion
	}
}
