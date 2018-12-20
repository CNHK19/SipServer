// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Interop;
using Windows.Helpers;
using Messenger.Properties;
using Uccapi;

namespace Messenger.Windows
{
    /// <summary>
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class Contacts 
		: WindowEx
    {
		private ObservableCollection<GroupsMenuItem> groupsMenuItems;
		private ObservableCollection<IPresentity> presentitiesProxy;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Drawing.Icon trayRedIcon;
		private System.Drawing.Icon trayGreenIcon;
		private System.Drawing.Icon trayYellowIcon;
		private System.Drawing.Icon trayGreyIcon;
		private bool forceClose = false;

        public Contacts()
        {
            InitializeComponent();
            InitializeSyscommands();

			groupsMenuItems = new ObservableCollection<GroupsMenuItem>();
			ExpandedGroups = new StringCollection();
			if (Messenger.Properties.Settings.Default.ExpandedGroups != null)
				foreach (var item in Messenger.Properties.Settings.Default.ExpandedGroups)
					ExpandedGroups.Add(item);

			presentitiesProxy = new ObservableCollection<IPresentity>();
			presentitiesProxy.CollectionChanged += PresentitiesProxy_CollectionChanged;

			trayRedIcon = this.LoadIcon(@"Red.ico");
			trayGreenIcon = this.LoadIcon(@"Green.ico");
			trayYellowIcon = this.LoadIcon(@"Yellow.ico");
			trayGreyIcon = this.LoadIcon(@"Grey.ico");

			this.Minimizing += Window_Minimizing;
            this.Maximizing += Window_Maximizing;

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = this.Title;
			notifyIcon.Icon = trayGreyIcon;
            notifyIcon.Visible = true;
            notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDown);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseClick);
		}

		private System.Drawing.Icon LoadIcon(string fileName)
		{
			return 
				new System.Drawing.Icon(
					GetType().Assembly.GetManifestResourceStream(@"Messenger.Icons." + fileName));
		}

		#region GetSelectedContacts, GetSelectedContact

		public IPresentity[] GetSelectedContacts()
		{
			lock (this.ContactList.SelectedItems.SyncRoot)
			{
				IPresentity[] presentity = new IPresentity[this.ContactList.SelectedItems.Count];
				this.ContactList.SelectedItems.CopyTo(presentity, 0);
				return presentity;
			}
		}

		public IPresentity GetSelectedContact()
		{
			return this.ContactList.SelectedItem as IPresentity;
		}

		#endregion

		#region NotifyIcon - Clicks, Maximizing, Minimizing, OnClosing

		private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Show();
                this.Activate();
            }
        }

		private void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.Clicks == 1)
            {
                this.Activate(); // enable menu items
                
                ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
                menu.CommandBindings.AddRange(this.CommandBindings);
                menu.IsOpen = true;
            }
		}

		private void Window_Maximizing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
		}

		private void Window_Minimizing(object sender, CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (forceClose == false && Properties.Settings.Default.MinimizeOnClose)
				Window_Minimizing(this, e);
			else
				base.OnClosing(e);
		}

		#endregion

		public new void Close()
		{
			forceClose = true;
			base.Close();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Settings.Default.ExpandedGroups = ExpandedGroups;
			Settings.Default.ShowOffline = IsShowOffline;
			Settings.Default.ShowGroups = IsShowGroups;
			Settings.Default.Save();

			Programme.Instance.Endpoint.SelfPresentity.PropertyChanged -= SelfPresentity_PropertyChanged;
			notifyIcon.Dispose();
		}

		public Endpoint Endpoint
		{
			get
			{
				return Programme.Instance.Endpoint;
			}
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CommandBindings.AddRange(Programme.Instance.CommandBindings);
			this.CommandBindings.Add(new CommandBinding(MessengerCommands.ToggleShowOfflineContacts,
				new ExecutedRoutedEventHandler(ShowOfflineBinding_Executed)));
			this.CommandBindings.Add(new CommandBinding(MessengerCommands.ToggleShowGroups,
				new ExecutedRoutedEventHandler(ShowGroupsBinding_Executed)));

			// fix: binding not working every time
			IsShowOffline = Settings.Default.ShowOffline;
			IsShowGroups = Settings.Default.ShowGroups;

            try
            {
                VistaGlass.ExtendGlass(this, -1, -1, -1, -1);
            }
            catch //(DllNotFoundException)
            {
				this.Background = SystemColors.MenuBarBrush;
			}

			this.DataContext = this;

			//Programme.Instance.Contacts.CollectionChanged += Contacts_CollectionChanged;
			//Programme.Instance.Contacts.ItemPropertyChanged += Contacts_ItemPropertyChanged;
			//ContactList.DataContextChanged += ContactList_DataContextChanged;
			//contactsView = new ListCollectionView(Programme.Instance.Contacts);
			//ContactList.ItemsSource = contactsView;
			//this.UpdateSorting();

			state.DataContext = Programme.Instance.Endpoint.SelfPresentity.Availability;

			Programme.Instance.Endpoint.SelfPresentity.PropertyChanged += SelfPresentity_PropertyChanged;

			Programme.Instance.Endpoint.Presentities.Groups.CollectionChanged += Groups_CollectionChanged;
			foreach (string groupName in Programme.Instance.Endpoint.Presentities.Groups)
				AddGroup(groupName);
		}

		public IPresentitiesCollection Presentities
		{
			set
			{
				presentitiesProxy.Clear();
				foreach (var item in value)
					presentitiesProxy.Add(item);

				value.CollectionChanged += Presentities_CollectionChanged;
				value.ItemPropertyChanged += Presentities_ItemPropertyChanged;
				ContactList.ItemsSource = new ListCollectionView(presentitiesProxy);
				this.UpdateSorting();
			}
		}

		private void PresentitiesProxy_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (PresentitiesView != null)
				using (PresentitiesView.DeferRefresh()) { };
		}

		private void Presentities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (IPresentity item in e.NewItems)
					presentitiesProxy.Add(item);

			if (e.OldItems != null)
				foreach (IPresentity item in e.OldItems)
					presentitiesProxy.Remove(item);
		}

		protected void SoftRefreshItem(object item)
		{
			(ContactList.Items as IEditableCollectionView).EditItem(item);
			(ContactList.Items as IEditableCollectionView).CommitEdit();
		}

		protected void HardRefreshItem(object item)
		{
			presentitiesProxy.Remove(item as IPresentity);
			presentitiesProxy.Add(item as IPresentity);
		}

		private void Presentities_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == PropertyName.DisplayNameOrAor)
				HardRefreshItem(sender);

			if (e.PropertyName == PropertyName.Availability)
				HardRefreshItem(sender);
		}

		#region Status of this user

		private void SelfPresentity_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == PropertyName.Availability)
			{
				this.state.DataContext = (sender as SelfPresentity).Availability;

				switch ((sender as SelfPresentity).Availability)
				{
					case AvailabilityValues.Online:
						notifyIcon.Icon = trayGreenIcon;
						break;
					case AvailabilityValues.Away:
					case AvailabilityValues.BeRightBack:
					case AvailabilityValues.Idle:
						notifyIcon.Icon = trayYellowIcon;
						break;
					case AvailabilityValues.Busy:
					case AvailabilityValues.BusyIdle:
					case AvailabilityValues.DoNotDisturb:
						notifyIcon.Icon = trayRedIcon;
						break;
					case AvailabilityValues.Unknown:
					case AvailabilityValues.Offline:
					default:
						notifyIcon.Icon = trayGreyIcon;
						break;
				}
			}
		}

		private void StateMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AvailabilityValues availability = (AvailabilityValues)(sender as MenuItem).DataContext;

			if (Programme.Instance.Endpoint.IsEnabled)
			{
				Programme.Instance.Endpoint.SelfPresentity.SetAvailability(availability);
			}
			else if (Programme.Instance.Endpoint.IsDisabled)
			{
				Commands.Login.Execute(availability, this);
			}
		}

		#endregion

		#region IsShowOffline, IsShowGroups

		public bool IsShowOffline
		{
			get
			{
				return PresentitiesView.Filter == null;
			}
			private set
			{
				if (value)
					PresentitiesView.Filter = null;
				else
				{
					if (PresentitiesView.Filter == null)
						PresentitiesView.Filter = new Predicate<object>(FilterOfflineContacts);
				}

				// fix: binding not working every time
				menuShowOffline.IsChecked = IsShowOffline;
			}
		}

		private bool FilterOfflineContacts(object c)
		{
			IPresentity presentity = c as IPresentity;
			return (presentity.Availability != AvailabilityValues.Offline)
				&& (presentity.Availability != AvailabilityValues.Unknown);
		}

		public bool IsShowGroups
		{
			get
			{
				return PresentitiesView.GroupDescriptions.Count > 0;
			}
			private set
			{
				if (this.IsShowGroups != value)
				{
					if (value)
					{
						if (PresentitiesView.GroupDescriptions.Count <= 0)
							PresentitiesView.GroupDescriptions.Add(new PropertyGroupDescription(PropertyName.Group));
					}
					else
						PresentitiesView.GroupDescriptions.Clear();

					this.UpdateSorting();
				}

				// fix: binding not working every time
				menuShowGroups.IsChecked = IsShowGroups;
			}
		}
		
		private void UpdateSorting()
		{
			using (PresentitiesView.DeferRefresh())
			{
				PresentitiesView.SortDescriptions.Clear();
				if (this.IsShowGroups)
					PresentitiesView.SortDescriptions.Add(new SortDescription(PropertyName.Group, ListSortDirection.Ascending));
				PresentitiesView.SortDescriptions.Add(new SortDescription(PropertyName.DisplayNameOrAor, ListSortDirection.Ascending));
			}
		}

		private void ShowOfflineBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.IsShowOffline = !this.IsShowOffline;
		}

		private void ShowGroupsBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.IsShowGroups = !this.IsShowGroups;
		}

		private ListCollectionView PresentitiesView
		{
			get
			{
				return ContactList.ItemsSource as ListCollectionView;
			}
		}

		public StringCollection ExpandedGroups
		{
			get;
			private set;
		}

		private void GroupExpander_Expanded(object sender, RoutedEventArgs e)
		{
			string groupName = (sender as Control).Tag as string;

			if (ExpandedGroups.Contains(groupName) == false)
				ExpandedGroups.Add(groupName);
		}

		private void GroupExpander_Collapsed(object sender, RoutedEventArgs e)
		{
			string groupName = (sender as Control).Tag as string;

			ExpandedGroups.Remove(groupName);
		}

		#endregion

		private void ListContacts_Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (MessengerCommands.SendInstantMessage.CanExecute(null, (sender as Control)))
				MessengerCommands.SendInstantMessage.Execute(null, (sender as Control));
			e.Handled = true;
		}

		private void ContactList_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Helpers.ListGridView_UpdateColumnWidth(sender as ListView, e, 1);
		}

		#region Groups Menu

		public class GroupsMenuItem
			: INotifyPropertyChanged
		{
			bool isChecked;

			public bool IsChecked 
			{
				get { return isChecked; }
				set
				{
					if (isChecked != value)
					{
						isChecked = value;
						OnPropertyChanged("IsChecked");
					}
				}
			}
			public string Group { get; set; }

			#region INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnPropertyChanged(String property)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(property));
			}

			#endregion INotifyPropertyChanged
		}

		public ObservableCollection<GroupsMenuItem> GroupsMenuItems
		{
			get { return groupsMenuItems; }
		}

		private void SetGroupToSelectedItems(string groupName)
		{
			object[] items = new object[ContactList.SelectedItems.Count];
			ContactList.SelectedItems.CopyTo(items, 0);

			foreach (var item in items)
			{
				(item as IPresentity).Group = groupName;
				SoftRefreshItem(item);
			}

			UpdateGroupCheckbox();
		}

		private void SelectGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetGroupToSelectedItems((sender as MenuItem).Tag as string);
		}

		private void NewGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var window = new NewGroup()
			{
				Owner = this,
			};

			if (window.ShowDialog() == true)
				SetGroupToSelectedItems(window.Group);
		}

		private void AddGroup(string groupName)
		{
			int i = 0;
			for (; i < groupsMenuItems.Count; i++)
				if (string.Compare(groupName, groupsMenuItems[i].Group, true) < 0)
					break;

			groupsMenuItems.Insert(i, new GroupsMenuItem() { Group = groupName, });
		}

		private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				for (int i = groupsMenuItems.Count - 1; i >= 0; i--)
					foreach (string groupName in e.OldItems)
						if (groupsMenuItems[i].Group == groupName)
						{
							groupsMenuItems.RemoveAt(i);
							break;
						}
			}

			if (e.NewItems != null)
			{
				foreach (string groupName in e.NewItems)
					AddGroup(groupName);
			}
		}

		private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateGroupCheckbox();
		}

		private void UpdateGroupCheckbox()
		{
			foreach (var group in groupsMenuItems)
				group.IsChecked = false;

			foreach (IPresentity item in ContactList.SelectedItems)
			{
				string groupName = item.Group;

				foreach (var group in groupsMenuItems)
					if(groupName == group.Group)
						group.IsChecked = true;
			}
		}

		#endregion
	}
}
