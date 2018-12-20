// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Uccapi
{
	/// <summary>
	/// Com Events Helper
	/// Source: Uccapi.chm::Code Listing: Basic Event Registration and Other Helper Methods in C#
	/// </summary>
	class ComEvents
	{
		private class Advisory
		{
			public Advisory(object source, Guid guid, int sinkHash, int cookie)
			{
				this.Source = source;
				this.Guid = guid;
				this.SinkHash = sinkHash;
				this.Cookie = cookie;
			}

			public object Source { get; private set; }
			public Guid Guid { get; private set; }
			public int SinkHash { get; private set; }
			public int Cookie { get; private set; }
		}

		static Dictionary<string, Advisory> cookieJar;

		static ComEvents()
		{
			cookieJar = new Dictionary<string, Advisory>();
		}

		private static string GetKey(IntPtr i, int j, int k)
		{
			return i.ToString() + j.ToString() + k.ToString();
		}

		public static void Advise<T>(object source, T sink)
		{
			if (source == null || sink == null)
			{
				throw new ArgumentNullException("source", "AdviseForEvents<T>: Source and sink cannot be null");
			}

			IntPtr i = Marshal.GetIUnknownForObject(source);
			int j = sink.GetHashCode();
			int k = typeof(T).GetHashCode();
			string key = GetKey(i, j, k);

			if (cookieJar.ContainsKey(key))
			{
				return;   // already advise the source of the sink.
			}

			IConnectionPoint cp;
			int cookie;
			IConnectionPointContainer container = (IConnectionPointContainer)source;
			Guid guid = typeof(T).GUID;
			container.FindConnectionPoint(ref guid, out cp);
			cp.Advise(sink, out cookie);

			cookieJar.Add(key, new Advisory(source, guid, j, cookie));
		}

		public static void Unadvise<T>(object source, T sink)
		{
			if (source == null || sink == null)
			{
				throw new ArgumentNullException("source", "UnadviseForEvents<T>: Source and sink cannot be null");
			}

			IntPtr i = Marshal.GetIUnknownForObject(source);
			int j = sink.GetHashCode();
			int k = typeof(T).GetHashCode();
			string key = GetKey(i, j, k);

			// avoid exception, check to see if cookieJar
			// has key value before referencing item using
			// index syntax
			if (cookieJar.ContainsKey(key))
			{
				int cookie = cookieJar[key].Cookie;

				IConnectionPointContainer container = (IConnectionPointContainer)source;
				IConnectionPoint cp;
				Guid guid = typeof(T).GUID;

				container.FindConnectionPoint(ref guid, out cp);
				cp.Unadvise(cookie);

				cookieJar.Remove(key);
			}
		}

		public static void UnadviseAll(object sink)
		{
			List<string> removeKeys = new List<string>();
			int j = sink.GetHashCode();

			foreach (KeyValuePair<string, Advisory> advisory in cookieJar)
			{
				if (advisory.Value.SinkHash == j)
				{
					int cookie = advisory.Value.Cookie;

					IConnectionPointContainer container = (IConnectionPointContainer)advisory.Value.Source;
					IConnectionPoint cp;
					Guid guid = advisory.Value.Guid;

					container.FindConnectionPoint(ref guid, out cp);
					cp.Unadvise(cookie);

					removeKeys.Add(advisory.Key);
				}
			}

			foreach(string key in removeKeys)
				cookieJar.Remove(key);
		}
	}
}
