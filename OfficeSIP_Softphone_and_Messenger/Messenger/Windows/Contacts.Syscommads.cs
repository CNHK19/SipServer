// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.ComponentModel;

namespace Messenger.Windows
{
    public partial class Contacts
    {
        private HwndSource hwndSource;
        public event CancelEventHandler Minimizing;
        public event CancelEventHandler Maximizing;

        private void InitializeSyscommands()
        {
            this.Loaded += new RoutedEventHandler(Window_Loaded2);
        }

        void Window_Loaded2(object sender, RoutedEventArgs e)
        {
            hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32.WM_SYSCOMMAND)
            {
                if (wParam == (IntPtr)Win32.SC_MAXIMIZE ||
                    wParam == (IntPtr)Win32.SC_UNDOCUMENTED_CAPTIONDCLICK)
                {
                    if (Maximizing != null)
                    {
                        CancelEventArgs eventArgs = new CancelEventArgs();
                        Maximizing(this, eventArgs);
                        handled = eventArgs.Cancel;
                    }
                }
                else if (wParam == (IntPtr)Win32.SC_MINIMIZE)
                {
                    if (Minimizing != null)
                    {
                        CancelEventArgs eventArgs = new CancelEventArgs();
                        Minimizing(this, eventArgs);
                        handled = eventArgs.Cancel;
                    }
                }
            }

            return IntPtr.Zero;
        }
    }
}
