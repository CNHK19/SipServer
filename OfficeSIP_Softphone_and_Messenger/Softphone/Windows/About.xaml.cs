// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;

namespace Messenger.Windows
{
    /// <summary>
    /// SOFTPHONE
    /// </summary>
    public partial class About 
		: WindowEx
    {
        public About()
        {
            InitializeComponent();
        }

		private void OkBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Close();
		}
    }
}
