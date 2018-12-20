// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Text;

namespace Messenger.Windows
{
    class WindowMonitor<T> where T : System.Windows.Window, new()
    {
        private T window;

        public void Show()
        {
            if (window == null)
            {
                window = new T();
                window.Closed += new EventHandler(Window_Closed);
                window.Show();
            }
            else
            {
                window.Focus();
            }
        }

        public T Window
        {
            get
            {
                Show();
                return window;
            }
        }

        private void Window_Closed(Object sender, EventArgs e)
        {
            window = null;
        }
    }
}
