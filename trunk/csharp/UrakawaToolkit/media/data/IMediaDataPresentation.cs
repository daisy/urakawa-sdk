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
		/// Gets the <see cref="IMediaDataManager"/> managing the <see cref="IMediaData"/> of the <see cref="IMediaDataPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaDataManager"/></returns>
		IMediaDataManager getMediaDataManager();

		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/> creating <see cref="IMediaData"/> for the <see cref="IMediaDataPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaDataFactory"/></returns>
		IMediaDataFactory getMediaDataFactory();

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> creating <see cref="IDataProvider"/>s for <see cref="IMediaData"/>
		/// of the <see cref="IMediaDataPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		IDataProviderFactory getDataProviderFactory();
	}
}
