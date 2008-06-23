using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.timing;

namespace urakawa.media.data.utilities
{
	/// <summary>
	/// Represents a generic media clip
	/// </summary>
	public abstract class Clip
	{
		private Time mClipBegin = new Time();
		/// <summary>
		/// Gets (a copy of) the clip begin <see cref="Time"/> of <c>this</c>
		/// </summary>
		/// <returns>
		/// The clip begin <see cref="Time"/> - can not be <c>null</c>
		/// </returns>
		public Time getClipBegin()
		{
			return mClipBegin.copy();
		}

		/// <summary>
		/// Sets the clip begin <see cref="Time"/> of <c>this</c>
		/// </summary>
		/// <param name="newClipBegin">The new clip begin <see cref="Time"/> - can not be <c>null</c></param>
		public void setClipBegin(Time newClipBegin)
		{
			if (newClipBegin == null)
			{
				throw new exception.MethodParameterIsNullException("Clip begin of a WavClip can not be null");
			}
			if (newClipBegin.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The new clip begin is beyond the current clip end");
			}
			mClipBegin = newClipBegin.copy();
		}

		private Time mClipEnd = null;
		/// <summary>
		/// Gets (a copy of) the clip end <see cref="Time"/> of <c>this</c>
		/// </summary>
		/// <returns>The clip end <see cref="Time"/></returns>
		public Time getClipEnd()
		{
			if (mClipEnd == null) return Time.Zero.addTimeDelta(getMediaDuration());
			return mClipEnd.copy();
		}

		/// <summary>
		/// Sets the clip end <see cref="Time"/> of <c>this</c>
		/// </summary>
		/// <param name="newClipEnd">
		/// The new clip end <see cref="Time"/> 
		/// - a <c>null</c> ties the clip end to the end of the underlying wave audio
		/// </param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new clip end <see cref="Time"/> is less that the current clip begin <see cref="Time"/>
		/// </exception>
		/// <remarks>
		/// There is not check to see if the new clip end <see cref="Time"/> 
		/// is beyond the end of the underlyind wave audio
		/// </remarks>
		public void setClipEnd(Time newClipEnd)
		{
			if (newClipEnd == null)
			{
				mClipEnd = null;
			}
			else
			{
				if (newClipEnd.isLessThan(getClipBegin()))
				{
					throw new exception.MethodParameterIsOutOfBoundsException(
						"The new clip end time is before current clip begin");
				}
				mClipEnd = newClipEnd.copy();
			}
		}

		/// <summary>
		/// Determines if clip end is tied to the end of the underlying media
		/// </summary>
		/// <returns>
		/// A <see cref="bool"/> indicating if clip end is tied to the end of the underlying media
		/// </returns>
		public bool isClipEndTiedToEOM()
		{
			return (mClipEnd == null);
		}

		/// <summary>
		/// Gets the duration of the clip
		/// </summary>
		/// <returns>The duration of as a <see cref="TimeDelta"/></returns>
		public TimeDelta getDuration()
		{
			return getClipEnd().getTimeDelta(getClipBegin());
		}

		/// <summary>
		/// Gets the duration of the underlying media
		/// </summary>
		/// <returns>The duration of the underlying media</returns>
		public abstract TimeDelta getMediaDuration();
	}
}
