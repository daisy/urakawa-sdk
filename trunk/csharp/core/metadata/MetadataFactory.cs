using urakawa.xuk;

namespace urakawa.metadata
{
    /// <summary>
    /// Default <see cref="Metadata"/> factory - supports creation of <see cref="Metadata"/> instances
    /// </summary>
    public sealed class MetadataFactory : GenericWithPresentationFactory<Metadata>
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.MetadataFactory;
        }
        public MetadataFactory(Presentation pres) : base(pres)
        {
        }

        public Metadata CreateMetadata()
        {
            return Create<Metadata>();
        }
    }
}