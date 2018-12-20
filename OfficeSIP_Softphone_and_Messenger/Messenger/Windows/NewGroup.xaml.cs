// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for NewGroup.xaml
	/// </summary>
	public partial class NewGroup : Window
	{
		private OkEnabler okEnabler;

		public NewGroup()
		{
			okEnabler = new OkEnabler(this);

			InitializeComponent();

			Group = @"";

			this.DataContext = this;
		}

		public string Group
		{
			get;
			set;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
