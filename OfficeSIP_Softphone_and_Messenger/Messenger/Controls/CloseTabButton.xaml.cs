// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Messenger.Controls
{
    /// <summary>
    /// Interaction logic for CloseTabButton.xaml
    /// </summary>
    public partial class CloseTabButton : UserControl
    {
        public CloseTabButton()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Click;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
                Click.Invoke(sender, e);
        }
    }
}
