// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Net;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for Crash2.xaml
	/// </summary>
	public partial class Crash2 : Window
	{
		private static readonly DependencyProperty UploadingProperty;
		private static readonly DependencyProperty UploadedProperty;
		private static readonly DependencyProperty NotUploadedProperty;
		private static readonly DependencyProperty UploadingProgressProperty;
		private static readonly DependencyProperty UploadResultProperty;

		static Crash2()
		{
			UploadingProperty = DependencyProperty.Register("Uploading", typeof(bool),
				typeof(Crash2), new FrameworkPropertyMetadata(false));
			UploadedProperty = DependencyProperty.Register("Uploaded", typeof(bool),
				typeof(Crash2), new FrameworkPropertyMetadata(false));
			NotUploadedProperty = DependencyProperty.Register("NotUploaded", typeof(bool),
				typeof(Crash2), new FrameworkPropertyMetadata(true));
			UploadingProgressProperty = DependencyProperty.Register("UploadingProgress", typeof(int),
				typeof(Crash2), new FrameworkPropertyMetadata(0));
			UploadResultProperty = DependencyProperty.Register("UploadResult", typeof(string),
				typeof(Crash2), new FrameworkPropertyMetadata());
		}

		private string reportFileName;
		private WebClient webClient;
		private Uri uploadUri = new Uri("http://www.officesip.com/uprep.php");

		public Crash2()
		{
			DataContext = this;
			InitializeComponent();

			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
			message.Text = message.Text.Replace("AssemblyProduct", AssemblyInfo.AssemblyProduct);
		}

		public string Report
		{
			get;
			set;
		}

		private void CreateWebClient()
		{
			if (webClient == null)
			{
				CredentialCache credentialCache = new CredentialCache();
				credentialCache.Add(uploadUri, @"Basic", new NetworkCredential(@"uprep", @"qeiusroi123woi3zf"));

				webClient = new WebClient();
				webClient.UploadFileCompleted += WebClient_UploadFileCompleted;
				webClient.UploadProgressChanged += WebClient_UploadProgressChanged;
				webClient.Credentials = credentialCache.GetCredential(uploadUri, @"Basic");
				webClient.QueryString.Add("app", "MSG");
				webClient.QueryString.Add("ver", AssemblyInfo.AssemblyVersion);
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Send_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//File.WriteAllText(@"C:\Temp\OfficeSIP-Messenger.txt", this.Report, Encoding.Unicode);

				reportFileName = Path.GetTempFileName();
				File.WriteAllText(reportFileName, this.Report, Encoding.Unicode);

				CreateWebClient();
				webClient.UploadFileAsync(uploadUri, reportFileName);

				SetValue(UploadingProgressProperty, 0);
				SetValue(UploadResultProperty, null);
				SetValue(UploadingProperty, true);
			}
			catch
			{
				SetValue(UploadResultProperty, @"Failed");
				SetValue(UploadingProperty, false);
			}
		}

		private void WebClient_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
		{
			SetValue(UploadingProgressProperty, e.ProgressPercentage);
		}

		private void WebClient_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
		{
			if (string.IsNullOrEmpty(reportFileName) == false)
				File.Delete(reportFileName);

			string serverResponse;

			if (e.Error != null)
				serverResponse = e.Error.Message;
			else if (e.Cancelled)
				serverResponse = @"Canceled";
			else
			{
				string replyTag = @"#RESPONSE#";
				serverResponse = Encoding.UTF8.GetString(e.Result);
				int begin = serverResponse.IndexOf(replyTag) + replyTag.Length;
				int end = serverResponse.LastIndexOf(replyTag);
				if (begin >= 0 && end >= 0)
					serverResponse = serverResponse.Substring(begin, end - begin);

				if (serverResponse == @"OK")
				{
					SetValue(UploadedProperty, true);
					SetValue(NotUploadedProperty, false);
				}
			}

			SetValue(UploadingProperty, false);
			SetValue(UploadResultProperty, serverResponse);
		}
	}
}
