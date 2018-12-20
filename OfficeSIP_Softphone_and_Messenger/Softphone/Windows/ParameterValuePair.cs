// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;

namespace Messenger.Windows
{
	/// <summary>
	/// ParameterValuePair for Session Details ListView
	/// </summary>
	public class ParameterValuePair
		: FrameworkElement
	{
		public static readonly DependencyProperty ValueProperty;

		static ParameterValuePair()
		{
			ValueProperty = DependencyProperty.Register(@"Value", typeof(string), typeof(ParameterValuePair));
		}

		public string Parameter
		{
			get;
			set;
		}

		public string Value
		{
			set { SetValue(ValueProperty, value); }
			get { return (string)GetValue(ValueProperty); }
		}
	}
}
