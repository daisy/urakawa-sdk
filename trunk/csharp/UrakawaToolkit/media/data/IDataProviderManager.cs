using System;
using System.Collections.Generic;
using System.Text;
using urakawa.xuk;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a <see cref="IDataProviderManager"/>
	/// </summary>
	public interface IDataProviderManager : IXukAble, IValueEquatable<IDataProviderManager>
	{
		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> that owns the manager
		/// </summary>
		/// <returns>The <see cref="IMediaDataPresentation"/> that owns <c>this</c></returns>
		IMediaDataPresentation getMediaDataPresentation();

		/// <summary>
		/// Initializes the <see cref="IDataProviderManager"/> with 
		/// a owning the <see cref="IMediaDataPresentation"/>.
		/// </summary>
		///	<param name="ownerPres">The <see cref="IMediaDataPresentation"/> that owns <c>this</c></param>
		void setPresentation(IMediaDataPresentation ownerPres);

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> of the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		IDataProviderFactory getDataProviderFactory();

		string getUidOfDataProvider(IDataProvider provider);

		IDataProvider getDataProvider(string uid);

		/// <summary>
		/// Detaches one of the <see cref="IDataProvider"/>s managed by the manager
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to delete</param>
		void detachDataProvider(IDataProvider provider);

		/// <summary>
		/// Detaches the <see cref="IDataProvider"/> with a given UID from the manager
		/// </summary>
		/// <param name="uid">The given UID</param>
		void detachDataProvider(string uid);

		/// <summary>
		/// Adds a <see cref="IDataProvider"/> to the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to add</param>
		void addDataProvider(IDataProvider provider);

		/// <summary>
		/// Gets a list of the <see cref="IDataProvider"/>s that is managed by the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>A <see cref="IList{IDataProvider}"/> conatining the managed <see cref="IDataProvider"/>s</returns>
		IList<IDataProvider> getListOfManagedDataProviders();
	}
}
