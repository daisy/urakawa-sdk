using System;

namespace urakawa.media
{
	/// <summary>
	/// Used internally to simplify implementation of media objects.
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
		/// Returns the difference between <see cref="mClipBegin"/> and <see cref="mClipEnd"/>
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

		/// <summary>
		/// Returns the start time for the clip
		/// </summary>
		/// <returns></returns>
		public Time getClipBegin()
		{
			return mClipBegin;
		}


		ITime IClippedMedia.getClipEnd()
		{
			return getClipEnd();
		}

		/// <summary>
		/// Returns the end time for the clip
		/// </summary>
		/// <returns></returns>
		public Time getClipEnd()
		{
			return mClipEnd;
		}

		/// <summary>
		/// Defines a new begin time for the clip.  Changes to audio media objects are non-destructive.
		/// This function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
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
		/// Defines a new end time for the clip.  Changes to audio media objects are non-destructive.
		/// This function will throw exceptions <see cref="exception.MethodParameterIsNullException"/>
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
		/// This function splits the media at a given point.
		/// The end time of this clip is shortened, and a new <see cref="ClippedMedia"/>
		/// object with the split point as its begin time is returned
		/// </summary>
		/// <param name="splitPoint">The time at which to split the media</param>
		/// <returns></returns>
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

			//You have to use IMedia here, because calling the copy function on it will invoke
			//the real-life copy functions on AudioMedia or VideoMedia
			//the IClippedMedia.copy function doesn't exist because ClippedMedia is a convenience class
			IMedia thisAsIMedia = this;
			IClippedMedia splitMedia = (IClippedMedia)thisAsIMedia.copy();
			splitMedia.setClipBegin(splitPoint);

			this.setClipEnd(splitPoint);

			return splitMedia;
		}
		
		#endregion
	}
}
