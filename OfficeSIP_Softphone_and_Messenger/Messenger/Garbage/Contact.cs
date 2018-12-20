// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Uccapi;
/*
namespace Messenger
{
	[System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
	public class Contact 
		: Presentity
		, IXmlSerializable
    {
        private string group;

		public Contact(string uri, string group)
		{
			this.Uri = Uccapi.Helpers.CorrectUri(uri);

			if (Uccapi.Helpers.IsInvalidUri(this.Uri))
				throw new ArgumentException("Invalid URI");

			this.group = group;
		}

		#region IXmlSerializable

		public Contact()
		{
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("contact");
			writer.WriteAttributeString("version", "0");
			writer.WriteAttributeString("uri", this.Uri);
			writer.WriteAttributeString("name", this.DisplayName);
			writer.WriteAttributeString("group", this.group);
			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader.Name == "contact")
			{
				if (reader.GetAttribute("version") == "0")
				{
					this.Uri = reader.GetAttribute("uri");
					this.DisplayName = reader.GetAttribute("name");
					this.group = reader.GetAttribute("group");
				}
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Extension for ContactCollection, for monitoring property contact changes
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public bool IsPropertySerializable(string propertyName)
		{
			return propertyName == "Name" || propertyName == "Group";
		}

		#endregion IXmlSerializable

		#region Properties

		public bool IsOffline()
		{
			return this.Availability == AvailabilityValues.Unknown ||
				this.Availability == AvailabilityValues.Offline;
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
					base.OnPropertyChanged("Group");
                }
            }
		}
		
		public string NotNullGroup
		{
			get
			{
				if (String.IsNullOrEmpty(this.group))
					return @"No group";
				return this.group;
			}
		}

		#endregion Properties
	}
}
*/