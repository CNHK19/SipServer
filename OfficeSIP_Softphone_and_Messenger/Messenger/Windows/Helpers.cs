// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace Messenger.Windows
{
	class Helpers
	{
        private const long SIZE_LIMIT = 10;

		public static void ListGridView_UpdateColumnWidth(ListView listView, SizeChangedEventArgs e, int stretchColumn)
		{
			if (e.WidthChanged)
			{
				GridView gridView = listView.View as GridView;

				double total = 0;
				for (int i = 0; i < gridView.Columns.Count; i++)
					if (!Double.IsNaN(gridView.Columns[i].Width) && i != stretchColumn)
						total += gridView.Columns[i].Width;

				double width = listView.ActualWidth - total - 28;

				gridView.Columns[stretchColumn].Width = (width < 30) ? 30 : width;
			}
		}

        public static string SizeToStr(long Size)
        {
            if (Size < (SIZE_LIMIT * 1024))
                return Size.ToString("F0") + " B";

            if (Size < (SIZE_LIMIT * 1024 * 1024))
                return (Size / 1024.0).ToString("F1") + " KB";

            if (Size < (SIZE_LIMIT * 1024 * 1024 * 1024))
                return (Size / 1024.0 / 1024.0).ToString("F1") + " MB";

            return (Size / 1024.0 / 1024.0 / 1024.0).ToString("F1") + " GB";
        }

		#region SHGetSpecialFolderPath

		[DllImport("shell32.dll")]
		public static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

		public const int CSIDL_PROFILE = 0x0028;        // USERPROFILE
		public const int CSIDL_PERSONAL = 0x0005;		// C:\Documents and Settings\username\My Documents

		#endregion
	}
}
