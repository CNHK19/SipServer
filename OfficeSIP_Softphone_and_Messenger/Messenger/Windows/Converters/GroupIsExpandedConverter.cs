// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Specialized;

namespace Messenger.Windows
{
	public class GroupIsExpandedConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values != null && values.Length >= 2)
			{
				string group = values[0] as string;
				StringCollection expandedGroups = values[1] as StringCollection;

				if (expandedGroups != null && group != null)
					return expandedGroups.Contains(group);
			}

			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
