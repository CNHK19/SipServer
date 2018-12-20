// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows.Threading;

namespace System.Windows.Threading.Ex
{
	/// <summary>
	/// Encapsulates a WPF dispatcher with added functionalities.
	/// </summary>
	public class DispatcherHelper
	{
		private static DispatcherOperationCallback exitFrameCallback = new
			 DispatcherOperationCallback(ExitFrame);

		/// <summary>
		/// Processes all UI messages currently in the message queue.
		/// </summary>
		public static void DoEvents()
		{
			// Create new nested message pump.
			DispatcherFrame nestedFrame = new DispatcherFrame();

			// Dispatch a callback to the current message queue, when getting called,
			// this callback will end the nested message loop.
			// note that the priority of this callback should be lower than that of UI event messages.
			DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.Background, exitFrameCallback, nestedFrame);

			// pump the nested message loop, the nested message loop will immediately
			// process the messages left inside the message queue.
			Dispatcher.PushFrame(nestedFrame);

			// If the "exitFrame" callback is not finished, abort it.
			if (exitOperation.Status != DispatcherOperationStatus.Completed)
			{
				exitOperation.Abort();
			}
		}

		private static Object ExitFrame(Object state)
		{
			DispatcherFrame frame = state as DispatcherFrame;

			// Exit the nested message loop.
			frame.Continue = false;
			return null;
		}
	}
}
