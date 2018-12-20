// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Media;
using Messenger;
using Uccapi;

namespace Messenger.Windows
{
	[ValueConversion(typeof(AvailabilityValues), typeof(ImageSource))]
	[ValueConversion(typeof(AvailabilityValues), typeof(ControlTemplate))]
	[ValueConversion(typeof(AvailabilityValues), typeof(string))]
	class StateConverter 
		: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
			string parameterString = (parameter == null) ? @"" : (parameter as string);

			if (value is AvailabilityValues)
            {
				if (targetType == typeof(ImageSource))
				{
					try
					{
						switch ((AvailabilityValues)value)
						{
							//presenceBlock
							case AvailabilityValues.Online:
								result = Application.Current.FindResource(@"presenceOnline");
								break;
							case AvailabilityValues.Away:
								result = Application.Current.FindResource(@"presenceAway");
								break;
							case AvailabilityValues.BeRightBack:
							case AvailabilityValues.Idle:
								result = Application.Current.FindResource("presenceIdleOnline");
								break;
							case AvailabilityValues.Busy:
								result = Application.Current.FindResource("presenceBusy");
								break;
							case AvailabilityValues.BusyIdle:
								result = Application.Current.FindResource("presenceIdleBusy");
								break;
							case AvailabilityValues.DoNotDisturb:
								result = Application.Current.FindResource("presenceDnd");
								break;
							case AvailabilityValues.Offline:
								result = Application.Current.FindResource("presenceOffline");
								break;
							case AvailabilityValues.Unknown:
							default:
								result = Application.Current.FindResource("presenceOffline");
								//result = Application.Current.FindResource("presenceUnknown");
								break;
						}
					}
					catch (Exception)
					{
					}
				}
				else if (targetType == typeof(ControlTemplate))
				{
					try
					{
						switch ((AvailabilityValues)value)
						{
							case AvailabilityValues.Online:
								result = Application.Current.FindResource("GreenBall");
								break;
							case AvailabilityValues.Away:
							case AvailabilityValues.BeRightBack:
							case AvailabilityValues.Idle:
								result = Application.Current.FindResource("YellowBall");
								break;
							case AvailabilityValues.Busy:
							case AvailabilityValues.BusyIdle:
							case AvailabilityValues.DoNotDisturb:
								result = Application.Current.FindResource("RedBall");
								break;
							case AvailabilityValues.Unknown:
							case AvailabilityValues.Offline:
							default:
								result = Application.Current.FindResource("GreyBall");
								break;
						}
					}
					catch (Exception)
					{
					}
				}
				else //if (targetType == typeof(string))
				{
					switch ((AvailabilityValues)value)
					{
						case AvailabilityValues.Online:
							result = BaseCommands.LoadString(@"Available");
							break;
						case AvailabilityValues.Away:
							result = BaseCommands.LoadString(@"Away");
							break;
						case AvailabilityValues.Idle:
							result = BaseCommands.LoadString(@"Idle");
							break;
						case AvailabilityValues.BeRightBack:
							result = BaseCommands.LoadString(@"BeRightBack");
							break;
						case AvailabilityValues.Busy:
							result = BaseCommands.LoadString(@"Busy");
							break;
						case AvailabilityValues.BusyIdle:
							result = BaseCommands.LoadString(@"BusyAway");
							break;
						case AvailabilityValues.DoNotDisturb:
							result = BaseCommands.LoadString(@"DoNotDisturb");
							break;
						case AvailabilityValues.Offline:
							if (parameterString == @"NoAppearOffline")
								goto case AvailabilityValues.Unknown;
							result = BaseCommands.LoadString(@"AppearOffline");
							break;
						case AvailabilityValues.Unknown:
						default:
							result = BaseCommands.LoadString(@"Offline");
							break;
					}
				}
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
