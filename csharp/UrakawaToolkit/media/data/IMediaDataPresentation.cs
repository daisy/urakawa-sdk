using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a <see cref="IMediaPresentation"/> that supports <see cref="MediaData"/>
	/// </summary>
	public interface IMediaDataPresentation : IMediaPresentation
	{
		/// <summary>
		/// Gets the manager of the <see cref="MediaData"/> of the presentation
		/// </summary>
		/// <returns>The media data manager</returns>
		MediaDataManager getMediaDataManager();

		/// <summary>
		/// Gets the factory creating <see cref="MediaData"/> for the presentation.
		/// Convenience for <c>this.getMediaDataManager().getMediaDataFactory()</c>.
		/// </summary>
		/// <returns>The media data factory</returns>
		IMediaDataFactory getMediaDataFactory();


		/// <summary>
		/// Gets the manager managing the <see cref="IDataProvider"/>s 
		/// of the <see cref="MediaData"/> of the presentation
		/// </summary>
		/// <returns>The manager</returns>
		IDataProviderManager getDataProviderManager();
	}
}
