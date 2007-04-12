using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a generic <see cref="IMediaLocation"/> that points to <see cref="IMediaData"/>
	/// </summary>
	public interface IMediaDataLocation : IMediaLocation
	{
		/// <summary>
		/// Gets the <see cref="IMediaData"/> pointed to by <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaData"/></returns>
		IMediaData getMediaData();

		/// <summary>
		/// Sets the <see cref="IMediaData"/> pointed to by this
		/// </summary>
		/// <param name="newData">The new <see cref="IMediaData"/> that <c>this</c> should point to</param>
		void setMediaData(IMediaData newData);

		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataFactory"/></returns>
		IMediaDataFactory getMediaDataFactory();
	}
}
