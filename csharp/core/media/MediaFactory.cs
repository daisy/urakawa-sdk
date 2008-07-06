using System;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media
{
    /// <summary>
    /// The media factory will create any media object of MediaType.xxx
    /// </summary>
    public class MediaFactory : WithPresentation, IMediaFactory
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected internal MediaFactory()
        {
        }

        #region IMediaFactory Members

        /// <summary>
        /// Creates an <see cref="IMedia"/> matching a given QName
        /// </summary>
        /// <param name="localName">The local part of the QName</param>
        /// <param name="namespaceUri">The namespace uri part of the QName</param>
        /// <returns>The creates <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
        public IMedia CreateMedia(string localName, string namespaceUri)
        {
            IMedia res = null;
            if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
            {
                switch (localName)
                {
                    case "ManagedAudioMedia":
                        res = new data.audio.ManagedAudioMedia();
                        break;
                    case "ExternalAudioMedia":
                        res = new ExternalAudioMedia();
                        break;
                    case "ExternalImageMedia":
                        res = new ExternalImageMedia();
                        break;
                    case "ExternalVideoMedia":
                        res = new ExternalVideoMedia();
                        break;
                    case "TextMedia":
                        res = new TextMedia();
                        break;
                    case "SequenceMedia":
                        res = new SequenceMedia();
                        break;
                    case "ExternalTextMedia":
                        res = new ExternalTextMedia();
                        break;
                }
            }
            if (res != null) res.Presentation = Presentation;
            return res;
        }

        /// <summary>
        /// Creates a <see cref="data.audio.ManagedAudioMedia"/> which is the default <see cref="IAudioMedia"/> of the factory
        /// </summary>
        /// <returns>The creation</returns>
        public virtual IAudioMedia CreateAudioMedia()
        {
            IMedia newMedia = CreateMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS);
            if (newMedia is IAudioMedia) return (IAudioMedia) newMedia;
            throw new exception.FactoryCannotCreateTypeException(
                "The factory unexpectedly could not create a ManagedAudioMedia");
        }

        /// <summary>
        /// Creates a <see cref="TextMedia"/> which is the default <see cref="ITextMedia"/> of the factory
        /// </summary>
        /// <returns>The creation</returns>
        public virtual ITextMedia CreateTextMedia()
        {
            IMedia newMedia = CreateMedia("TextMedia", ToolkitSettings.XUK_NS);
            if (newMedia is ITextMedia) return (ITextMedia) newMedia;
            throw new exception.FactoryCannotCreateTypeException(
                "The factory unexpectedly could not create a TextMedia");
        }

        /// <summary>
        /// Creates a <see cref="ExternalImageMedia"/> which is the default <see cref="IImageMedia"/> of the factory
        /// </summary>
        /// <returns>The creation</returns>
        public virtual IImageMedia CreateImageMedia()
        {
            IMedia newMedia = CreateMedia("ExternalImageMedia", ToolkitSettings.XUK_NS);
            if (newMedia is IImageMedia) return (IImageMedia) newMedia;
            throw new exception.FactoryCannotCreateTypeException(
                "The factory unexpectedly could not create an ExternalImageMedia");
        }

        /// <summary>
        /// Creates a <see cref="ExternalVideoMedia"/> which is the default <see cref="IVideoMedia"/> of the factory
        /// </summary>
        /// <returns>The creation</returns>
        public virtual IVideoMedia CreateVideoMedia()
        {
            IMedia newMedia = CreateMedia("ExternalVideoMedia", ToolkitSettings.XUK_NS);
            if (newMedia is IVideoMedia) return (IVideoMedia) newMedia;
            throw new exception.FactoryCannotCreateTypeException(
                "The factory unexpectedly could not create an ExternalVideoMedia");
        }

        /// <summary>
        /// Creates a <see cref="SequenceMedia"/> which is the default <see cref="SequenceMedia"/> of the factory
        /// </summary>
        /// <returns>The creation</returns>
        public virtual SequenceMedia CreateSequenceMedia()
        {
            IMedia newMedia = CreateMedia("SequenceMedia", ToolkitSettings.XUK_NS);
            if (newMedia is SequenceMedia) return (SequenceMedia) newMedia;
            throw new exception.FactoryCannotCreateTypeException(
                "The factory unexpectedly could not create an SequenceMedia");
        }

        #endregion
    }
}