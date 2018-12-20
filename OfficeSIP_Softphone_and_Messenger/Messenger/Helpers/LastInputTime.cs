// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Runtime.InteropServices;

namespace Messenger.Helpers
{
	[StructLayout(LayoutKind.Sequential)]
	struct LASTINPUTINFO
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

		[MarshalAs(UnmanagedType.U4)]
		public int cbSize;
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 dwTime;
	}

	public class LastInputTime
	{
		[DllImport("user32.dll")]
		static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		public static int GetLastInputTime()
		{
			int idleTime = 0;
			LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
			lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
			lastInputInfo.dwTime = 0;

			int envTicks = Environment.TickCount;

			if (GetLastInputInfo(ref lastInputInfo))
			{
				int lastInputTick = (int)lastInputInfo.dwTime;

				idleTime = envTicks - lastInputTick;
			}

			return (idleTime > 0) ? (idleTime / 1000) : idleTime;
		}
	}
}
