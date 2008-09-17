using urakawa.media.data.audio;

namespace urakawa.media
{
    /// <summary>
    /// The media factory will create any media object of MediaType.xxx
    /// </summary>
    public sealed class MediaFactory : GenericWithPresentationFactory<Media>
    {
        #region IMediaFactory Members

        /// <summary>
        /// Creates a <see cref="data.audio.ManagedAudioMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public ManagedAudioMedia CreateManagedAudioMedia()
        {
            return Create<ManagedAudioMedia>();
        }

        /// <summary>
        /// Creates a <see cref="TextMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public TextMedia CreateTextMedia()
        {
            return Create<TextMedia>();
        }

        /// <summary>
        /// Creates a <see cref="ExternalImageMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public ExternalImageMedia CreateExternalImageMedia()
        {
            return Create<ExternalImageMedia>();
        }

        /// <summary>
        /// Creates a <see cref="ExternalVideoMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public ExternalVideoMedia CreateExternalVideoMedia()
        {
            return Create<ExternalVideoMedia>();
        }

        /// <summary>
        /// Creates a <see cref="SequenceMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public SequenceMedia CreateSequenceMedia()
        {
            return Create<SequenceMedia>();
        }

        #endregion
    }
}