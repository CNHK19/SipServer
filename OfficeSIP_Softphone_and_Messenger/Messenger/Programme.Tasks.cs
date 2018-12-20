// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Messenger.Windows;
using System.Runtime.InteropServices;
using Messenger.Properties;
using Microsoft.Office.Interop.UccApi;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Uccapi;

namespace Messenger
{
    public partial class Programme
    {
        public CommandBindingCollection CommandBindings { get; private set; }

        private void InitializeCommandBindings()
        {
            CommandBindings = new CommandBindingCollection();

            CommandBindings.Add(new CommandBinding(MessengerCommands.AddContact, new ExecutedRoutedEventHandler(AddContactBinding_Executed)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.RemoveContact, new ExecutedRoutedEventHandler(RemoveContactBinding_Executed), new CanExecuteRoutedEventHandler(RemoveContactBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.FindContact, new ExecutedRoutedEventHandler(FindContactBinding_Executed), new CanExecuteRoutedEventHandler(FindContactBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.SendInstantMessage, new ExecutedRoutedEventHandler(SendInstantMessageBinding_Executed), new CanExecuteRoutedEventHandler(StartConversationBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.SendFile, new ExecutedRoutedEventHandler(SendFileBinding_Executed), new CanExecuteRoutedEventHandler(SendFileBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.StartAudioConversation, new ExecutedRoutedEventHandler(StartAudioConversationBinding_Executed), new CanExecuteRoutedEventHandler(StartConversationBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.StartVideoConversation, new ExecutedRoutedEventHandler(StartVideoConversationBinding_Executed), new CanExecuteRoutedEventHandler(StartConversationBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.UserProperties, new ExecutedRoutedEventHandler(UserPropertiesBinding_Executed), new CanExecuteRoutedEventHandler(UserPropertiesBinding_CanExecute)));
            CommandBindings.Add(new CommandBinding(MessengerCommands.CloseDialog, new ExecutedRoutedEventHandler(CloseDialogBinding_Executed)));
        }

        #region Close

        private void CloseBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Do NOT use Window.Close, Contacts.Close differ from base one
            (this.MainWindow as Contacts).Close();
        }

        #endregion

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == @"AlwaysOnTop")
                this.MainWindow.Topmost = Messenger.Properties.Settings.Default.AlwaysOnTop;
        }

        #region AddContact

        private void AddContactBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (false == ActivateWindow<AddContact>())
            {
                AddContact window = new AddContact();
                window.Done += new EventHandler<AddContact.Result>(AddContact_Done);
                window.Show();
            }
        }

        void AddContact_Done(object sender, AddContact.Result e)
        {
            IPresentity presentity = this.Endpoint.CreatePresentity(e.Uri, e.Group);

            this.Endpoint.Presentities.Add(presentity);
        }

        #endregion

        #region RemoveContact

        private void RemoveContactBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Do you realy want to remove selected contact(s)?",
                AssemblyInfo.AssemblyProduct, MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                System.Collections.ArrayList list =
                    new System.Collections.ArrayList(this.ContactsWindow.ContactList.SelectedItems);

                this.Endpoint.Presentities.RemoveRange(list);
            }
        }

        private void RemoveContactBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ContactsWindow.ContactList.SelectedIndex >= 0;
        }

        #endregion

        #region FindContact

