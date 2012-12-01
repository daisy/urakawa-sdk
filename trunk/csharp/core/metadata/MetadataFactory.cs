using urakawa.xuk;

namespace urakawa.metadata
{
    [XukNameUglyPrettyAttribute("metadtFct", "MetadataFactory")]
    public sealed class MetadataFactory : GenericWithPresentationFactory<Metadata>
    {
        public MetadataFactory(Presentation pres) : base(pres)
        {
        }

        public Metadata CreateMetadata()
        {
            return Create<Metadata>();
        }
    }
}