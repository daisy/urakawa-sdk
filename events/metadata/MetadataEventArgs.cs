using urakawa.metadata;

namespace urakawa.events.metadata
{
    /// <summary>
    /// Base class for arguments of <see cref="Metadata"/> sourced events
    /// </summary>
    public class MetadataEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Metadata"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="Metadata"/> of the event</param>
        public MetadataEventArgs(Metadata source)
            : base(source)
        {
            SourceMetadata = source;
        }

        /// <summary>
        /// The source <see cref="Metadata"/> of the event
        /// </summary>
        public readonly Metadata SourceMetadata;
    }
}