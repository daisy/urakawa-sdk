using urakawa.xuk;

namespace urakawa.metadata
{
    [XukNameUglyPrettyAttribute("metadtFct", "MetadataFactory")]
    public sealed class MetadataFactory : GenericWithPresentationFactory<Metadata>
    {
        public MetadataFactory(Presentation pres) : base(pres)
        {
        }

        //public MetadataAttribute CreateMetadataAttribute()
        //{
        //    return Create<MetadataAttribute>();
        //}

        public Metadata CreateMetadata()
        {
            return Create<Metadata>();
        }
    }
}