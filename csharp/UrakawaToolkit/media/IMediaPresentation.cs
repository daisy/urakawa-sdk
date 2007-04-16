using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.media
{
	/// <summary>
	/// Interface for a presentation that supports <see cref="IMedia"/>
	/// </summary>
	public interface IMediaPresentation : ICorePresentation
	{
		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> creating <see cref="IMedia"/>
		/// for the <see cref="IMediaPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaPresentation"/></returns>
		IMediaFactory getMediaFactory();

		/// <summary>
		/// Gets the base uri for relative uri's of <see cref="IMedia"/> objects in the presentaion
		/// </summary>
		/// <returns>The base uri</returns>
		Uri getBaseUri();

		/// <summary>
		/// Sets the base uri for relative uri's of <see cref="IMedia"/> objects in the presentaion
		/// </summary>
		/// <param name="newBase">The new base uri</param>
		void setBaseUri(Uri newBase);
	}
}
