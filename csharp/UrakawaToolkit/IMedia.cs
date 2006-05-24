 using System;
using urakawa;
using urakawa.xuk;

namespace urakawa.media
{
	/// <summary>
	/// This is the base interface for all media-related classes and interfaces.  
	/// Media is continuous (time-based) or discrete (static), and is of a specific type.
	/// </summary>
	public interface IMedia:urakawa.xuk.IXUKable
	{
		bool isContinuous();
		bool isDiscrete();
		/// <summary>
		/// tells you if the media object itself is a sequence
		/// does not tell you if your individual media object is part of a sequence
		/// </summary>
		/// <returns></returns>
		bool isSequence();
		MediaType getType();
		IMedia copy();
	}
}
