using System;
using urakawa.core;
using urakawa.property.channel;
using urakawa.property.xml;

namespace urakawa.property
{
	/// <summary>
	/// Factory for creating <see cref="Property"/>s
	/// </summary>
	public class PropertyFactory : GenericPropertyFactory, IChannelsPropertyFactory, IXmlPropertyFactory
	{


		#region IChannelsPropertyFactory Members

		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/>
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		public ChannelsProperty createChannelsProperty()
		{
			ChannelsProperty newProp = new ChannelsProperty();
			newProp.setPresentation(getPresentation());
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
		/// Creates an <see cref="XmlProperty"/> instance
		/// </summary>
		/// <returns>The created instance</returns>
		public XmlProperty createXmlProperty()
		{
			XmlProperty newProp = new XmlProperty();
			newProp.setPresentation(getPresentation());
			return newProp;
		}

		/// <summary>
		/// Creates an <see cref="XmlAttribute"/> instance with a given <see cref="XmlProperty"/> parent
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <returns>The created instance</returns>
		public XmlAttribute createXmlAttribute(XmlProperty parent)
		{
			return new XmlAttribute(parent);
		}

		/// <summary>
		/// Creates a <see cref="XmlAttribute"/> of type matching a given QName with a given parent <see cref="XmlProperty"/>
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created instance or <c>null</c> if the QName is not recognized</returns>
		public XmlAttribute createXmlAttribute(XmlProperty parent, string localName, string namespaceUri)
		{
			if (namespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "XmlAttribute":
						return createXmlAttribute(parent);
				}
			}
			return null;
		}

		#endregion
	}
}
