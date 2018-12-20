// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Linq;
using Uccapi;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Helpers;

namespace Messenger.Windows
{
	public class ChatTabItem
		: INotifyPropertyChanged
	{
		private MediaPlayer mediaPlayer;
		private DispatcherTimer restoreSessionTimer;
		private bool isRestoringSession;

		public ChatTabItem(IImSession session)
		{
			this.mediaPlayer = new MediaPlayer();

			this.restoreSessionTimer = new DispatcherTimer();
			this.restoreSessionTimer.Interval = new TimeSpan(0, 0, 1);
			this.restoreSessionTimer.Tick += RestoreSessionTimer_Tick;
			this.restoreSessionTimer.Start();

			IsRestoringSession = false;

			this.Session = session;
			this.Session.PartipantLogs.CollectionChanged += PartipantLogs_CollectionChanged;
			this.Session.SendResult += SendResult;
			this.Session.IncomingMessage += IncomingMessage;

			this.Session.TransfersManager.TransferError += OnTransferError;
			this.Session.TransfersManager.TransferEnded += OnTransferEnded;
			this.Session.TransfersManager.IncomingTransferRequest += OnIncomingTransferRequest;
			this.Session.TransfersManager.OutgoingTransfer += TransfersManager_OutgoingTransfer;

		}

		private void PartipantLogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (ParticipantLog log in e.NewItems)
					log.PropertyChanged += ParticipantLog_PropertyChanged;
			}

			if (e.OldItems != null && e.Action != NotifyCollectionChangedAction.Move)
			{
				foreach (ParticipantLog log in e.NewItems)
					log.PropertyChanged -= ParticipantLog_PropertyChanged;
			}

