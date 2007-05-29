using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a generic <see cref="IMediaData"/>. Uses <see cref="IDataProvider"/>s to store actual data
	/// </summary>
	public interface IMediaData : xuk.IXukAble, IValueEquatable<IMediaData>
	{
		/// <summary>
		/// Gets the <see cref="IMediaDataManager"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The assicoated <see cref="IMediaDataManager"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when <c>this</c> has not been associated with a <see cref="IMediaDataManager"/>
		/// </exception>
		IMediaDataManager getMediaDataManager();

		/// <summary>
		/// Associates <c>this</c> with a <see cref="IMediaDataManager"/> - 
		/// initializer that is called in method <see cref="IMediaDataManager.addMediaData"/> method. 
		/// Calling the initializer elsewhere may corrupt the data model.
		/// </summary>
		/// <param name="mngr">The <see cref="IMediaDataManager"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="mngr"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="IMediaDataManager"/>
		/// </exception>
		void setMediaDataManager(IMediaDataManager mngr);

		/// <summary>
		/// Gets the UID of <c>this</c> within the .
		/// Convenience for <c><see cref="getMediaDataManager"/>().<see cref="IMediaDataManager.getUidOfMediaData"/>(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		string getUid();
		
		/// <summary>
		/// Gets the name of <c>this</c>
		/// </summary>
		/// <returns></returns>
		string getName();
		
		/// <summary>
		/// Sets the name of <c>this</c>
		/// </summary>
		/// <param name="newName">The new name - must not be <c>null</c></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newName"/> is <c>null</c>
		/// </exception>
		void setName(string newName);

		/// <summary>
		/// Deletes <c>this</c>, detaching from the owning manager
		/// </summary>
		void delete();

		/// <summary>
		/// Copies <c>this</c> including any underlying <see cref="IDataProvider"/>s
		/// </summary>
		/// <returns></returns>
		IMediaData copy();

		/// <summary>
		/// Gets a list of the <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		List<IDataProvider> getListOfUsedDataProviders();
	}
}
