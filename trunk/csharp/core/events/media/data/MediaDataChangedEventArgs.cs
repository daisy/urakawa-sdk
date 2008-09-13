using urakawa.media;
using urakawa.media.data;

namespace urakawa.events.media.data
{
    /// <summary>
    /// Arguments of the <see cref="IManaged.MediaDataChanged"/> event
    /// </summary>
    public class MediaDataChangedEventArgs : MediaEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="IManaged"/>, 
        /// the new <see cref="MediaData"/> and the <see cref="MediaData"/> prior to the change
        /// </summary>
        /// <param name="source">The source <see cref="Media"/> - must also be <see cref="IManaged"/></param>
        /// <param name="newMD">The new <see cref="MediaData"/></param>
        /// <param name="prevMD">The <see cref="MediaData"/> prior to the change</param>
        public MediaDataChangedEventArgs(Media source, MediaData newMD, MediaData prevMD)
            : base(source)
        {
            SourceManagedMedia = source as IManaged;
            NewMediaData = newMD;
            PreviousMediaData = prevMD;
        }

        /// <summary>
        /// The source <see cref="IManaged"/>
        /// </summary>
        public readonly IManaged SourceManagedMedia;

        /// <summary>
        /// The new <see cref="MediaData"/>
        /// </summary>
        public readonly MediaData NewMediaData;

        /// <summary>
        /// The <see cref="MediaData"/> prior to the change
        /// </summary>
        public readonly MediaData PreviousMediaData;
    }
}