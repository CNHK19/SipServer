// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;

namespace Uccapi
{
	//Unknown: 0 to 2999. Availability is undefined. 
	//Online: 3000 to 4499. Willing and able to communicate. 
	//Idle: 4500 to 5999. Willing but potentially unable to communicate. 
	//Busy: 6000 to 7499. Able but potentially unwilling to communicate. 
	//BusyIdle: 7500 to 8999. Able but potentially unwilling to communicate. 
	//DoNotDisturb: 9000 to 11999. Able but potentially unwilling to communicate. 
	//Away: 12000 to 17999. Willing but unable to communicate. 
	//Offline: 18000 and higher. Not available to communicate

	//Available 3500
	//Busy 6500
	//DND (includes urgent interruption for people in Team container) 9500
	//Be Right Back 12500
	//Away 15500
	//Offline 18500

	// machine state
	//Active (Busy) 3500
	//Inactive	3750
	//Away 15500
	//Offline 18500

	//public enum UserStateValues
	//{
	//    Available = 3500,
	//    Busy = 6500,
	//    DND = 9500,
	//    BeRightBack = 12500,
	//    Away = 15500,
	//    Offline = 18500
	//}

	//public class UserStateHelper
	//{/*
	//    public static UserStateValues ConvertFromAvailability(AvailabilityValues value)
	//    {
	//        switch (value)
	//        {
	//            case AvailabilityValues.Unknown:
	//            case AvailabilityValues.Offline:
	//                return UserStateValues.Offline;

	//            case AvailabilityValues.Online:
	//                return UserStateValues.Available;

	//            case AvailabilityValues.DoNotDisturb:

	//        }
	//    }*/
	//}

	[System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
	public enum AvailabilityValues
	{
		Unknown = 0,
		Online = 3500,
		Idle = 5000,
		Busy = 6500,
		BusyIdle = 8000,
		DoNotDisturb = 9500,
		BeRightBack = 12500,
		Away = 15500,
		Offline = 18500
	}

	public class AvailabilityHelper
	{
		public static AvailabilityValues ConvertFromInt(int value)
		{
			if (value >= 0 && value <= 2999)
				return AvailabilityValues.Unknown;
			if (value >= 3000 && value <= 4499)
				return AvailabilityValues.Online;
			if (value >= 4500 && value <= 5999)
				return AvailabilityValues.Idle;
			if (value >= 6000 && value <= 7499)
				return AvailabilityValues.Busy;
			if (value >= 7500 && value <= 8999)
				return AvailabilityValues.BusyIdle;
			if (value >= 9000 && value <= 11999)
				return AvailabilityValues.DoNotDisturb;
			if (value >= 12000 && value <= 14999)
				return AvailabilityValues.BeRightBack;
			if (value >= 15000 && value <= 17999)
				return AvailabilityValues.Away;
			if (value >= 18000)
				return AvailabilityValues.Offline;

			return AvailabilityValues.Unknown;
		}
	}
}
