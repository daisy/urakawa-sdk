using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Common interface for <see cref="IMedia"/> that use <see cref="MediaData"/> to store their content
	/// </summary>
	public interface IManagedMedia
	{

		/// <summary>
		/// Gets the <see cref="MediaData"/> storing the content
		/// </summary>
		/// <returns>The media data</returns>
		MediaData getMediaData();

		/// <summary>
		/// Sets the <see cref="MediaData"/> storing the content
		/// </summary>
		/// <param name="data">The new media data</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		void setMediaData(MediaData data);

		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/> creating the <see cref="MediaData"/>
		/// used by the <see cref="IManagedMedia"/>.
		/// Convenience for <c>getMediaData().getMediaDataManager().getMediaDataFactory()</c>
		/// </summary>
		/// <returns>The media data factory</returns>
		MediaDataFactory getMediaDataFactory();
	}
}
