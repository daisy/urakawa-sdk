using System;

namespace urakawa.media
{
	/// <summary>
	/// This interface is for referring to time-based segments of external media
	/// </summary>
	public interface IClippedMedia : IExternalMedia
	{ 
		/// <summary>
		/// Return the duration of the clip.
		/// </summary>
		/// <returns>The duration</returns>
		ITimeDelta getDuration();
		/// <summary>
		/// Get the begin <see cref="ITime"/> for the clip.
		/// </summary>
		/// <returns>The begin <see cref="ITime"/></returns>
		ITime getClipBegin();
		/// <summary>
		/// Get the end <see cref="ITime"/> for the clip.
		/// </summary>
		/// <returns>The end <see cref="ITime"/></returns>
		ITime getClipEnd();		
		/// <summary>
		/// Set the begin <see cref="ITime"/> for the clip.
		/// </summary>
		/// <param name="beginPoint">The new begin <see cref="ITime"/></param>
		/// <exception cref="exception.TimeOffsetIsOutOfBoundsException">
		/// Thrown when <c><paramref name="beginPoint"/> 
		/// is not between <c>0</c> and <c><see cref="getEndTime"/>()</c>
		/// </exception>
		void setClipBegin(ITime beginPoint);
		/// <summary>
		/// Set the end <see cref="ITime"/> for the clip.
		/// </summary>
		/// <param name="endPoint">The new end <see cref="ITime"/></param>
		/// <exception cref="exception.TimeOffsetIsOutOfBoundsException">
		/// Thrown when <c><paramref name="beginPoint"/>&gt;<see cref="getEndTime"/>()</c>
		/// </exception>
		void setClipEnd(ITime endPoint);
		/// <summary>
		/// Split the <see cref="IClippedMedia"/> object at the given point in time.
		/// </summary>
		/// <param name="splitPoint">The <see cref="ITime"/> at which to split</param>
		/// <returns></returns>
		/// <exception cref="exception.TimeOffsetIsOutOfBoundsException">
		/// Thrown when <paramref name="splitPoint"/> is not between
		/// <c><see cref="getClipBegin"/>()</c> and <c><see cref="getClipEnd"/>()</c>
		/// </exception>
		IClippedMedia split(ITime splitPoint);
	}
}
