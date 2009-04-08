using System;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// Interface for continuous time-based media
	/// </summary>
	public interface IContinuous
	{
		/// <summary>
		/// Gets the duration of the media
		/// </summary>
		/// <returns>The duration</returns>
		TimeDelta getDuration();

		/// <summary>
		/// Splits the continuous media at a given split point, leaving the instance with the part before the split point
		/// and creating a new media with the part after
		/// </summary>
		/// <param name="splitPoint">The given split point - must be between <c>00:00:00.000</c> and <c>getDuration()</c></param>
		/// <returns>The media with the part of the media after the split point</returns>
		IContinuous split(Time splitPoint);
	}
}
