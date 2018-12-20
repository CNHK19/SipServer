// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Messenger
{
	public static class FlashWindow
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

		[StructLayout(LayoutKind.Sequential)]
		private struct FLASHWINFO
		{
			public uint cbSize;
			public IntPtr hwnd;
			public uint dwFlags;
			public uint uCount;
			public uint dwTimeout;
		}

		public const uint FLASHW_STOP = 0;
		public const uint FLASHW_CAPTION = 1;
		public const uint FLASHW_TRAY = 2;
		public const uint FLASHW_ALL = 3;
		public const uint FLASHW_TIMER = 4;
		public const uint FLASHW_TIMERNOFG = 12;

		public static bool Flash(Window window)
		{
			return Flash(window, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);
		}

		public static bool Start(Window window)
		{
			return Flash(window, FLASHW_ALL, uint.MaxValue, 0);
		}

		public static bool Stop(Window window)
		{
			return Flash(window, FLASHW_STOP, 0, 0);
		}

		public static bool Flash(Window window, uint flags, uint count)
		{
			return Flash(window, flags, count, 0);
		}

		public static bool Flash(Window window, uint flags, uint count, uint timeout)
		{
			if (Environment.OSVersion.Version.Major >= 5)
			{
				var wi = new WindowInteropHelper(window);

				var fi = new FLASHWINFO()
				{
					hwnd = wi.Handle,
					dwFlags = flags,
					uCount = count,
					dwTimeout = timeout,
				};

				fi.cbSize = (UInt32)(Marshal.SizeOf(fi));

				return FlashWindowEx(ref fi);
			}

			return false;
		}
	}
}
