using System;

namespace urakawa.media
{
	/// <summary>
	/// The media factory will create any media object of MediaType.xxx
	/// </summary>
	public class MediaFactory : IMediaFactory
	{
		public MediaFactory()
		{
			// 
			// TODO: Add constructor logic here
			//
		}
		#region IMediaFactory Members

		public IMedia createMedia(urakawa.media.MediaType type)
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

			else 
			{
				throw new exception.MediaTypeIsIllegalException("MediaFactory.createMedia(" + 
					type.ToString() + ") caused MediaTypeIsIllegalException");
				return null;
			}
		}

		#endregion

		private IAudioMedia createAudioMedia()
		{
			return new AudioMedia();
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
	}
}
