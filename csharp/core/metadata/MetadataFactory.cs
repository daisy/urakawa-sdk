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

        /// <summary>
        /// Creates an <see cref="Metadata"/> instance
        /// </summary>
        /// <returns>The created instance</returns>
        public Metadata CreateMetadata()
        {
            return Create<Metadata>();
        }
    }
}