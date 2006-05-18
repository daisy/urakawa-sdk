using System;

namespace urakawa.media
{
	/// <summary>
	/// VideoMedia is the video object.
	/// It is time-based, comes from an external source, and has a visual presence.
	/// </summary>
	public class VideoMedia : IVideoMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		private MediaLocation mMediaLocation = new MediaLocation();
		int mWidth;
		int mHeight;

		
		//internal constructor encourages use of MediaFactory to create VideoMedia objects
		internal VideoMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "VideoMedia";
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
			if (beginPoint == null)
			{
				throw new exception.MethodParameterIsNullException("VideoMedia.setClipBegin (null) caused MethodParameterIsNullException");
			}

			if (beginPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("VideoMedia.setClipBegin (" + 
					beginPoint.ToString() + ") caused TimeOffsetIsNegativeException");
				
			}

			mClipBegin = (Time)beginPoint;
		}

		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("VideoMedia.setClipEnd (null) caused MethodParameterIsNullException");
			}

			if (endPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("VideoMedia.setClipEnd (" + 
					endPoint.ToString() + ") caused TimeOffsetIsNegativeException");
			}
			mClipEnd = (Time)endPoint;
		}

		public IClippedMedia split(ITime splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException("VideoMedia.split (null) caused MethodParameterIsNullException");
			}

			if (splitPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("VideoMedia.split (" + 
					splitPoint.ToString() + ") caused TimeOffsetIsNegativeException");
			}

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
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("VideoMedia.setLocation(null) caused MethodParameterIsNullException");
			}
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

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion


	}
}
