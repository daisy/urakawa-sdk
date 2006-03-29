using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for VideoObject.
	/// </summary>
	public class VideoMedia : IVideoMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		private MediaLocation mMediaLocation = new MediaLocation();
		int mWidth;
		int mHeight;

		public VideoMedia()
		{
		}

		public void setWidth(int width)
		{
			mWidth = width;
		}

		public void setHeight(int height)
		{
			mHeight = height;
		}

		#region IClippedMedia Members

		public ITimeDelta getDuration()
		{
			return mClipBegin.getDelta(mClipEnd);
		}

		public ITime getClipBegin()
		{
			return mClipBegin;
		}

		public ITime getClipEnd()
		{
			return mClipEnd;
		}

		public void setClipBegin(ITime beginPoint)
		{
			mClipBegin = (Time)beginPoint;
		}

		public void setClipEnd(ITime endPoint)
		{
			mClipEnd = (Time)endPoint;
		}

		public IClippedMedia split(ITime splitPoint)
		{
			VideoMedia splitMedia = new VideoMedia();
			splitMedia.setClipBegin(splitPoint);
			splitMedia.setClipEnd(mClipEnd);

			this.setClipEnd(splitPoint);

			return splitMedia;
		}


		#endregion

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
			return true;
		}

		public bool isDiscrete()
		{
			return false;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.VIDEO;
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
