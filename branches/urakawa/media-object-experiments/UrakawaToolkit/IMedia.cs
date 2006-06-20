 using System;
using urakawa;

namespace urakawa.media
{
	/// <summary>
	/// This is the base interface for all media-related classes and interfaces.  
	/// Media is continuous (time-based) or discrete (static), and is of a specific type.
	/// </summary>
	public interface IMedia:urakawa.core.IXUKable
	{
    /// <summary>
    /// Determines if the <see cref="IMedia"/> is continuous
    /// </summary>
    /// <returns><c>true</c> if the <see cref="IMedia"/> is continuous, <c>false</c> else</returns>
		bool isContinuous();

    /// <summary>
    /// Convenience Equivalent to <c>!<see cref="isContinuous"/>()</c>
    /// </summary>
    /// <returns><c>!<see cref="isContinuous"/>()</c></returns>
		bool isDiscrete();
		
    /// <summary>
		/// tells you if the media object itself is a sequence
		/// does not tell you if your individual media object is part of a sequence
		/// </summary>
		/// <returns></returns>
		bool isSequence();

    /// <summary>
    /// Gets the <see cref="MediaType"/> of the <see cref="IMedia"/>
    /// </summary>
    /// <returns>The <see cref="MediaType"/></returns>
		MediaType getType();

    /// <summary>
    /// Gets a copy of the <see cref="IMedia"/>
    /// </summary>
    /// <returns></returns>
		IMedia copy();
	}
}
