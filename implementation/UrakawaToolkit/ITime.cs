using System;

namespace urakawa.media
{
	/// <summary>
	/// A simple interface for a measure of time.
	/// </summary>
	public interface ITime
	{
		/// <summary>
		/// Determines if the time offset is negative.
		/// </summary>
		/// <returns></returns>
		bool isNegativeTimeOffset();
		/// <summary>
		/// Copy this object.
		/// </summary>
		/// <returns></returns>
		ITime copy();
	}
}
