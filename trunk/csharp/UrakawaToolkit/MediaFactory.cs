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

		#endregion

		private IAudioMedia createAudioMedia()
		{
			return AudioMedia.create();
		}

		private IImageMedia createImageMedia()
		{
			return new ImageMedia();
		}

		private IVideoMedia createVideoMedia()
		{
			return new VideoMedia();
		}

		private ITextMedia createTextMedia()
		{
			return new TextMedia();
		}

		private ISequenceMedia createEmptySequenceMedia()
		{
			return new SequenceMedia(this);
		}
	}
}
