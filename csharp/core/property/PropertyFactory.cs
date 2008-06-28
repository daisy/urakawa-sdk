using System;
using System.Xml;
using urakawa.core;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.property
{
	/// <summary>
	/// Factory for creating <see cref="Property"/>s
	/// </summary>
	public class PropertyFactory 
		: GenericPropertyFactory, IChannelsPropertyFactory, urakawa.property.xml.IXmlPropertyFactory, IXukAble
	{
		/// <summary>
		/// Defautl constructor
		/// </summary>
		protected internal PropertyFactory()
		{
		}

		#region IChannelsPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		public ChannelsProperty createChannelsProperty()
		{
			ChannelsProperty newProp = new ChannelsProperty();
			newProp.Presentation = Presentation;
			return newProp;
		}

		#endregion

		#region IGenericPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="Property"/> of type matching a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Property"/> or <c>null</c> is the given QName is not recognized</returns>
		public override Property createProperty(string localName, string namespaceUri)
		{
			if (namespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "XmlProperty":
						return createXmlProperty();
					case "ChannelsProperty":
						return createChannelsProperty();
				}
			}
			return base.createProperty(localName, namespaceUri);;
		}

		#endregion

		#region IXmlPropertyFactory Members

		/// <summary>
		/// Creates an <see cref="urakawa.property.xml.XmlProperty"/> instance
		/// </summary>
		/// <returns>The created instance</returns>
		public urakawa.property.xml.XmlProperty createXmlProperty()
		{
			urakawa.property.xml.XmlProperty newProp = new urakawa.property.xml.XmlProperty();
			newProp.Presentation = Presentation;
			return newProp;
		}

		/// <summary>
		/// Creates an <see cref="urakawa.property.xml.XmlAttribute"/> instance 
		/// with a given <see cref="urakawa.property.xml.XmlProperty"/> parent
		/// </summary>
		/// <returns>The created instance</returns>
		public urakawa.property.xml.XmlAttribute createXmlAttribute()
		{
			urakawa.property.xml.XmlAttribute newAttr = new urakawa.property.xml.XmlAttribute();
			newAttr.Presentation = Presentation;
			return newAttr;
		}

		/// <summary>
		/// Creates a <see cref="urakawa.property.xml.XmlAttribute"/> of type 
		/// matching a given QName with a given parent <see cref="urakawa.property.xml.XmlProperty"/>
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created instance or <c>null</c> if the QName is not recognized</returns>
		public urakawa.property.xml.XmlAttribute createXmlAttribute(string localName, string namespaceUri)
		{
			if (namespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "XmlAttribute":
						return createXmlAttribute();
				}
			}
			return null;
		}

		#endregion

		

	}
}
