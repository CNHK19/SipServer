// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Uccapi
{
	public class PresentitiesCollectionXmlSerializer
		: IXmlSerializable
	{
		private IPresentitiesCollection presentities;

		public PresentitiesCollectionXmlSerializer()
		{
			this.presentities = new PresentitiesCollection(false);
		}

		public PresentitiesCollectionXmlSerializer(IPresentitiesCollection presentities)
		{
			this.presentities = presentities;
		}

		public IPresentitiesCollection Presentities
		{
			get
			{
				return this.presentities;
			}
		}

		public static bool IsPropertySerializable(string propertyName)
		{
			return propertyName == PropertyName.DisplayName || propertyName == PropertyName.Group;
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("presentitiesCollection");

			foreach (IPresentity presentity in this.presentities)
			{
				writer.WriteStartElement("presentity");
				writer.WriteAttributeString("version", "0");
				writer.WriteAttributeString("uri", presentity.Uri);
				writer.WriteAttributeString("name", presentity.DisplayName);
				writer.WriteAttributeString("group", presentity.Group);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			this.presentities.Clear();

			while (reader.Read())
			{
				if (reader.Name == "presentitiesCollection" && reader.NodeType == XmlNodeType.EndElement)
					break;

				if (reader.Name == "presentity")
				{
					Presentity presentity = new Presentity();

					if (reader.GetAttribute("version") == "0")
					{
						presentity.Uri = reader.GetAttribute("uri");
						presentity.DisplayName = reader.GetAttribute("name");
						presentity.Group = reader.GetAttribute("group");
					}
					else
						presentity = null;

					if (presentity != null)
						this.presentities.Add(presentity);
				}
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}
	}
}
