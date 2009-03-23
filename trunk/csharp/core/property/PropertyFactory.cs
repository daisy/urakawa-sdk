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
    }
}