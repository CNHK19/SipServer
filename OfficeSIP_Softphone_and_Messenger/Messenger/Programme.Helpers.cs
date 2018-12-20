// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Uccapi;

namespace Messenger
{
	public partial class Programme : Application
	{
		private SplashScreen screen;

		private string GetCommandLineArgValue(string argName)
		{
			string[] args = Environment.GetCommandLineArgs();
			foreach (string arg in args)
				if (arg.StartsWith(argName))
					return arg.Substring(argName.Length).Trim();

			return null;
		}

		private void CloseSplash()
		{
			// Fix:
			//	Source: WindowsBase
			//	StackTrace: 
			//		в MS.Win32.UnsafeNativeMethods.SetActiveWindow(HandleRef hWnd)
			//		в System.Windows.SplashScreen.Close(TimeSpan fadeoutDuration)
			//		в System.Windows.SplashScreen.<Show>b__0(Object splashObj)
			// https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=378575

			if (this.screen != null)
			{
				bool repeatClose = true;

				while (repeatClose)
					try
					{
						this.screen.Close(new TimeSpan(0, 0, 0, 0, 500));
						this.screen = null;
						repeatClose = false;
					}
					catch (System.ComponentModel.Win32Exception)
					{
					}
			}
		}

		public T FindWindow<T>() where T : Window
		{
			foreach (Window window in this.Windows)
				if (window is T)
					return (window as T);
			return null;
		}

		public bool ActivateWindow<T>() where T : Window
		{
			T window = FindWindow<T>();
			if (window != null)
			{
				window.Activate();
				return true;
			}
			return false;
		}
	}
}
