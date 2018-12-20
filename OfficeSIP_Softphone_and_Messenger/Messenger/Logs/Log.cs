// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Logs
{
	public class Log
		: IEnumerable<LogItem>
	{
		private object sync;
		private List<LogItem> items; 

		public Log()
		{
			sync = new object();
			items = new List<LogItem>();
		}

		public object SyncRoot
		{
			get { return sync; }
		}

		public void Add(LogItem item)
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)items;
		}

		IEnumerator<LogItem> IEnumerable<LogItem>.GetEnumerator()
		{
			return (IEnumerator<LogItem>)items;
		}
	}
}
