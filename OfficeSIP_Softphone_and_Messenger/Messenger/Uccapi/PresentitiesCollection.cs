// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Uccapi
{
	class PresentitiesCollection
		: ObservableCollection<IPresentity>
		, IPresentitiesCollection
	{
		public event PropertyChangedEventHandler ItemPropertyChanged;
		public event NotifyCollectionChangedEventHandler PostCollectionChanged;

		public PresentitiesCollection(bool enableItemPropertyChangedEvent)
		{
			if (enableItemPropertyChangedEvent)
				base.CollectionChanged += Event_CollectionChanged;
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			if (PostCollectionChanged != null)
				PostCollectionChanged(this, e);
		}

		#region Group List Maintaner

		private ObservableCollection<string> groups = new ObservableCollection<string>();

		public ObservableCollection<string> Groups
		{
			get
			{
				return groups;
			}
		}

		private void UpdateGroups()
		{
			var newGroups = this.Select<IPresentity, string>((presentity) => { return presentity.Group; })
						.Distinct<string>()
						.OrderByDescending<string, string>((group) => { return group; });

			var remove = groups.Except<string>(newGroups).ToArray();
			var add = newGroups.Except<string>(groups).ToArray();

			foreach (var item in remove)
				groups.Remove(item);

			foreach (var item in add)
				groups.Add(item);
		}

		#endregion

		#region Events [ CollectionChanged, PropertyChanged ]

		private void Event_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (IPresentity presentity in e.OldItems)
					presentity.PropertyChanged -= Presentity_PropertyChanged;
			}

			if (e.NewItems != null)
			{
				foreach (IPresentity presentity in e.NewItems)
					presentity.PropertyChanged += Presentity_PropertyChanged;
			}

			if (e.OldItems != null || e.NewItems != null)
				UpdateGroups();
		}

		private void Presentity_PropertyChanged(object item, PropertyChangedEventArgs e)
		{
			if (ItemPropertyChanged != null)
				ItemPropertyChanged(item, e);

			if (e.PropertyName == PropertyName.Group)
				UpdateGroups();
		}

		#endregion

		#region IPresentitiesCollection [ Find, Contain, Add, Clear, AddRange, RemoveRange ]

		public IPresentity Find(IPresentity presentity)
		{
			foreach (var item in Items)
				if (Helpers.IsUriEqual(item.Uri, presentity.Uri))
					return item;
			return null;
		}

		public bool Contain(string uri)
		{
			uri = Helpers.CorrectUri(uri);
			foreach (var item in Items)
				if (Helpers.IsUriEqual(item.Uri, uri))
					return true;
			return false;
		}

		public new void Add(IPresentity presentity)
		{
			if (Find(presentity) == null)
				base.Add(presentity);
		}

		public new void Clear()
		{
			base.Clear();
		}

		public void AddRange(ICollection<IPresentity> collection)
		{
			foreach (IPresentity item in collection)
				this.Add(item);
		}

		public void RemoveRange(ICollection<IPresentity> collection)
		{
			this.RemoveRange(collection as IList);
		}

		public void RemoveRange(IList list)
		{
			foreach (IPresentity item in list)
				this.Remove(item);
		}

		#endregion
	}
}
