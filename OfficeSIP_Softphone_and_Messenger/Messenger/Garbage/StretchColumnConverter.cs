// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using Messenger;

namespace Messenger.Windows
{
	// using: Width="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ListView}}, Converter={StaticResource StretchColumnConverter}}"
	// не работает до конца -- вызывается только один раз, не реагирует на изменение размера
	[ValueConversion(typeof(Double), typeof(Double))]
	class StretchColumnConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ListView listView = value as ListView;
			GridView gridView = listView.View as GridView;

			double total = 0;
			for (int i = 0; i < gridView.Columns.Count; i++)
				if (!Double.IsNaN(gridView.Columns[i].Width) && i != 1)
					total += gridView.Columns[i].Width;

			return listView.ActualWidth - total; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
