using System;

namespace urakawa.media
{
	/// <summary>
	/// the difference between two <see cref="ITime"/> objects is an ITimeDelta
	/// </summary>
	public interface ITimeDelta
	{
		/// <summary>
		/// Determines if the difference is negative
		/// </summary>
		/// <returns></returns>
		bool isNegative();	
	}
}
