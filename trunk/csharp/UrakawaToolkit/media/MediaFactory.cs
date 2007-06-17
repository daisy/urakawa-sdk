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
		/// Create a media object of the given type.
		/// </summary>
		/// <param name="type">The type of media object to create.</param>
		/// <returns>a new MediaObject of a specific type.</returns>
		public virtual IMedia createMedia(urakawa.media.MediaType type)
		{
			if (type == MediaType.AUDIO)
			{
				return createMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS);
			}

			else if (type == MediaType.IMAGE)
			{
				return createMedia("ImageMedia", ToolkitSettings.XUK_NS);
			}

			else if (type == MediaType.TEXT)
			{
				return createMedia("TextMedia", ToolkitSettings.XUK_NS);
			}

			else if (type == MediaType.VIDEO)
			{
				return createMedia("VideoMedia", ToolkitSettings.XUK_NS);
			}
			else if (type == MediaType.EMPTY_SEQUENCE)
			{
				return createMedia("SequenceMedia", ToolkitSettings.XUK_NS);
			}

			else
			{
				throw new exception.MediaTypeIsIllegalException("MediaFactory.createMedia(" +
					type.ToString() + ") caused MediaTypeIsIllegalException");
			}
		}

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
						return new data.ManagedAudioMedia(
							this,	(data.AudioMediaData)getPresentation().getMediaDataFactory().createMediaData(
							typeof(data.audio.codec.WavAudioMediaData)));
					case "ExternalAudioMedia":
						return new ExternalAudioMedia(this);
					case "ImageMedia":
						return new ImageMedia(this);
					case "VideoMedia":
						return new VideoMedia(this);
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

		#endregion
	}
}
