// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;

namespace Uccapi
{
	public interface IPresentity
		: INotifyPropertyChanged
	{
		string Uri { get; }
		string Aor { get; }

		string DisplayName { get; }
		string Group { get; set; }
		AvailabilityValues Availability { get; }
		string Homepage { get; }
		string Fax { get; }
		string[] Emails { get; }
		string Address { get; }
		IPhoneLine[] Phones { get; }

		string DisplayNameOrAor { get; }
	}

	public interface ISelfPresentity
		: IPresentity
	{
		void SetAvailability(AvailabilityValues availability);
	}
}
