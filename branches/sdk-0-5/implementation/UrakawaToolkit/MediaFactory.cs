using System;

namespace urakawa.media
{
	/// <summary>
	/// The media factory will create any media object of MediaType.xxx
	/// </summary>
	public class MediaFactory : IMediaFactory
	{
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
				return createAudioMedia();
			}

			else if (type == MediaType.IMAGE)
			{
				return createImageMedia();
			}

			else if (type == MediaType.TEXT)
			{
				return createTextMedia();
			}

			else if (type == MediaType.VIDEO)
			{
				return createVideoMedia();
			}
			else if (type == MediaType.EMPTY_SEQUENCE)
			{
				return createEmptySequenceMedia();
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
					case "AudioMedia":
						return createAudioMedia();
					case "ImageMedia":
						return createImageMedia();
					case "VideoMedia":
						return createVideoMedia();
					case "TextMedia":
						return createTextMedia();
					case "SequenceMedia":
						return createEmptySequenceMedia();
				}
			}
			return null;
		}


		#endregion

		private IAudioMedia createAudioMedia()
		{
			return AudioMedia.create();
		}

		private IImageMedia createImageMedia()
		{
			return ImageMedia.create();
		}

		private IVideoMedia createVideoMedia()
		{
			return VideoMedia.create();
		}

		private ITextMedia createTextMedia()
		{
			return TextMedia.create();
		}

		private ISequenceMedia createEmptySequenceMedia()
		{
			return SequenceMedia.create(this);
		}
	}
}
