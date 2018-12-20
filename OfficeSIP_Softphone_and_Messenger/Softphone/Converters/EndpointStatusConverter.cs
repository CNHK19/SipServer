// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using Uccapi;

namespace Messenger.Windows
{
	[ValueConversion(typeof(EndpointStatus), typeof(string))]
	[ValueConversion(typeof(AvailabilityValues), typeof(ImageSource))]
	class EndpointStatusConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is EndpointStatus)
			{
				if (targetType == typeof(ImageSource))
				{
					switch ((EndpointStatus)value)
					{
						case EndpointStatus.Enabled:
							return Application.Current.FindResource(@"presenceOnline");
						case EndpointStatus.Disabled:
						case EndpointStatus.Enabling:
						case EndpointStatus.Disabling:
							return Application.Current.FindResource("presenceOffline");
					}
				}
				else
				{
					switch ((EndpointStatus)value)
					{
						case EndpointStatus.Enabled:
							return @"Connected";
						case EndpointStatus.Disabled:
							return @"Disconnected";
						case EndpointStatus.Enabling:
							return @"Connecting...";
						case EndpointStatus.Disabling:
							return @"Disconnecting...";
					}
				}
			}

			throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
