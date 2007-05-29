using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data;

namespace urakawa.media
{
	/// <summary>
	/// Interface for an <see cref="IMedia"/> that uses one or more <see cref="IMediaData"/> to store content.
	/// </summary>
	public interface IDataMedia : IMedia
	{
		/// <summary>
		/// Gets the list of <see cref="IMediaData"/> used by <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		IList<IMediaData> getMediaData();
	}
}
