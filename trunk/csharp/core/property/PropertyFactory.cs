using System;
using System.Xml;
using urakawa.core;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.property
{
    /// <summary>
    /// Factory for creating <see cref="Property"/>s and derived types
    /// </summary>
    public sealed class PropertyFactory : GenericFactory<Property>
    {

        /// <summary>
        /// Creates a <see cref="ChannelsProperty"/>
        /// </summary>
        /// <returns>The created <see cref="ChannelsProperty"/></returns>
        public ChannelsProperty CreateChannelsProperty()
        {
            return Create<ChannelsProperty>();
        }

        /// <summary>
        /// Creates an <see cref="urakawa.property.xml.XmlProperty"/> instance
        /// </summary>
        /// <returns>The created instance</returns>
        public urakawa.property.xml.XmlProperty CreateXmlProperty()
        {
            return Create<urakawa.property.xml.XmlProperty>();
        }

        /// <summary>
        /// Creates an <see cref="urakawa.property.xml.XmlAttribute"/> instance 
        /// with a given <see cref="urakawa.property.xml.XmlProperty"/> parent
        /// </summary>
        /// <returns>The created instance</returns>
        /// TODO: Move method to XmlProperty
        public urakawa.property.xml.XmlAttribute CreateXmlAttribute()
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
        /// TODO: Move method to XmlProperty
        public urakawa.property.xml.XmlAttribute CreateXmlAttribute(string localName, string namespaceUri)
        {
            if (namespaceUri == XukAble.XUK_NS)
            {
                switch (localName)
                {
                    case "XmlAttribute":
                        return CreateXmlAttribute();
                }
            }
            return null;
        }
    }
}