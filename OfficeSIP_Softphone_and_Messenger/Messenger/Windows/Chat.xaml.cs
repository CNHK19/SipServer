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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Helpers;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for Chat.xaml
	/// </summary>
	public partial class Chat : Window
	{
		public Chat()
		{
			InitializeComponent();
			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				VistaGlass.ExtendGlass(this, -1, -1, -1, -1);
			}
			// If not Vista, paint background white.
			catch //(DllNotFoundException)
			{
				this.Background = SystemColors.MenuBarBrush;
			}
		}

		private void AddTabItem(IImSession session)
		{
			ChatTabItem chatData = new ChatTabItem(session);

			ChatTabItemHeader header = new ChatTabItemHeader();
			header.CloseButton.Click += new RoutedEventHandler(chatData.DestroySession);


			ChatTabItemContent content = new ChatTabItemContent();
			content.SendButton.Click += new RoutedEventHandler(chatData.SendMessage);
			content.SkipButton.Click += new RoutedEventHandler(chatData.SkipButton_Click);
			content.CancelButton.Click += new RoutedEventHandler(chatData.CancelButton_Click);

			content.CommandBindings.Add(
				new CommandBinding(RichTextBoxEx.RichTextBoxExCommands.CtrlEnter, chatData.Execute_InsertNewLine));
			chatData.ChatEdit = content.ChatViewer;
			chatData.MessageEdit = content.OutgoingMessageEdit;

			TabItem tabItem = new TabItem();
			tabItem.Header = header;
			tabItem.Content = content;
			tabItem.DataContext = chatData;

			tabControl.Items.Add(tabItem);
			tabControl.SelectedIndex = tabControl.Items.Count - 1;

			session.IncomingMessage += session_IncomingMessage;
		}

		private TabItem FindTabItem(ISession session)
		{
			foreach (TabItem tabItem in this.tabControl.Items)
				if ((tabItem.DataContext as ChatTabItem).Session == session)
					return tabItem;
			return null;
		}

		private void RemoveTabItem(IImSession session)
		{
			this.tabControl.Items.Remove(this.FindTabItem(session));

			session.IncomingMessage -= session_IncomingMessage;
		}

		public void SelectTabItem(ISession session)
		{
			this.tabControl.SelectedItem = this.FindTabItem(session);
		}

		public void Sessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Session session in e.NewItems)
					if (session is ImSession)
					{
						this.AddTabItem(session as ImSession);
						if (tabControl.Items.Count == 1)
						{
							this.Show();
							this.Activate();
							this.SelectTabItem(session);
						}
					}
			}

			if (e.OldItems != null && e.Action != NotifyCollectionChangedAction.Move)
			{
				foreach (Session session in e.OldItems)
					if (session is IImSession)
						this.RemoveTabItem(session as IImSession);
				if (tabControl.Items.Count == 0)
					this.Hide();
			}
		}

		private void session_IncomingMessage(object sender, ImSessionEventArgs2 e)
		{
			FlashWindow.Flash(this, FlashWindow.FLASHW_TIMERNOFG | FlashWindow.FLASHW_TRAY, 2);
		}
	}
}
