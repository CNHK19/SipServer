// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
/*
namespace Messenger
{
	public class SerializableObservableContactsCollection :
		ObservableCollection<Contact>, IXmlSerializable
	{
		public event PropertyChangedEventHandler ItemPropertyChanged;

		public SerializableObservableContactsCollection(bool notIXmlSerializableConstructor)
		{
			SerializableObservableContactsCollection storedContacts = 
				Messenger.Properties.ContactList.Default.Value;

			if (storedContacts != null)
			{
				foreach (Contact contact in storedContacts)
				{
					contact.PropertyChanged += Contact_PropertyChanged;
					this.Add(contact);
				}
			}

			this.CollectionChanged += CollectionChangedHandler;
		}

		private void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
		{
			Messenger.Properties.ContactList.Default.Value = this;
			Messenger.Properties.ContactList.Default.Save();

			if (e.OldItems != null)
				foreach (Contact contact in e.OldItems)
					contact.PropertyChanged -= Contact_PropertyChanged;

			if (e.NewItems != null)
				foreach (Contact contact in e.NewItems)
					contact.PropertyChanged += Contact_PropertyChanged;
		}

		private void Contact_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if ((sender as Contact).IsPropertySerializable(e.PropertyName))
			{
				if (Items.Contains(sender as Contact))
				{
					Messenger.Properties.ContactList.Default.Value = this;
					Messenger.Properties.ContactList.Default.Save();
				}
			}

			if (ItemPropertyChanged != null)
				ItemPropertyChanged(sender, e);
		}

		#region IXmlSerializable

		public SerializableObservableContactsCollection()
		{
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("contactList");

			foreach (Contact contact in this.Items)
				contact.WriteXml(writer);

			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.Name == "contactList" && reader.NodeType == XmlNodeType.EndElement)
					break;

				if (reader.Name == "contact")
				{
					Contact contact = new Contact();
					contact.ReadXml(reader);

					this.Items.Add(contact);
				}
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		#endregion IXmlSerializable
	}
}
*/