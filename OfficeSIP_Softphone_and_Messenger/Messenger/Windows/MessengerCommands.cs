// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows.Input;

namespace Messenger.Windows
{
	public class MessengerCommands
		: BaseCommands
	{
		public static RoutedUICommand AddContact { get; private set; }
		public static RoutedUICommand RemoveContact { get; private set; }
		public static RoutedUICommand FindContact { get; private set; }
		public static RoutedUICommand SetNewGroup { get; private set; }
		public static RoutedUICommand SendInstantMessage { get; private set; }
		public static RoutedUICommand SendFile { get; private set; }
		public static RoutedUICommand StartAudioConversation { get; private set; }
		public static RoutedUICommand StartVideoConversation { get; private set; }
		public static RoutedUICommand AlwaysOnTop { get; private set; }
		//public static RoutedUICommand Close { get; private set; }
		public static RoutedUICommand ToggleShowOfflineContacts { get; private set; }
		public static RoutedUICommand ToggleShowGroups { get; private set; }
		//public static RoutedUICommand ShowSessionDetails { get; private set; }
		public static RoutedUICommand UserProperties { get; private set; }
		//public static RoutedUICommand GotoUrl { get; private set; }
		//public static RoutedUICommand GotoEmail { get; private set; }
		public static RoutedUICommand CloseDialog { get; private set; }

		static MessengerCommands()
		{
			Initialize();
		}

		public static void Initialize()
		{
			if (AddContact == null)
			{
				AddContact = CreateRoutedUICommand("AddContact", typeof(MessengerCommands));
				RemoveContact = CreateRoutedUICommand("RemoveContact", typeof(MessengerCommands));
				FindContact = CreateRoutedUICommand("FindContact", typeof(MessengerCommands));
				SetNewGroup = CreateRoutedUICommand("SetNewGroup", typeof(MessengerCommands));
				SendInstantMessage = CreateRoutedUICommand("SendInstantMessage", typeof(MessengerCommands));
				SendFile = CreateRoutedUICommand("SendFile", typeof(MessengerCommands));
				StartAudioConversation = CreateRoutedUICommand("StartAudioConversation", typeof(MessengerCommands));
				StartVideoConversation = CreateRoutedUICommand("StartVideoConversation", typeof(MessengerCommands));
				AlwaysOnTop = CreateRoutedUICommand("AlwaysOnTop", typeof(MessengerCommands));
				//Close = new RoutedUICommand("Close", "Close", typeof(MessengerCommands));
				ToggleShowOfflineContacts = CreateRoutedUICommand("ToggleShowOfflineContacts", typeof(MessengerCommands));
				ToggleShowGroups = CreateRoutedUICommand("ToggleShowGroups", typeof(MessengerCommands));
				//ShowSessionDetails = new RoutedUICommand("Session Details", "ShowSessionDetails", typeof(MessengerCommands), CreateGestureCollection(Key.D, ModifierKeys.Control));
				UserProperties = CreateRoutedUICommand("UserProperties", typeof(MessengerCommands));
				//GotoUrl = new RoutedUICommand("Go to Url", "GotoUrl", typeof(MessengerCommands));
				//GotoEmail = new RoutedUICommand("Go to Email", "GotoEmail", typeof(MessengerCommands));
				CloseDialog = CreateRoutedUICommand("CloseDialog", typeof(MessengerCommands));
			}
		}
	}
}
