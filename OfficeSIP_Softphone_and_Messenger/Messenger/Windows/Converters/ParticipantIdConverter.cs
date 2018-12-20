// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace Messenger.Windows
{
	[ValueConversion(typeof(int), typeof(Brush))]
	class ParticipantIdConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
				return GetBrush((int)value);

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public static Brush GetBrush(int id)
		{
			#region var brushes = new Brush[] {...}

			var brushes = new Brush[] 
			{
				Brushes.DarkBlue,
				Brushes.DarkRed,
				Brushes.DarkOrange,
				Brushes.DarkCyan,
				Brushes.DarkGoldenrod,
				Brushes.DarkGray,
				Brushes.DarkGreen,
				Brushes.DarkKhaki,
				Brushes.DarkMagenta,
				Brushes.DarkOliveGreen,
				Brushes.DarkOrchid,
				Brushes.DarkSalmon,
				Brushes.DarkSeaGreen,
				Brushes.DarkSlateBlue,
				Brushes.DarkSlateGray,
				Brushes.DarkTurquoise,
				Brushes.DarkViolet,
			};

			#endregion

			return brushes[id % brushes.Length];
		}
	}
}
