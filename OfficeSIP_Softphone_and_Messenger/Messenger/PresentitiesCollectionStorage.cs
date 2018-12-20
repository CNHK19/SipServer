// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Configuration;
using Uccapi;

namespace Messenger
{
	sealed class PresentitiesCollectionStorage
		: ApplicationSettingsBase
	{
		[UserScopedSettingAttribute()]
		[global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
		public PresentitiesCollectionXmlSerializer PresentitiesSerializer
		{
			get
			{
				return (PresentitiesCollectionXmlSerializer)this["PresentitiesSerializer"];
			}
			set
			{
				this["PresentitiesSerializer"] = value;
			}
		}

		public IPresentitiesCollection Presentities
		{
			get
			{
				var serializer = this.PresentitiesSerializer;
				if (serializer != null)
					return serializer.Presentities;
				return null;
			}
			set
			{
				this.PresentitiesSerializer = new PresentitiesCollectionXmlSerializer(value);
			}
		}
	}
}
