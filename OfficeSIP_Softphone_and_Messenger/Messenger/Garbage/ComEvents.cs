// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
/*
namespace Uccapi
{
	/// <summary>
	/// Com Events Helper
	/// Source: Uccapi.chm::Code Listing: Basic Event Registration and Other Helper Methods in C#
	/// </summary>
	public class ComEvents
	{
		static Dictionary<string, int> cookieJar;

		static ComEvents()
        {
            cookieJar = new Dictionary<string, int>();
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

            cookieJar.Add(key, cookie);
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
				int cookie = cookieJar[key];

				IConnectionPointContainer container = (IConnectionPointContainer)source;
				IConnectionPoint cp;
				Guid guid = typeof(T).GUID;

				container.FindConnectionPoint(ref guid, out cp);
				cp.Unadvise(cookie);

				cookieJar.Remove(key);
			}
		}
	}
}
*/