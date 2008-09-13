using System;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.xuk;

namespace urakawa.media
{
    /// <summary>
    /// The media factory will create any media object of MediaType.xxx
    /// </summary>
    public class MediaFactory : GenericFactory<Media>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected internal MediaFactory()
        {
        }

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
        public virtual ExternalImageMedia CreateExternalImageMedia()
        {
            return Create<ExternalImageMedia>();
        }

        /// <summary>
        /// Creates a <see cref="ExternalVideoMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public virtual ExternalVideoMedia CreateExternalVideoMedia()
        {
            return Create<ExternalVideoMedia>();
        }

        /// <summary>
        /// Creates a <see cref="SequenceMedia"/>
        /// </summary>
        /// <returns>The creation</returns>
        public virtual SequenceMedia CreateSequenceMedia()
        {
            return Create<SequenceMedia>();
        }

        #endregion
    }
}