// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;

namespace Uccapi
{
	public class SearchUserRecord
		: INotifyPropertyChanged
	{
		public string Uri { get; set; }
		public string DisplayName { get; set; }
		public string Title { get; set; }
		public string Office { get; set; }
		public string Phone { get; set; }
		public string Company { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string Email { get; set; }

		#region public bool InContacts { get; set; }

		private bool inContacts;
		
		public bool InContacts
		{
			get
			{
				return inContacts;
			}
			set
			{
				if (inContacts != value)
				{
					inContacts = value;
					OnPropertyChanged(PropertyName.InContacts);
				}
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(String property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		#endregion INotifyPropertyChanged
	}
}
