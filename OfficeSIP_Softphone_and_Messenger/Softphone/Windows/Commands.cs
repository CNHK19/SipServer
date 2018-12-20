// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows.Input;

namespace Messenger.Windows
{
	/// <summary>
	/// SOFTPHONE
	/// </summary>
	public class Commands
		: BaseCommands
	{
		public static RoutedUICommand Ok { get; private set; }
		public static RoutedUICommand Apply { get; private set; }
		public static RoutedUICommand Cancel { get; private set; }
		public static RoutedUICommand PhoneDigit { get; private set; }
		public static RoutedUICommand About { get; private set; }
		public static RoutedUICommand Homepage { get; private set; }
		public static RoutedUICommand Preferences { get; private set; }
		public static RoutedUICommand Feedback { get; private set; }
		public static RoutedUICommand GotoUrl { get; private set; }
		public static RoutedUICommand GotoEmail { get; private set; }
		public static RoutedUICommand Login { get; private set; }
		public static RoutedUICommand LoginAs { get; private set; }
		public static RoutedUICommand Logout { get; private set; }
		public static RoutedUICommand Close { get; private set; }
		public static RoutedUICommand UccTracing { get; private set; }
		public static RoutedUICommand InvokeTuningWizard { get; private set; }
		public static RoutedUICommand ResetTuningWizardSettings { get; private set; }
		public static RoutedUICommand Refresh { get; private set; }
		public static RoutedUICommand OpenFolder { get; private set; }
		public static RoutedUICommand View { get; private set; }
		public static RoutedUICommand Call { get; private set; }
		public static RoutedUICommand HangUp { get; private set; }
		public static RoutedUICommand AcceptCall { get; private set; }
		public static RoutedUICommand RejectCall { get; private set; }
		public static RoutedUICommand ViewDialpad { get; private set; }
		public static RoutedUICommand ViewSessionDetails { get; private set; }
		public static RoutedUICommand ViewIncomingCalls { get; private set; }
		public static RoutedUICommand ViewLocalVideo { get; private set; }
		public static RoutedUICommand CopyAll { get; private set; }
		public static RoutedUICommand Redial { get; private set; }
		public static RoutedUICommand Remove { get; private set; }
		public static RoutedUICommand RemoveAll { get; private set; }

		static Commands()
		{
			Initialize();

			//Ok = new RoutedUICommand(@"Ok", @"Ok", typeof(Commands));
			//Apply = new RoutedUICommand(@"Apply", @"Apply", typeof(Commands));
			//Cancel = new RoutedUICommand(@"Cancel", @"Cancel", typeof(Commands));
			////PhoneDigit = new RoutedUICommand(@"PhoneDigit", @"PhoneDigit", typeof(Commands));
			//About = new RoutedUICommand("About", "About", typeof(Commands));
			//Homepage = new RoutedUICommand("Homepage", "Homepage", typeof(Commands));
			//Preferences = new RoutedUICommand("Preferences", "Preferences", typeof(Commands));
			//Feedback = new RoutedUICommand("Feedback", "Feedback", typeof(Commands));
			//GotoUrl = new RoutedUICommand("Go to Url", "GotoUrl", typeof(Commands));
			//GotoEmail = new RoutedUICommand("Go to Email", "GotoEmail", typeof(Commands));
			//Login = new RoutedUICommand("Login", "Login", typeof(Commands), CreateGesture(Key.L, ModifierKeys.Control));
			//LoginAs = new RoutedUICommand("Login As", "LoginAs", typeof(Commands));
			//Logout = new RoutedUICommand("Logout", "Logout", typeof(Commands));
			//Close = new RoutedUICommand("Close", "Close", typeof(Commands));
			//UccTracing = new RoutedUICommand("Troubleshooting", "UccTracing", typeof(Commands));
			//InvokeTuningWizard = new RoutedUICommand("Tuning Wizard", "InvokeTuningWizard", typeof(Commands), CreateGesture(Key.W, ModifierKeys.Control));
			//ResetTuningWizardSettings = new RoutedUICommand("Reset Wizard Settings", "ResetTuningWizardSettings", typeof(Commands));
			//Refresh = new RoutedUICommand("Refresh", "Refresh", typeof(Commands));
			//OpenFolder = new RoutedUICommand("Open Containing Folder", "OpenFolder", typeof(Commands));
			//View = new RoutedUICommand("View", "View", typeof(Commands));
			//Call = new RoutedUICommand("Call", "Call", typeof(Commands));
			//HangUp = new RoutedUICommand("End Call", "EndCall", typeof(Commands), CreateGesture(Key.E, ModifierKeys.Control));
			//AcceptCall = new RoutedUICommand("Accept", "AcceptCall", typeof(Commands));
			//RejectCall = new RoutedUICommand("Reject", "RejectCall", typeof(Commands));
			//ViewDialpad = new RoutedUICommand("Dial Pad", "ViewDialpad", typeof(Commands), CreateGesture(Key.D, ModifierKeys.Control));
			//ViewSessionDetails = new RoutedUICommand("Session Details", "ViewSessionDetails", typeof(Commands), CreateGesture(Key.T, ModifierKeys.Control));
			//ViewIncomingCalls = new RoutedUICommand("Incoming Calls", "ViewIncomingCalls", typeof(Commands), CreateGesture(Key.I, ModifierKeys.Control));
			//ViewLocalVideo = new RoutedUICommand("Local Video", "ViewLocalVideo", typeof(Commands), CreateGesture(Key.M, ModifierKeys.Control));
			//CopyAll = new RoutedUICommand("Copy All", "CopyAll", typeof(Commands));
			//Redial = new RoutedUICommand("Re-Dial", "Redial", typeof(Commands));
			//Remove = new RoutedUICommand("Remove", "Remove", typeof(Commands));
			//RemoveAll = new RoutedUICommand("Remove All", "RemoveAll", typeof(Commands));
		}

		public static void Initialize()
		{
			if (Ok == null)
			{
				Ok = CreateRoutedUICommand(@"Ok", typeof(Commands));
				Apply = CreateRoutedUICommand(@"Apply", typeof(Commands));
				Cancel = CreateRoutedUICommand(@"Cancel", typeof(Commands));
				PhoneDigit = CreateRoutedUICommand(@"PhoneDigit", typeof(Commands));
				About = CreateRoutedUICommand("About", typeof(Commands));
				Homepage = CreateRoutedUICommand("Homepage", typeof(Commands));
				Preferences = CreateRoutedUICommand("Preferences", typeof(Commands));
				Feedback = CreateRoutedUICommand("Feedback", typeof(Commands));
				GotoUrl = CreateRoutedUICommand("GotoUrl", typeof(Commands));
				GotoEmail = CreateRoutedUICommand("GotoEmail", typeof(Commands));
				Login = CreateRoutedUICommand("Login", typeof(Commands), CreateGesture(Key.L, ModifierKeys.Control));
				LoginAs = CreateRoutedUICommand("LoginAs", typeof(Commands));
				Logout = CreateRoutedUICommand("Logout", typeof(Commands));
				Close = CreateRoutedUICommand("Close", typeof(Commands));
				UccTracing = CreateRoutedUICommand("UccTracing", typeof(Commands));
				InvokeTuningWizard = CreateRoutedUICommand("InvokeTuningWizard", typeof(Commands), CreateGesture(Key.W, ModifierKeys.Control));
				ResetTuningWizardSettings = CreateRoutedUICommand("ResetTuningWizardSettings", typeof(Commands));
				Refresh = CreateRoutedUICommand("Refresh", typeof(Commands));
				OpenFolder = CreateRoutedUICommand("OpenFolder", typeof(Commands));
				View = CreateRoutedUICommand("View", typeof(Commands));
				Call = CreateRoutedUICommand("Call", typeof(Commands));
				HangUp = CreateRoutedUICommand("EndCall", typeof(Commands), CreateGesture(Key.E, ModifierKeys.Control));
				AcceptCall = CreateRoutedUICommand("AcceptCall", typeof(Commands));
				RejectCall = CreateRoutedUICommand("RejectCall", typeof(Commands));
				ViewDialpad = CreateRoutedUICommand("ViewDialpad", typeof(Commands), BaseCommands.CreateGesture(Key.D, ModifierKeys.Control));
				ViewSessionDetails = CreateRoutedUICommand("ViewSessionDetails", typeof(Commands), CreateGesture(Key.T, ModifierKeys.Control));
				ViewIncomingCalls = CreateRoutedUICommand("ViewIncomingCalls", typeof(Commands), CreateGesture(Key.I, ModifierKeys.Control));
				ViewLocalVideo = CreateRoutedUICommand("ViewLocalVideo", typeof(Commands), CreateGesture(Key.M, ModifierKeys.Control));
				CopyAll = CreateRoutedUICommand("CopyAll", typeof(Commands));
				Redial = CreateRoutedUICommand("Redial", typeof(Commands));
				Remove = CreateRoutedUICommand("Remove", typeof(Commands));
				RemoveAll = CreateRoutedUICommand("RemoveAll", typeof(Commands));
			}
		}
	}
}
