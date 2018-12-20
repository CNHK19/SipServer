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
	[ValueConversion(typeof(bool), typeof(string))]
	class ComposingConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (((bool)value))
				return @"typing";
			return @"";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