			this.OnPropertyChanged("HeaderText");
			this.OnPropertyChanged("HeaderAvailability");
		}

		void ParticipantLog_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == @"Availability")
				this.OnPropertyChanged("HeaderAvailability");
			if (e.PropertyName == @"DisplayNameOrAor")
				this.OnPropertyChanged("HeaderText");
		}

		public IImSession Session
		{
			get;
			private set;
		}

		public bool IsRestoringSession
		{
			get
			{
				return isRestoringSession;
			}
			private set
			{
				if (value != isRestoringSession)
				{
					isRestoringSession = value;
					OnPropertyChanged(@"IsRestoringSession");
				}
			}
		}

		public string HeaderText
		{
			get
			{
				if (this.Session.PartipantLogs.Count > 2)
					return @"Multi-user";
				foreach (ParticipantLog log in this.Session.PartipantLogs)
					if (log.IsLocal == false)
						return log.DisplayNameOrAor;
				return @"--";
			}
		}

		public AvailabilityValues HeaderAvailability
		{
			get
			{
				if (this.Session.PartipantLogs.Count == 2)
				{
					foreach (ParticipantLog log in this.Session.PartipantLogs)
						if (log.IsLocal == false)
							return log.Availability;
				}
				else if (this.Session.PartipantLogs.Count > 2)
				{
					if (this.Session.IsTerminated())
						return AvailabilityValues.Offline;
					else
						return AvailabilityValues.Online;
				}

				return AvailabilityValues.Unknown;
			}
		}

		public RichTextBoxEx.RichTextBoxEx ChatEdit { get; set; }
		public RichTextBoxEx.RichTextBoxEx MessageEdit { get; set; }

		private void PlaySound(string soundFile)
		{
			try
			{
				mediaPlayer.Stop();
				mediaPlayer.Open(new Uri(System.IO.Path.GetFullPath(soundFile)));
				mediaPlayer.Play();
			}
			catch
			{
			}
		}

		private void SendResult(Object sender, ImSessionEventArgs1 e)
		{
			if (string.IsNullOrEmpty(e.Message.Error) == false)
				InsertMessage(@"Failed: " + e.Message.Error, Brushes.Red);
			else
				PlaySound(Properties.Settings.Default.OutgoingMessageSound);
		}

		private void InsertMessage(string message, Brush foreground)
		{
			Paragraph paragraph = new Paragraph();
			paragraph.Inlines.Add(
				new Run(message)
				{
					FontWeight = FontWeights.Normal,
					Foreground = foreground,
				}
			);

			if (ChatEdit.Document.Blocks.Count == 1)
				ChatEdit.RemoveEmptyLastParagraph();
			ChatEdit.Document.Blocks.Add(paragraph);

			ChatEdit.ScrollToEnd();
		}

		private Paragraph InsertHeader(ImSessionEventArgs2 e)
		{
			return this.InsertHeader(
				this.Session.GetDisplayNameOrAor(e.Message.FromUri), e.Message.DateTime, GetColor(e.Message.FromUri));
		}

		private Paragraph InsertHeader(string name, DateTime time, Brush foreground)
		{
			Paragraph paragraph = new Paragraph()
			{
				Foreground = foreground,
				Margin = new Thickness(0, 5, 0, 0),
			};

			paragraph.Inlines.Add(new Run(name) { FontWeight = FontWeights.Bold, });
			paragraph.Inlines.Add(new Run(@" (" + time.ToShortTimeString() + @"):"));
			paragraph.Inlines.Add(new LineBreak());

			if (ChatEdit.Document.Blocks.Count == 1)
				ChatEdit.RemoveEmptyLastParagraph();
			ChatEdit.Document.Blocks.Add(paragraph);

			return paragraph;
		}

		private void InsertRichText(string rtf)
		{
			ChatEdit.AppendRtf(rtf);
			ChatEdit.RemoveEmptyLastParagraph();

			ChatEdit.ScrollToEnd();
		}

		private void IncomingMessage(Object sender, ImSessionEventArgs2 e)
		{
			if (e.Message.ContentType == MessageContentType.Plain)
			{
				this.InsertHeader(e).Inlines.Add(new Run(e.Message.Message));
			}
			else if (e.Message.ContentType == MessageContentType.Enriched)
			{
				this.InsertHeader(e);
				this.InsertRichText(e.Message.Message);
			}
			else
			{
				this.InsertHeader(e).Inlines.Add(new Run("NOT SUPPORTED MESSAGE FORMAT. PLEASE CONTACT MESSENGER DEVELOPER."));
			}

			PlaySound(Properties.Settings.Default.IncomingMessageSound);
		}

		public void Execute_InsertNewLine(object sender, ExecutedRoutedEventArgs e)
		{
			MessageEdit.InsertNewLine();
		}

		public void Execute_SendMessage(object sender, ExecutedRoutedEventArgs e)
		{
			SendMessage(sender, e);
		}

		public void SendMessage(object sender, RoutedEventArgs e)
		{
			if (Programme.Instance.Endpoint.IsEnabled)
			{
				if (this.Session.IsTerminated())
				{
					if (IsRestoringSession == false)
					{
						IsRestoringSession = true;
						Programme.Instance.Endpoint.RestoreSession(this.Session);
					}
				}
				else
				{
#if DEBUG
					// *For testing purposes.
					if (new TextRange(MessageEdit.Document.ContentStart, MessageEdit.Document.ContentEnd).Text == "b93954f6-3c85-4c39-b488-1dd966e9eff0\r\n")
					{
						FileTransferTests FTests = new FileTransferTests();
						FTests.PerformFileTransfersTests(this.Session, ChatEdit);
						return;
					}
#endif
					var rtf = MessageEdit.InsertDefaultFontInfo(
								MessageEdit.CutRtf(new TextRange(
									MessageEdit.Document.ContentStart,
									MessageEdit.Document.ContentEnd)));

					this.InsertHeader(this.Session.SelfPresentity.DisplayNameOrAor, DateTime.Now, GetColor(this.Session.SelfPresentity.Uri));
					this.InsertRichText(rtf);

					this.Session.Send(MessageContentType.Enriched, rtf);
				}
			}
			else
			{
				InsertMessage("The messenger is logged out. You need to login before sending message.", Brushes.Red);
			}
		}

		private void TransfersManager_OutgoingTransfer(object sender, TransfersManagerEventArgs e)
		{
			InsertMessage("Sent request for file transfer.", Brushes.Gray);
		}

		private void OnIncomingTransferRequest(object sender, TransfersManagerEventArgs e)
		{
			var select = new SelectFiles(sender as ITransfersManager, e.Items,
				e.FromUri, Session.GetDisplayNameOrAor(e.FromUri));
			select.Show();
		}

		private string GetFilesList(IEnumerable<ITransferItem> items)
		{
			string files = "";
			foreach (ITransferItem item in items)
			{
				if (string.IsNullOrEmpty(files) == false)
					files += "\n";
				files += item.FileName + " (" + Helpers.SizeToStr(item.FileSize) + ")";
			}

			return files;
		}

		private void OnTransferError(object sender, TransferErrorEventArgs e)
		{
			string files = GetFilesList(e.Items);

			switch (e.ItemsState)
			{
				case FileTransferState.RejectedRemote:
					InsertMessage("File(s) was rejected by the user: " + files, Brushes.Red);
					break;

				case FileTransferState.StoppedByUser:
					InsertMessage("Transfer of file(s) was stopped by the user: " + files, Brushes.Red);
					break;

				case FileTransferState.StoppedByError:
				case FileTransferState.ErrorDuringCreation:
					InsertMessage("There were problems during transfer request: " + files, Brushes.Red);
					break;

				case FileTransferState.Corrupted:
					InsertMessage("File was corrupted: " + files, Brushes.Red);
					break;

				default:
					break;
			}
		}

		private void OnTransferEnded(object sender, TransfersManagerEventArgs e)
		{
			if (e.Items.First().State == FileTransferState.SuccessfullyEnded)
				InsertMessage("File(s) has been transferred: " + GetFilesList(e.Items), Brushes.Gray);
		}

		private void RestoreSessionTimer_Tick(object sender, EventArgs e)
		{
			if (IsRestoringSession)
			{
				if (Session.HasConnectingParticipants() == false)
				{
					IsRestoringSession = false;

					if (this.Session.IsTerminated())
						InsertMessage(@"Failed to restore instant messaging session.", Brushes.Red);
					else
						SendMessage(sender, null);
				}
			}
		}

		public void SkipButton_Click(object sender, RoutedEventArgs e)
		{
			Session.TransfersManager.Stop(Session.TransfersManager.Incoming.CurrentFile);
		}

		public void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Session.TransfersManager.Stop(Session.TransfersManager.Incoming.Files);
		}

		public void DestroySession(object sender, RoutedEventArgs e)
		{
			this.Session.Destroy();
		}

		private Brush GetColor(string uri)
		{
			#region var brushes = new Brush[] {...}

			var brushes = new Brush[] 
			{
				Brushes.DarkBlue,
				Brushes.DarkRed,
				Brushes.DarkOrange,
				Brushes.DarkCyan,
				Brushes.DarkGoldenrod,
				Brushes.DarkGray,
				Brushes.DarkGreen,
				Brushes.DarkKhaki,
				Brushes.DarkMagenta,
				Brushes.DarkOliveGreen,
				Brushes.DarkOrchid,
				Brushes.DarkSalmon,
				Brushes.DarkSeaGreen,
				Brushes.DarkSlateBlue,
				Brushes.DarkSlateGray,
				Brushes.DarkTurquoise,
				Brushes.DarkViolet,
			};

			#endregion

			var log = Session.FindParticipantLog(uri);
			if (log != null)
				return ParticipantIdConverter.GetBrush(log.Id);

			return Brushes.Black;
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(String property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		#endregion
	}
}
