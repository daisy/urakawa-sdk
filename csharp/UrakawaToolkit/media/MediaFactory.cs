using System;

namespace urakawa.media
{
	/// <summary>
	/// The media factory will create any media object of MediaType.xxx
	/// </summary>
	public class MediaFactory : IMediaFactory
	{
		private data.IMediaDataPresentation mPresentation = null;

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
					case "ImageMedia":
						return new ExternalImageMedia(this);
					case "VideoMedia":
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

		IMediaPresentation IMediaFactory.getPresentation()
		{
			return getPresentation();
		}

		/// <summary>
		/// Gets the <see cref="IMediaPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaPresentation"/></returns>
		public data.IMediaDataPresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"No media presentation has yet been associated with the media factory");
			}
			return mPresentation;
		}

		void IMediaFactory.setPresentation(IMediaPresentation pres)
		{
			if (!(pres is data.IMediaDataPresentation))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The presentation of a MediaFactory must be a MediaDataPresentation");
			}
			setPresentation((data.IMediaDataPresentation)pres);
		}

		/// <summary>
		/// Initiaælizes <c>this</c> with an associated <see cref="IMediaPresentation"/>
		/// </summary>
		/// <param name="pres">The associated <see cref="IMediaPresentation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="pres"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when another presentation has already been associated with <c>this</c>
		/// </exception>
		public void setPresentation(data.IMediaDataPresentation pres)
		{
			if (pres==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The media factory can not be associated with a null media presentation");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The media presentation has already been associated with a meida presentation");
			}
			mPresentation = pres;
		}

		/// <summary>
		/// Creates a <see cref="data.ManagedAudioMedia"/> which is the default <see cref="IAudioMedia"/> of the factory
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
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
