using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for ImageObject.
	/// </summary>
	public class ImageMedia : IImageMedia
	{
		int mWidth;
		int mHeight;
		MediaLocation mMediaLocation = new MediaLocation();

		public ImageMedia()
		{
			mWidth = 0;
			mHeight = 0;
		}

		public ImageMedia(int width, int height)
		{
			mWidth = width;
			mHeight = height;
		}

		public void setWidth(int width)
		{
			mWidth = width;
		}

		public void setHeight(int height)
		{
			mHeight = height;
		}

		#region IExternalMedia Members

		public IMediaLocation getLocation()
		{
			return mMediaLocation;
		}

		public void setLocation(IMediaLocation location)
		{
			mMediaLocation = (MediaLocation)location;
		}

		#endregion

		#region IMedia Members

		public bool isContinuous()
		{
			return false;
		}

		public bool isDiscrete()
		{
			return true;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.IMAGE;
		}

		#endregion

		#region IImageSize Members

		public int getWidth()
		{
			return mWidth;
		}

		public int getHeight()
		{
			return mHeight;
		}

		#endregion
	}
}
