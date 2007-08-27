using System;

namespace urakawa.media
{
	/// <summary>
	/// The media factory will create any media object of MediaType.xxx
	/// </summary>
	public class MediaFactory : WithPresentation, IMediaFactory
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		public MediaFactory()
		{
		}

		#region IMediaFactory Members

		/// <summary>
		/// Creates an <see cref="IMedia"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The creates <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
		public IMedia createMedia(string localName, string namespaceUri)
		{
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "ManagedAudioMedia":
						return new data.audio.ManagedAudioMedia(
							this,	(data.audio.AudioMediaData)getPresentation().getMediaDataFactory().createMediaData(
							typeof(data.audio.codec.WavAudioMediaData)));
					case "ExternalAudioMedia":
						return new ExternalAudioMedia(this);
					case "ExternalImageMedia":
						return new ExternalImageMedia(this);
					case "ExternalVideoMedia":
						return new ExternalVideoMedia(this);
					case "TextMedia":
						return new TextMedia(this);
					case "SequenceMedia":
						return new SequenceMedia(this);
					case "ExternalTextMedia":
						return new ExternalTextMedia(this);

				}
			}
			return null;
		}

		/// <summary>
		/// Creates a <see cref="data.audio.ManagedAudioMedia"/> which is the default <see cref="IAudioMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual IAudioMedia createAudioMedia()
		{
			IMedia newMedia = createMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS);
			if (newMedia is IAudioMedia) return (IAudioMedia)newMedia;
			throw new exception.FactoryCanNotCreateTypeException(
				"The factory unexpectedly could not create a ManagedAudioMedia");
		}

		/// <summary>
		/// Creates a <see cref="TextMedia"/> which is the default <see cref="ITextMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual ITextMedia createTextMedia()
		{
			IMedia newMedia = createMedia("TextMedia", ToolkitSettings.XUK_NS);
			if (newMedia is ITextMedia) return (ITextMedia)newMedia;
			throw new exception.FactoryCanNotCreateTypeException(
				"The factory unexpectedly could not create a TextMedia");

		}

		/// <summary>
		/// Creates a <see cref="ExternalImageMedia"/> which is the default <see cref="IImageMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual IImageMedia createImageMedia()
		{
			IMedia newMedia = createMedia("ExternalImageMedia", ToolkitSettings.XUK_NS);
			if (newMedia is IImageMedia) return (IImageMedia)newMedia;
			throw new exception.FactoryCanNotCreateTypeException(
				"The factory unexpectedly could not create an ExternalImageMedia");
		}

		/// <summary>
		/// Creates a <see cref="ExternalVideoMedia"/> which is the default <see cref="IVideoMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual IVideoMedia createVideoMedia()
		{
			IMedia newMedia = createMedia("ExternalVideoMedia", ToolkitSettings.XUK_NS);
			if (newMedia is IVideoMedia) return (IVideoMedia)newMedia;
			throw new exception.FactoryCanNotCreateTypeException(
				"The factory unexpectedly could not create an ExternalVideoMedia");
		}

		/// <summary>
		/// Creates a <see cref="SequenceMedia"/> which is the default <see cref="SequenceMedia"/> of the factory
		/// </summary>
		/// <returns>The creation</returns>
		public virtual SequenceMedia createSequenceMedia()
		{
			IMedia newMedia = createMedia("SequenceMedia", ToolkitSettings.XUK_NS);
			if (newMedia is SequenceMedia) return (SequenceMedia)newMedia;
			throw new exception.FactoryCanNotCreateTypeException(
				"The factory unexpectedly could not create an SequenceMedia");
		}

		#endregion
	}
}
