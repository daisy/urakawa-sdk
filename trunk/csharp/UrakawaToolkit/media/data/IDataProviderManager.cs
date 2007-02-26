using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a <see cref="IDataProviderManager"/>
	/// </summary>
	public interface IDataProviderManager
	{
		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> that owns the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IMediaDataPresentation"/> that owns <c>this</c></returns>
		IMediaDataPresentation getMediaDataPresentation();

		/// <summary>
		/// Initializes the <see cref="IDataProviderManager"/> with 
		/// a owning the <see cref="IMediaDataPresentation"/>.
		/// </summary>
		///	<param name="ownerPres">The <see cref="IMediaDataPresentation"/> that owns <c>this</c></param>
		void setMediaDataPresentation(IMediaDataPresentation ownerPres);

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> of the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		IDataProviderFactory getDataProviderFactory();

		/// <summary>
		/// Deletes one of the <see cref="IDataProvider"/>s managed by the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to delete</param>
		void deleteDataProvider(IDataProvider provider);

		/// <summary>
		/// Adds a <see cref="IDataProvider"/> to the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to add</param>
		void addDataProvider(IDataProvider provider);

		/// <summary>
		/// Gets a list of the <see cref="IDataProvider"/>s that is managed by the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>A <see cref="IList{IDataProvider}"/> conatining the managed <see cref="IDataProvider"/>s</returns>
		IList<IDataProvider> getListOfManagedDataProvider();
	}
}
