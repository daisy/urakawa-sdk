using System;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class AudioMedia : IAudioMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		private MediaLocation mMediaLocation = new MediaLocation();
		
		//internal constructor encourages use of MediaFactory to create AudioMedia objects
		internal AudioMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "AudioMedia";
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
				throw new exception.MethodParameterIsNullException("AudioMedia.setClipBegin (null) caused MethodParameterIsNullException");
			}

			if (beginPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.setClipBegin (" + 
					beginPoint.ToString() + ") caused TimeOffsetIsNegativeException");

			}

			mClipBegin = (Time)beginPoint;
		}

		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setClipEnd (null) caused MethodParameterIsNullException");
			}

			if (endPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.setClipEnd (" + 
					endPoint.ToString() + ") caused TimeOffsetIsNegativeException");

			}
			mClipEnd = (Time)endPoint;
		}

		public IClippedMedia split(ITime splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.split (null) caused MethodParameterIsNullException");
			}
	
			if (splitPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.split (" + 
					splitPoint.ToString() + ") caused TimeOffsetIsNegativeException");
				
				
			}


			AudioMedia splitMedia = new AudioMedia();
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
				throw new exception.MethodParameterIsNullException("AudioMedia.setLocation(null) caused MethodParameterIsNullException");
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

		public bool isSequence()
		{
			return false;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.AUDIO;
		}
		
		IMedia IMedia.copy()
		{
			return copy();
		}

		public AudioMedia copy()
		{
			AudioMedia newMedia = new AudioMedia();
			newMedia.setClipBegin(this.getClipBegin().copy());
			newMedia.setClipEnd(this.getClipEnd().copy());
			newMedia.setLocation(this.getLocation().copy());

			return newMedia;
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
