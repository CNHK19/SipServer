// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;

namespace Messenger.Windows
{
	[ValueConversion(typeof(string[]), typeof(Visibility))]
	[ValueConversion(typeof(string), typeof(Visibility))]
	[ValueConversion(typeof(bool), typeof(Visibility))]
	[ValueConversion(typeof(int), typeof(Visibility))]
	[ValueConversion(typeof(object), typeof(Visibility))]
	class VisibilityConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;

			if (value is string)
				if ((value as string).Length == 0)
					return Visibility.Collapsed;
			
			if (value is string[])
				if ((value as string[]).Length == 0)
					return Visibility.Collapsed;
			
			if (value is bool)
				if ((bool)value == false)
					return Visibility.Collapsed;

			if (value is int)
				if ((int)value == 0)
					return Visibility.Collapsed;

			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
