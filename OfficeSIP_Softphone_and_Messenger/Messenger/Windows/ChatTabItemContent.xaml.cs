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
using Uccapi;

namespace Messenger.Windows
{
    /// <summary>
    /// Interaction logic for ChatTabItemContent.xaml
    /// </summary>
    public partial class ChatTabItemContent : UserControl
    {
        public ChatTabItemContent()
        {
            InitializeComponent();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Helpers.ListGridView_UpdateColumnWidth(sender as ListView, e, 1);
        }

        private void ListView_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as ListView).SelectedIndex = -1;
        }
    }
}
