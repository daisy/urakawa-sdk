using System;

namespace urakawa.media
{
	/// <summary>
	/// for internal use
	/// </summary>
	public abstract class ClippedMedia : ExternalMedia, IClippedMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		
		internal ClippedMedia()
		{
			
		}

		#region IClippedMedia Members

		/// <summary>
		/// returns the difference between <see cref="mClipBegin"/> and <see cref="mClipEnd"/>
		/// </summary>
		/// <returns></returns>
		public ITimeDelta getDuration()
		{
			return mClipBegin.getTimeDelta(mClipEnd);
		}

		ITime IClippedMedia.getClipBegin()
		{
			return getClipBegin();
		}

		public Time getClipBegin()
		{
			return mClipBegin;
		}


		ITime IClippedMedia.getClipEnd()
		{
			return getClipEnd();
		}

		public Time getClipEnd()
		{
			return mClipEnd;
		}

		/// <summary>
		/// defines a new begin time for the clip.  changes to audio media objects are non-destructive.
		/// this function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
		/// and <see cref="exception.TimeOffsetIsNegativeException"/> if provoked.
		/// </summary>
		/// <param name="beginPoint">the new begin time</param>
		public void setClipBegin(ITime beginPoint)
		{
			if (beginPoint == null)
			{
				throw new exception.MethodParameterIsNullException("clip begin parameter cannot be null");
			}

			if (beginPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("clip begin parameter = " + 
					beginPoint.ToString());
			}
			mClipBegin = (Time)beginPoint;
		}

		/// <summary>
		/// defines a new end time for the clip.  changes to audio media objects are non-destructive.
		/// this function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
		/// and <see cref="exception.TimeOffsetIsNegativeException"/> if provoked.
		/// </summary>
		/// <param name="endPoint">the new end time</param>
		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("end point parameter cannot be null");
			}

			if (endPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("clip end parameter = " + 
					endPoint.ToString());

			}
			mClipEnd = (Time)endPoint;
		}

		/// <summary>
		/// split the object at the time given and return the later portion
		/// this function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
		/// and <see cref="exception.TimeOffsetIsNegativeException"/> if provoked.
		/// </summary>
		/// <param name="splitPoint">the time when the audio object should be split</param>
		/// <returns>a new object representing the later portion of the recently-split media object</returns>
		public IClippedMedia split(ITime splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException("split point parameter cannot be null");
			}
	
			if (splitPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("split point parameter = " + 
					splitPoint.ToString());		
			}

			AudioMedia splitMedia = new AudioMedia();
			splitMedia.setClipBegin(splitPoint);
			splitMedia.setClipEnd(mClipEnd);

			this.setClipEnd(splitPoint);

			return splitMedia;
		}

		#endregion


    /*

		#region IExternalMedia Members

		public abstract IMediaLocation getLocation();

		public abstract void setLocation(IMediaLocation location);

		#endregion

		#region IMedia Members

		public abstract bool isContinuous();

		public abstract bool isDiscrete();

		public abstract bool isSequence();

		public abstract urakawa.media.MediaType getType();

		public abstract IMedia copy();
    
    #endregion

		#region IXUKable Members

		public abstract bool XUKin(System.Xml.XmlReader source);

		public abstract bool XUKout(System.Xml.XmlWriter destination);

		#endregion
    */
	}
}
