using System;
using System.Xml;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.property
{
    /// <summary>
    /// Factory for creating <see cref="Property"/>s and derived types
    /// </summary>
    public sealed class PropertyFactory : GenericWithPresentationFactory<Property>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.PropertyFactory;
        }
        public PropertyFactory(Presentation pres) : base(pres)
        {
        }

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
        public xml.XmlProperty CreateXmlProperty()
        {
            return Create<xml.XmlProperty>();
        }

        private string m_DefaultXmlNamespaceUri = null;

        ///<summary>
        /// Default NS Uri for XmlProperties (valid at the entire Presentation level,
        /// avoids duplicating the NSURI attribute in the XUK file)
        ///</summary>
        public string DefaultXmlNamespaceUri
        {
            get { return m_DefaultXmlNamespaceUri; }
            set { m_DefaultXmlNamespaceUri = value; }
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);
            if (!String.IsNullOrEmpty(DefaultXmlNamespaceUri))
            {
                destination.WriteAttributeString(XukStrings.DefaultXmlNamespaceUri, DefaultXmlNamespaceUri);
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            string defaultNS = source.GetAttribute(XukStrings.DefaultXmlNamespaceUri);
            if (!String.IsNullOrEmpty(defaultNS))
            {
                DefaultXmlNamespaceUri = defaultNS;
            }
            base.XukInAttributes(source);
        }
    }
}