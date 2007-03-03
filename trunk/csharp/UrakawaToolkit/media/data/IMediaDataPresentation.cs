using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a <see cref="IMediaPresentation"/> that supports <see cref="IMediaData"/>
	/// </summary>
	public interface IMediaDataPresentation : IMediaPresentation
	{
		/// <summary>
		/// Gets the manager of the <see cref="IMediaData"/> of the presentation
		/// </summary>
		/// <returns>The media data manager</returns>
		IMediaDataManager getMediaDataManager();

		/// <summary>
		/// Gets the factory creating <see cref="IMediaData"/> for the presentation.
		/// Convenience for <c>this.getMediaDataManager().getMediaDataFactory()</c>.
		/// </summary>
		/// <returns>The media data factory</returns>
		IMediaDataFactory getMediaDataFactory();


		/// <summary>
		/// Gets the manager managing the <see cref="IDataProvider"/>s 
		/// of the <see cref="IMediaData"/> of the presentation
		/// </summary>
		/// <returns>The manager</returns>
		IDataProviderManager getDataProviderManager();
	}
}
