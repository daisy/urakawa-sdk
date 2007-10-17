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
		/// Gets the <see cref="Presentation"/> that owns the manager
		/// </summary>
		/// <returns>The <see cref="Presentation"/> that owns <c>this</c></returns>
		Presentation getPresentation();

		/// <summary>
		/// Initializes the <see cref="IDataProviderManager"/> with 
		/// a owning the <see cref="Presentation"/>.
		/// </summary>
		///	<param name="ownerPres">The <see cref="Presentation"/> that owns <c>this</c></param>
		void setPresentation(Presentation ownerPres);

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> of the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		IDataProviderFactory getDataProviderFactory();

		/// <summary>
		/// Gets the UID of a given <see cref="IDataProvider"/>
		/// </summary>
		/// <param name="provider">The given data provider</param>
		/// <returns>The UID of <paramref name="provider"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="provider"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when data provider <paramref name="provider"/> is not managed by <c>this</c>
		/// </exception>
		string getUidOfDataProvider(IDataProvider provider);

		/// <summary>
		/// Gets the <see cref="IDataProvider"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The data provider with the given UID</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// When no data providers managed by <c>this</c> has the given UID
		/// </exception>
		IDataProvider getDataProvider(string uid);

		/// <summary>
		/// Determines if the manager manages a <see cref="IDataProvider"/> with a given uid
		/// </summary>
		/// <param name="uid">The given uid</param>
		/// <returns>
		/// A <see cref="bool"/> indicating if the manager manages a <see cref="IDataProvider"/> with the given uid
		/// </returns>
		bool isManagerOf(string uid);

		/// <summary>
		/// Removes one of the <see cref="IDataProvider"/>s managed by the manager
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to remove</param>
		/// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted as well</param>
		void removeDataProvider(IDataProvider provider, bool delete);

		/// <summary>
		/// Removes the <see cref="IDataProvider"/> with a given UID from the manager
		/// </summary>
		/// <param name="uid">The uid of the provider to remove</param>
		/// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted as well</param>
		void removeDataProvider(string uid, bool delete);

		/// <summary>
		/// Adds a <see cref="IDataProvider"/> to the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to add</param>
		void addDataProvider(IDataProvider provider);

		/// <summary>
		/// Adds a <see cref="IDataProvider"/> to the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <param name="provider">The <see cref="IDataProvider"/> to add</param>
		/// <param name="uid">The uid to associate with the added data provider</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="provider"/> or <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when the data provider is already added tothe manager 
		/// or if the manager already manages another data provider with the given uid
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">Thrown if the data provides does not have <c>this</c> as manager</exception>
		void addDataProvider(IDataProvider provider, string uid);

		/// <summary>
		/// Gets a list of the <see cref="IDataProvider"/>s that is managed by the <see cref="IDataProviderManager"/>
		/// </summary>
		/// <returns>A <see cref="List{IDataProvider}"/> conatining the managed <see cref="IDataProvider"/>s</returns>
		List<IDataProvider> getListOfDataProviders();

		/// <summary>
		/// Removes any <see cref="IDataProvider"/>s "not used", 
		/// that is all <see cref="IDataProvider"/>s that are not used by a <see cref="MediaData"/> of the <see cref="Presentation"/>
		/// </summary>
		/// <param name="delete">A <see cref="bool"/> indicating if the removed data providers should be deleted</param>
		void removeUnusedDataProviders(bool delete);
	}
}
