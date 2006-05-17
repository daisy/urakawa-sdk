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
		bool isContinuous();
		bool isDiscrete();
		MediaType getType();
	}
}
