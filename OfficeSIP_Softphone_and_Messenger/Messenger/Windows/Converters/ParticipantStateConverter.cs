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
using Uccapi;

namespace Messenger.Windows
{
	[ValueConversion(typeof(PartipantLogState), typeof(string))]
	class ParticipantStateConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is PartipantLogState)
			{

				switch ((PartipantLogState)value)
				{
					case PartipantLogState.Null:
						return @"";
					case PartipantLogState.Local:
						return @"Connected";
					case PartipantLogState.InvalidUri:
						return @"Invalid contact";

					case PartipantLogState.AddBegin:
						return @"Connecting";

					case PartipantLogState.AddFailed:
						return @"Failed";

					case PartipantLogState.AddSuccess:
						return @"Connected";

					case PartipantLogState.RemoveFailed:
						return @"Remove failed";
					case PartipantLogState.RemoveSuccess:
						return @"Removed";

					case PartipantLogState.SessionTerminated:
						return @"Session terminated";

					// UCC_SESSION_ENTITY_STATE
					case PartipantLogState.Connecting:
						return @"Connecting";
					case PartipantLogState.Connected:
						return @"Connected";
					case PartipantLogState.Disconnected:
						return @"Disconnected";
					case PartipantLogState.Disconnecting:
						return @"Disconnecting";
					case PartipantLogState.Idle:
						return @"Idle";
				}
			}

			return @"";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
