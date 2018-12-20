// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;

namespace Uccapi
{
	class SelfPresentity
		: PresentityBase
		, ISelfPresentity
	{
		public void SetUri(string uri)
		{
			base.Uri = uri;
		}

		public void SetAvailability(AvailabilityValues availability)
		{
			base.Availability = availability;
		}
	}
}
