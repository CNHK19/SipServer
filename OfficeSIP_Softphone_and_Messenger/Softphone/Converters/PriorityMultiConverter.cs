// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Specialized;

namespace Messenger.Windows
{
	public class PriorityMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values != null)
			{
				for (int i = 0; i < values.Length; i++)
					if (values[i] != null && values[i] != DependencyProperty.UnsetValue)
						return values[i];
			}

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
