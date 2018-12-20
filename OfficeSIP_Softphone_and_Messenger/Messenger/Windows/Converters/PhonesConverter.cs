// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using Uccapi;

namespace Messenger.Windows
{
	[ValueConversion(typeof(IPhoneLine[]), typeof(string))]
	class PhonesConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string result = "";

			if (value != null)
			{
				IPhoneLine[] phones = value as IPhoneLine[];

				foreach (IPhoneLine phone in phones)
					result += (string.IsNullOrEmpty(result) ? "" : "\r\n") + phone.Uri;
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
