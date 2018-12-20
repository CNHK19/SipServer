// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
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

using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;
using UCCPSample;


namespace Messenger
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        UccController uccControl = null;

        const UInt32 UCC_E_CODECS_MISMATCH = 0x80EE0000;

        public Window1()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.uccControl = new UccController(this);
            WriteStatusMessage("Uccpapi was initialized");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.uccControl.Logout();
            this.uccControl.ShutdownPlatform();
        }

        /// <summary>
        /// Login a user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (this.uccControl == null)
            {
                WriteStatusMessage("Creating UCCP platform. Please try login later.");
                return;
            }

            if (this.uccControl.Endpoint != null)
            {
                return;
            }

            try
            {
                string uri = textLogin.Text;
                string signInName = uri.Split(new char[]{':', '@'})[1];
                string password = "pass";
                string domain = "officesip.local";
                string serverName = "192.168.1.15";

                this.buttonLogin.IsEnabled = false;
                WriteStatusMessage("User " + signInName + " is logging in ...");
                this.uccControl.CreateLoginSession(signInName, password, domain, serverName, "TCP", uri);

            }
            catch (COMException ex)
            {
                WriteStatusMessage("Login failed. Error: " + ex.Message);
                this.buttonLogin.IsEnabled = true;
            }

        }

        /// <summary>
        /// Logout the current user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLogout_Click(object sender, RoutedEventArgs e)
        {
            if (this.uccControl.Endpoint == null)
                return;
            try
            {
                WriteStatusMessage("User " + this.uccControl.SignName + " is logging out ...");
                this.uccControl.Logout();

            }
            catch (COMException ex)
            {
                WriteStatusMessage("Logout failed. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Start instance message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBeginIm_Click(object sender, RoutedEventArgs e)
        {
            if (this.uccControl == null)
            {
                WriteStatusMessage("Creating UCCP platform. Please try login later.");
                return;
            }

            if (this.uccControl.Endpoint == null)
            {
                WriteStatusMessage("ERROR: You must login first.");
                return;
            }

            if (this.uccControl.IMSession != null)
            {
                WriteStatusMessage("The App only supports one active IM session. " +
                    "Please end current active seesion and try again.");
                return;
            }

            try
            {
                string remoteUri = textImParti.Text;

                this.uccControl.StartIMSession(remoteUri);
                WriteStatusMessage("IM session is connecting with " + remoteUri);
            }
            catch (COMException ex)
            {
                WriteStatusMessage("IM connection failed. Error: " + ex.Message);
                this.uccControl.IMSession = null;
            }
        }


        /// <summary>
        /// End instance message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEndIm_Click(object sender, RoutedEventArgs e)
        {
            if (this.uccControl.IMSession == null)
            {
                WriteStatusMessage("ERROR: There is no active IM session.");
                return;
            }
            try
            {
                WriteStatusMessage("Ending IM session ...");
                this.uccControl.EndIMSession();
            }
            catch (COMException ex)
            {
                WriteStatusMessage("Failed to end IM session. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Start a call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClickCall_Click(object sender, EventArgs e)
        {
            if (this.uccControl == null)
            {
                WriteStatusMessage("Creating UCCP platform. Please try login later.");
                return;
            }

            if (this.uccControl.Endpoint == null)
            {
                WriteStatusMessage("ERROR: You must login first.");
                return;
            }

            if (this.uccControl.AVSession != null)
            {
                WriteStatusMessage("The App only supports one active AV session. " +
                    "Please end current active seesion and try again.");
                return;
            }

            try
            {
                string remoteUri = "sip:test@officesip.local";

                this.uccControl.StartAVSession(remoteUri);
                WriteStatusMessage("AV session is connecting with " + remoteUri);
            }
            catch (COMException ex)
            {
                WriteStatusMessage("AV connection failed. Error: " + ex.Message);
                this.uccControl.AVSession = null;
            }
        }

        /// <summary>
        /// End a call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEndCall_Click(object sender, EventArgs e)
        {
            if (this.uccControl.AVSession == null)
            {
                WriteStatusMessage("ERROR: There is no active AV session.");
                return;
            }
            try
            {
                WriteStatusMessage("Ending AV session ...");
                this.uccControl.EndAVSession();
            }
            catch (COMException ex)
            {
                WriteStatusMessage("Failed to end AV session. Error:" + ex.Message);
            }

        }

        ///// <summary>
        ///// Catch keyup events in IM input text box. When Enter is hit, send the message
        ///// and clear the text for further input.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void textBoxIMInput_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            this.uccControl.SessionSendMessage(this.textBoxIMInput.Text);
        //            this.textBoxIMInput.Text = string.Empty;
        //        }
        //    }
        //    catch (COMException ex)
        //    {
        //        WriteStatusMessage(ex.Message);
        //    }
        //}

        /// <summary>
        /// Write message to textBoxInfo box.
        /// </summary>
        /// <param name="message"></param>
        public void WriteStatusMessage(string message)
        {
            this.statusList.Items.Add(message);
            this.textBoxIMInput.Focus();
        }

        /// <summary>
        /// Write instance message to textBoxIM.
        /// </summary>
        /// <param name="message"></param>

        public void WriteIMMessage(string message)
        {
            this.statusList.Items.Add(message);
            this.textBoxIMInput.Focus();
        }

        /// <summary>
        /// Set instance message related buttons when there is no active IM sesssion
        /// </summary>
        public void SetIMOn()
        {
            this.buttonBeginIm.IsEnabled = true;
            this.buttonEndIm.IsEnabled = false;
            this.textBoxIMInput.IsReadOnly = true;
        }

        /// <summary>
        /// Set instance message related buttons when there is active IM sesssion
        /// </summary>
        public void SetIMOff()
        {
            this.buttonBeginIm.IsEnabled = false;
            this.buttonEndIm.IsEnabled = true;
            this.textBoxIMInput.IsReadOnly = false;
            this.textBoxIMInput.Focus();
        }

        /// <summary>
        /// Set VoIP related buttons' state
        /// when there is no active AV sesssion
        /// </summary>
        public void SetVoipOn()
        {
            this.buttonBeginCall.IsEnabled = true;
            this.buttonEndCall.IsEnabled = false;
        }

        /// <summary>
        /// Set VoIP related buttons's state
        /// when there is an active AV sesssion
        /// </summary>
        public void SetVoIPOff()
        {
            this.buttonBeginCall.IsEnabled = false;
            this.buttonEndCall.IsEnabled = true;
        }

        /// <summary>
        /// Set related buttons' state after login.
        /// </summary>
        public void SetButtonsAfterLogin()
        {
            SetIMOn();
            SetVoipOn();
            this.buttonLogin.IsEnabled = false;
            this.buttonLogout.IsEnabled = true;
        }

        /// <summary>
        /// Set related buttons' state after logout
        /// </summary>
        public void SetButtonsAfterLogout()
        {
            this.buttonBeginIm.IsEnabled = false;
            this.buttonEndIm.IsEnabled = false;
            this.textBoxIMInput.IsReadOnly = true;
            this.buttonBeginCall.IsEnabled = false;
            this.buttonEndCall.IsEnabled = false;
            this.buttonLogin.IsEnabled = true;
            this.buttonLogout.IsEnabled = false;
        }

        private void buttonLoginWindow_Click(object sender, RoutedEventArgs e)
        {
            new Windows.Login().ShowDialog();
        }

        private void buttonChatWindow_Click(object sender, RoutedEventArgs e)
        {
            new Windows.Chat().Show();
        }

        private void buttonContactsWindow_Click(object sender, RoutedEventArgs e)
        {
            new Windows.Contacts().Show();
        }

        private void buttonPrefWindow_Click(object sender, RoutedEventArgs e)
        {
            new Windows.Preferences().Show();
        }
    }
}
