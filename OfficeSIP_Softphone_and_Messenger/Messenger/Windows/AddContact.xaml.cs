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
using System.Windows.Shapes;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for AddContact.xaml
	/// </summary>
	public partial class AddContact : Window
	{
		private OkEnabler okEnabler;
		public event EventHandler<Result> Done;

		public class Result
			: EventArgs
		{
			public string Uri { get; set; }
			public string Group { get; set; }
		}

		public AddContact()
		{
			okEnabler = new OkEnabler(this);

			InitializeComponent();

			this.DataContext = new Result();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			if (Done != null)
			{
				var result = this.DataContext as Result;
				if (result.Group == null)
					result.Group = "";
				Done(this, result);
			}
		}

		//private void Cancel_Click(object sender, RoutedEventArgs e)
		//{
		//    this.Close();
		//}

		//private void OkBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		//{
		//    Close();
		//}

		//private void OkBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		//{
		//}

		private void CancelBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//(this.DataContext as Result).Uri = @"123";
			//			BindingOperations.GetBindingExpression(ur, TextBox.TextProperty).UpdateSource();
		}
	}
}
