// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;
using System.ComponentModel;


namespace Uccapi
{
	class PresentityBase
		: UccapiBase
		, IPresentity
	{
		private string uri;
		private string displayName;
		private AvailabilityValues availability = AvailabilityValues.Unknown;
		private string group;
		private string homepage;
		private string fax;
		private string address;
		private string[] emails;
		private IPhoneLine[] phones;

		public PresentityBase()
		{
		}

		#region IPresentity [ Uri, DisplayName, DisplayNameOrAor, Availability, Group... ]

		public string Uri
		{
			get
			{
				return this.uri;
			}
			protected set
			{
				this.uri = Helpers.CorrectUri(value);

				if (Helpers.IsInvalidUri(this.Uri))
					throw new ArgumentException("Invalid URI");

				this.OnPropertyChanged(PropertyName.Uri);
				this.OnPropertyChanged(PropertyName.Aor);
				this.OnPropertyChanged(PropertyName.DisplayNameOrAor);
			}
		}

		public string Aor
		{
			get
			{
				return Helpers.GetAor(this.uri);
			}
		}

		public virtual string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				if (value != this.displayName)
				{
					this.displayName = value;
					this.OnPropertyChanged(PropertyName.DisplayName);
					this.OnPropertyChanged(PropertyName.DisplayNameOrAor);
				}
			}
		}

		public string DisplayNameOrAor
		{
			get
			{
				if (string.IsNullOrEmpty(this.displayName))
					return Helpers.GetAor(this.uri);
				return this.displayName;
			}
		}

		public virtual AvailabilityValues Availability
		{
			get
			{
				return this.availability;
			}
			protected set
			{
				if (value != this.availability)
				{
					this.availability = value;
					this.OnPropertyChanged(PropertyName.Availability);
				}
			}
		}

		public string Group
		{
			get
			{
				return this.group;
			}
			set
			{
				if (value != this.group)
				{
					this.group = value;
					this.OnPropertyChanged(PropertyName.Group);
				}
			}
		}

		public string Homepage 
		{
			get
			{
				return this.homepage;
			}
			protected set
			{
				if (value != this.homepage)
				{
					this.homepage = value;
					this.OnPropertyChanged(PropertyName.Homepage);
				}
			}
		}
		
		public string Fax 
		{
			get
			{
				return this.fax;
			}
			protected set
			{
				if (value != this.fax)
				{
					this.fax = value;
					this.OnPropertyChanged(PropertyName.Fax);
				}
			}
		}

		public string Address 
		{
			get
			{
				return this.address;
			}
			protected set
			{
				if (value != this.address)
				{
					this.address = value;
					this.OnPropertyChanged(PropertyName.Address);
				}
			}
		}

		public string[] Emails
		{
			get
			{
				return this.emails;
			}
			protected set
			{
				this.emails = value;
				this.OnPropertyChanged(PropertyName.Emails);
			}
		}

		public IPhoneLine[] Phones
		{
			get
			{
				return this.phones;
			}
			protected set
			{
				this.phones = value;
				this.OnPropertyChanged(PropertyName.Phones);
			}
		}

		#endregion

		#region class PhoneLine

		protected class PhoneLine
			: IPhoneLine
		{
			public string Uri { get; set; }
			public PhoneLineType LineType { get; set; }
			public string LineServer { get; set; }

			public UCC_PRESENCE_PHONE_LINE_TYPE UccLineType
			{
				set
				{
					switch (value)
					{
						case UCC_PRESENCE_PHONE_LINE_TYPE.UCCPPLT_DUAL:
							LineType = PhoneLineType.Dual;
							break;
						case UCC_PRESENCE_PHONE_LINE_TYPE.UCCPPLT_NONE:
							LineType = PhoneLineType.None;
							break;
						case UCC_PRESENCE_PHONE_LINE_TYPE.UCCPPLT_REMOTE_CALL_CONTROL:
							LineType = PhoneLineType.RemoteCallControl;
							break;
						case UCC_PRESENCE_PHONE_LINE_TYPE.UCCPPLT_UNIFIED_COMMUNCATIONS:
							LineType = PhoneLineType.UnifiedCommunications;
							break;
					}
				}
			}
		}

		#endregion
	}

	class FakePresentity
		: PresentityBase
	{
		public FakePresentity(string uri)
		{
			base.Uri = uri;
		}
	}

	class Presentity
		: PresentityBase
		, _IUccPresentityEvents
		, _IUccCategoryContextEvents
		, _IUccCategoryInstanceEvents
	{
		public new string Uri
		{
			get
			{
				return base.Uri;
			}
			set
			{
				if (String.IsNullOrEmpty(base.Uri) == false)
					throw new InvalidOperationException("Uri already has no null value. You can not change Uri.");
				base.Uri = value;
			}
		}

		public void SetUri(UccUri uri)
		{
			base.Uri = uri.Value.ToString();
		}

		#region UccPresentity

		public UccPresentity UccPresentity { get; private set; }

		public virtual UccPresentity DestroyUccPresentity()
		{
			this.DettachPresentity();

			this.Availability = AvailabilityValues.Unknown;

			UccPresentity oldPresentity = this.UccPresentity;

			if (this.UccPresentity != null)
			{
				Uccapi.ComEvents.UnadviseAll(this);
				this.UccPresentity = null;
			}

			return oldPresentity;
		}

		public bool CreateUccPresentity(IUccSubscription subscription)
		{
			UccUri uccUri;
			if (Helpers.TryParseUri(this.Uri, out uccUri))
			{
				this.UccPresentity = subscription.CreatePresentity(uccUri, null);
				Uccapi.ComEvents.Advise<_IUccPresentityEvents>(this.UccPresentity, this);
			}

			return this.UccPresentity != null;
		}

		#endregion

		#region Process Category Instances

		protected virtual void ProcessCategoryInstance(IUccCategoryInstance categoryInstance)
		{
			switch (categoryInstance.CategoryContext.Name.Trim())
			{
				case CategoryName.State:
					ProcessStateInstance(categoryInstance as IUccPresenceStateInstance);
					break;
				case CategoryName.ContactCard:
					ProcessContactCardInstance(categoryInstance as IUccPresenceContactCardInstance);
					break;
				case CategoryName.UserProperties:
					ProcessUserPropertiesInstance(categoryInstance as IUccProvisioningPolicyInstance);
					break;
				default:
					break;
			}
		}

		protected virtual void ProcessStateInstance(IUccPresenceStateInstance state)
		{
			if (state != null)
			{
				this.Availability = AvailabilityHelper.ConvertFromInt(state.Availability);
			}
		}

		protected virtual void ProcessContactCardInstance(IUccPresenceContactCardInstance contactCard)
		{
			if (contactCard != null)
			{
				try
				{
					this.DisplayName = contactCard.Identity.DisplayName;
				}
				catch
				{
					this.DisplayName = null;
				}

				try
				{
					if (contactCard.Identity.EmailAddresses != null && contactCard.Identity.EmailAddresses.Count > 0)
					{
						string[] emails = new string[contactCard.Identity.EmailAddresses.Count];
						for (int i = 0; i < emails.Length; i++)
							emails[i] = contactCard.Identity.EmailAddresses[i + 1] as string;

						this.Emails = emails;
					}
					else
					{
						this.Emails = null;
					}
				}
				catch
				{
					this.Emails = null;
				}
			}
		}

		protected virtual void ProcessUserPropertiesInstance(IUccProvisioningPolicyInstance userProperties)
		{
			IUccProperty property = userProperties.Properties.get_NamedProperty(@"lines");
			if (property != null)
			{
				IUccCollection phoneLines = property.Value as IUccCollection;

				if (phoneLines.Count > 0)
				{
					PhoneLine[] phones = new PhoneLine[phoneLines.Count];

					for (int i = 0; i < phones.Length; i++)
					{
						IUccPresencePhoneLine line = phoneLines[i + 1] as IUccPresencePhoneLine;

						phones[i] = new PhoneLine();

						try { phones[i].LineServer = line.LineServer; }
						catch(COMException) { }

						try { phones[i].Uri = line.Uri; }
						catch (COMException) { }

						try { phones[i].UccLineType = line.LineType; }
						catch (COMException) { }
					}

					this.Phones = phones;
				}
			}

			this.Fax = GetNamedProperty(userProperties, @"facsimileTelephoneNumber");
			this.Homepage = GetNamedProperty(userProperties, @"wWWHomePage");

			string streetAddr = GetNamedProperty(userProperties, @"streetAddress");
			string city = GetNamedProperty(userProperties, @"l");
			string state = GetNamedProperty(userProperties, @"st");
			string zip = GetNamedProperty(userProperties, @"postalCode");
			string countryCode = GetNamedProperty(userProperties, @"countryCode");

			string addr = @"";
			if (string.IsNullOrEmpty(streetAddr) == false)
				addr += streetAddr + "\r\n";
			if (string.IsNullOrEmpty(state) == false)
				addr += state + "\r\n";
			if (string.IsNullOrEmpty(city) == false)
				addr += city + "\r\n";
			addr += zip + @" " + countryCode;

			this.Address = addr;
		}

		private string GetNamedProperty(IUccProvisioningPolicyInstance properties, string name)
		{
			if (properties.Properties.IsNamedPropertySet(name))
			{
				IUccProperty property = properties.Properties.get_NamedProperty(name);
				if (property != null)
					return property.StringValue;
			}
			return null;
		}

		#endregion

		#region AttachTo Presentity

		private IPresentity attachedPresentity;

		public void AttachPresentity(IPresentity presentity)
		{
			this.attachedPresentity = presentity;

			this.attachedPresentity.PropertyChanged += AttachedPresentity_PropertyChanged;

			this.SyncPresentity();
		}

		public void DettachPresentity()
		{
			if (this.attachedPresentity != null)
			{
				this.attachedPresentity.PropertyChanged -= AttachedPresentity_PropertyChanged;
				this.attachedPresentity = null;
			}
		}

		private void SyncPresentity()
		{
#if DEBUG
			// 2 non-sync properties: Uri, Aor, DisplayNameOrAor
			System.Diagnostics.Debug.Assert(typeof(IPresentity).GetProperties().Length - 3 == 8, "class Presentity: Update SyncPresentity and AttachedPresentity_PropertyChanged");
#endif

			this.Availability = this.attachedPresentity.Availability;

			if (string.IsNullOrEmpty(this.attachedPresentity.DisplayName) == false)
				this.DisplayName = this.attachedPresentity.DisplayName;

			if (string.IsNullOrEmpty(this.attachedPresentity.Group) == false)
				this.Group = this.attachedPresentity.Group;

			this.Emails = this.attachedPresentity.Emails;
			this.Address = this.attachedPresentity.Address;
			this.Phones = this.attachedPresentity.Phones;
			this.Fax = this.attachedPresentity.Fax;
			this.Homepage = this.attachedPresentity.Homepage;
		}

		private void AttachedPresentity_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case PropertyName.Availability:
					this.Availability = (sender as IPresentity).Availability;
					break;
				case PropertyName.DisplayName:
					this.DisplayName = (sender as IPresentity).DisplayName;
					break;
				case PropertyName.Group:
					this.Group = (sender as IPresentity).Group;
					break;
				case PropertyName.Emails:
					this.Emails = (sender as IPresentity).Emails;
					break;
				case PropertyName.Address:
					this.Address = (sender as IPresentity).Address;
					break;
				case PropertyName.Phones:
					this.Phones = (sender as IPresentity).Phones;
					break;
				case PropertyName.Fax:
					this.Fax = (sender as IPresentity).Fax;
					break;
				case PropertyName.Homepage:
					this.Homepage = (sender as IPresentity).Homepage;
					break;
			}
		}

		#endregion

		#region _IUccPresentityEvents

		void _IUccPresentityEvents.OnCategoryContextAdded(
			UccPresentity presentity,
			UccCategoryContextEvent categoryContext)
		{
			switch (categoryContext.CategoryContext.Name.Trim())
			{
				case CategoryName.State:
				case CategoryName.ContactCard:
				case CategoryName.UserProperties:
					ComEvents.Advise<_IUccCategoryContextEvents>(categoryContext.CategoryContext, this);
					break;
			}
		}

		void _IUccPresentityEvents.OnCategoryContextRemoved(
			UccPresentity presentity,
			UccCategoryContextEvent categoryContext)
		{
			ComEvents.Unadvise<_IUccCategoryContextEvents>(categoryContext.CategoryContext, this);
		}

		#endregion _IUccPresentityEvents

		#region _IUccCategoryContextEvents

		void _IUccCategoryContextEvents.OnCategoryInstanceAdded(
			IUccCategoryContext categoryContext,
			UccCategoryInstanceEvent categoryEvent)
		{
			IUccCategoryInstance categoryInstance = categoryEvent.CategoryInstance;

			ComEvents.Advise<_IUccCategoryInstanceEvents>(categoryInstance, this);

			ProcessCategoryInstance(categoryInstance);
		}


		void _IUccCategoryContextEvents.OnCategoryInstanceRemoved(
			IUccCategoryContext categoryContext,
			UccCategoryInstanceEvent categoryEvent)
		{
			ComEvents.Unadvise<_IUccCategoryInstanceEvents>(categoryEvent.CategoryInstance, this);
		}

		#endregion _IUccCategoryInstanceEvents

		#region _IUccCategoryInstanceEvents

		void _IUccCategoryInstanceEvents.OnCategoryInstanceValueModified(
			IUccCategoryInstance categoryInstance,
			object eventData)
		{
			ProcessCategoryInstance(categoryInstance);
		}

		#endregion _IUccCategoryInstanceEvents
	}
}
