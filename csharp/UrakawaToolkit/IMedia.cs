using System;
using urakawa;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public interface IMedia
	{
		bool isContinuous();
		bool isDiscrete();
		MediaType getType();
	}
}
