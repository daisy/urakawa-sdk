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
			//throws an exception
			IMedia thisAsIMedia = this;
			IClippedMedia splitMedia = (IClippedMedia)thisAsIMedia.copy();
			splitMedia.setClipBegin(splitPoint);

			this.setClipEnd(splitPoint);

			return splitMedia;
		}
		
		#endregion
	}
}