        private void FindContactBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (false == ActivateWindow<FindContact>())
            {
                FindContact window = new FindContact(this.Endpoint);
                window.Show();
            }
        }

        private void FindContactBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Endpoint.IsEnabled;
        }

        #endregion

        #region UserProperties

        private void UserPropertiesBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UserProperties window = this.FindWindow<UserProperties>();

            if (window != null)
            {
                ActivateWindow<UserProperties>();
            }
            else
            {
                window = new UserProperties();
                window.Show();
            }

            window.DataContext = this.ContactsWindow.ContactList.SelectedItem;
        }

        private void UserPropertiesBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ContactsWindow.ContactList.SelectedIndex >= 0;
            e.Handled = true;
        }

        #endregion

        #region SendInstantMessage, StartAudioConversation, StartVideoConversation

        private ISession StartConversation(SessionType sessionType, bool enableVideo)
        {
            BugTracer.Get(BugId.N01).Trace(string.Format(@"StartConversation: {0}, {1}", sessionType, enableVideo));

            IPresentity presentity = this.ContactsWindow.GetSelectedContact();

            BugTracer.Get(BugId.N01).Trace(string.Format(@"StartConversation: {0}", presentity.Uri));

            ISession session = this.Endpoint.FindSession(sessionType, presentity.Uri);

            BugTracer.Get(BugId.N01).Trace(string.Format(@"StartConversation: session {0}found", (session == null) ? "not " : ""));

            if (session != null)
            {
                if (enableVideo)
                    (session as IAvSession).EnableVideo();

                this.Endpoint.RestoreSession(session);
            }
            else
            {
                session = this.Endpoint.CreateSession(sessionType);

                if (enableVideo)
                    (session as IAvSession).EnableVideo();

                BugTracer.Get(BugId.N01).EndTrace(@"StartConversation -- endtrace");

                session.AddPartipant(presentity.Uri);
            }

            return session;
        }

        private void SendInstantMessageBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BugTracer.Get(BugId.N01).BeginTrace(@"SendInstantMessageBinding_Executed");

            ISession session = StartConversation(SessionType.ImSession, false);


            this.chatWindow.Activate();
            this.chatWindow.SelectTabItem(session);

            e.Handled = true;
        }

        private void SendFileBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "(*.*)|*.*";
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();

            if (result != true)
                return;

            ISession session = StartConversation(SessionType.ImSession, false);

            this.chatWindow.Activate();
            this.chatWindow.SelectTabItem(session);
            chatWindow.Dispatcher.BeginInvoke(
                new SendFilesWaitDelegate(WaitForSessionRemoteUserConnects),
                System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                new object[] { this.chatWindow, DateTime.Now, (IImSession)session, dlg.FileNames });

            e.Handled = true;
        }

        private delegate void SendFilesWaitDelegate(Window CallingWindow, DateTime StartTime, IImSession WaitingSession, string[] FileNames);

        public void WaitForSessionRemoteUserConnects(Window CallingWindow, DateTime StartTime, IImSession WaitingSession, string[] FileNames)
        {

            if ((DateTime.Now - StartTime).Milliseconds > 2000)
                return;

            if (WaitingSession.HasRemoteConnectedParticipants())
            {
                WaitingSession.TransfersManager.Send(FileNames);
            }
            else
            {
                CallingWindow.Dispatcher.BeginInvoke(
                                new SendFilesWaitDelegate(WaitForSessionRemoteUserConnects),
                                System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                                new object[] { this.chatWindow, StartTime, WaitingSession, FileNames });
            }
        }


        private void StartAudioConversationBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ISession session = StartConversation(SessionType.AvSession, false);

            e.Handled = true;
        }

        private void StartVideoConversationBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ISession session = StartConversation(SessionType.AvSession, true);

            e.Handled = true;
        }

        private void StartConversationBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.Endpoint.IsEnabled &&
                this.ContactsWindow.ContactList.SelectedIndex >= 0;

            if (e.CanExecute)
                e.CanExecute = !Uccapi.Helpers.IsUriEqual(this.ContactsWindow.GetSelectedContact().Uri, Endpoint.Uri);

            e.Handled = true;
        }

        private void SendFileBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            StartConversationBinding_CanExecute(sender, e);

            if (e.CanExecute)
                e.CanExecute = (this.ContactsWindow.GetSelectedContact().Availability != AvailabilityValues.Offline &&
                    this.ContactsWindow.GetSelectedContact().Availability != AvailabilityValues.Unknown);

            e.Handled = true;
        }

        #endregion

        #region CloseDialog

        private void CloseDialogBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Source is Window)
            {
                var window = e.Source as Window;

                window.Close();

                e.Handled = true;
            }
        }

        #endregion
    }
}
