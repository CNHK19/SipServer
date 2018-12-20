// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Messenger.Windows;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;

namespace Messenger
{
	partial class Programme
	{
		private string crashReport;

		void InitializeCrashHandler()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		}

		//[DllImport("clrdump.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		//static extern Int32 CreateDump(Int32 ProcessId, string FileName,
		//    Int32 DumpType, Int32 ExcThreadId, IntPtr ExtPtrs);

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception)
				crashReport = CreateCrashReport(e.ExceptionObject as Exception);
			else
				crashReport = CreateCrashReport(null);

			Thread newThread = new Thread(new ThreadStart(CrashThread));
			newThread.SetApartmentState(ApartmentState.STA);
			newThread.IsBackground = false;
			newThread.Start();
			newThread.Join();

			System.Environment.Exit(0);
		}

		private void CrashThread()
		{
			//Crash crashWindow = new Crash();
			//crashWindow.Report.Text = crashReport;
			//crashWindow.Closed += new EventHandler(CrashWindow_Closed);
			//crashWindow.Show();

			Crash2 crashWindow = new Crash2();
			crashWindow.Closed += new EventHandler(CrashWindow_Closed);
			crashWindow.Report = crashReport;
			crashWindow.Show();

			System.Windows.Threading.Dispatcher.Run();
		}

		private void CrashWindow_Closed(object sender, EventArgs e)
		{
			System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
		}

		private string CreateCrashReport(Exception ex)
		{
			string report = "";

			try
			{
				report += AssemblyInfo.AssemblyProduct + " " + AssemblyInfo.AssemblyVersion + "\r\n";

				report += System.Environment.OSVersion.ToString() + "\r\n";
				report += ".NET Framework " + System.Environment.Version.ToString() + "\r\n";

				if (ex != null)
				{
					report += "Exception\r\n";

					report += "Message: " + ex.Message + "\r\n";
					report += "TargetSite: " + ex.TargetSite + "\r\n";
					report += "Source: " + ex.Source + "\r\n";
					report += "StackTrace: \r\n" + ex.StackTrace + "\r\n";
					report += "\r\nToString: \r\n" + ex.ToString() + "\r\n";
				}
				else
				{
					report += "No Exception\r\n";
					report += "StackTrace: \r\n";

					StackTrace stackTrace = new StackTrace(true);

					for (int i = 0; i < stackTrace.FrameCount; i++)
					{
						StackFrame stackFrame = stackTrace.GetFrame(i);

						report += stackFrame.GetMethod() + "\r\n";
					}
				}

				report += "\r\nEnvironment.StackTrace: \r\n" + System.Environment.StackTrace + "\r\n";

				report += "\r\nApplication Settings: \r\n";
				try
				{
					if (Messenger.Properties.Settings.Default.PropertyValues.Count > 0)
					{
						foreach (System.Configuration.SettingsPropertyValue setting in Messenger.Properties.Settings.Default.PropertyValues)
							if (setting.Name != @"Password")
								report += setting.Name + ": " + setting.PropertyValue.ToString() + "\r\n";
					}
					else
					{
						report += "Settings not loaded yet\r\n";
					}
				}
				catch
				{
					report += "Failed to report settings\r\n";
				}

				try
				{
					report += "\r\nBug traces: \r\n";
					report += BugTracer.GetTraces() + "\r\n";
				}
				catch
				{
					report += "Bug traces failed\r\n";
				}

				report += "\r\n-- end of report --\r\n";
			}
			catch
			{
			}

			return report;
		}
	}
}
