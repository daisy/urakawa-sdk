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
    }
}