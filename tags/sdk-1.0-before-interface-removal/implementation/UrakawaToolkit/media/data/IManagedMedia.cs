using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Common interface for <see cref="IMedia"/> that use <see cref="IMediaData"/> to store their content
	/// </summary>
	public interface IManagedMedia
	{

		/// <summary>
		/// Gets the <see cref="IMediaData"/> storing the content
		/// </summary>
		/// <returns>The media data</returns>
		IMediaData getMediaData();

		/// <summary>
		/// Sets the <see cref="IMediaData"/> storing the content
		/// </summary>
		/// <param name="data">The new media data</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		void setMediaData(IMediaData data);

		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/> creating the <see cref="IMediaData"/>
		/// used by the <see cref="IManagedMedia"/>.
		/// Convenience for <c>getMediaData().getMediaDataManager().getMediaDataFactory()</c>
		/// </summary>
		/// <returns>The media data factory</returns>
		IMediaDataFactory getMediaDataFactory();
	}
}
