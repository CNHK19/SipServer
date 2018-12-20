// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;

namespace Messenger
{
	public enum BugId
	{
		//Message: Исключение из HRESULT: 0x80EE0081
		//TargetSite: Void AddParticipant(Microsoft.Office.Interop.UccApi.IUccSessionParticipant, Microsoft.Office.Interop.UccApi.UccOperationContext)
		//Source: Microsoft.Office.Interop.UccApi
		//StackTrace: 
		//   в Microsoft.Office.Interop.UccApi.IUccSession.AddParticipant(IUccSessionParticipant pParticipant, UccOperationContext pOperationContext)
		//   в Uccapi.Session.AddPartipant(String uri)
		//   в Messenger.Programme.StartConversation(SessionType sessionType, Boolean enableVideo)
		//   в Messenger.Programme.SendInstantMessageBinding_Executed(Object sender, ExecutedRoutedEventArgs e)
		N01 = 0,
	}

	public class BugTracer
	{
		private object sync = new object();
		private List<string> messages = new List<string>();

		public bool Tracing
		{
			get;
			set;
		}

		public void BeginTrace(string message)
		{
			lock (sync)
			{
				messages.Clear();
				Tracing = true;
				Trace(message);
			}
		}

		public void EndTrace(string message)
		{
			lock (sync)
			{
				Trace(message);
				Tracing = false;
			}
		}

		public void Trace(string message)
		{
			lock (sync)
			{
				if (Tracing)
				{
					if (messages.Count > 100)
						messages.RemoveAt(0);

					messages.Add(message);
				}
			}
		}

		public string GetTrace()
		{
			lock (sync)
			{
				string trace = "";

				foreach (var message in messages)
					trace += message + "\r\n";

				return trace + "[end]";
			}
		}



		private static BugTracer[] bugTracers;

		static BugTracer()
		{
			int count = Enum.GetNames(typeof(BugId)).Length;

			bugTracers = new BugTracer[count];
			for (int i = 0; i < count; i++)
				bugTracers[i] = new BugTracer();
		}

		public static BugTracer Get(BugId bugId)
		{
			return bugTracers[(int)bugId];
		}

		public static string GetTraces()
		{
			string traces = "";

			foreach (var tracer in bugTracers)
			{
				traces += tracer.GetTrace();
			}

			return traces;
		}
	}
}
