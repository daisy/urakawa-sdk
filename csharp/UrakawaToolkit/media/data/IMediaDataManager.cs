using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;
using urakawa.xuk;

namespace urakawa.media.data
{

	/// <summary>
	/// Manager for <see cref="IMediaData"/>. 
	/// </summary>
	public interface IMediaDataManager : IXukAble, IValueEquatable<IMediaDataManager>
	{
		/// <summary>
		/// Gets the presentation associated with <c>this</c> (the owner)
		/// </summary>
		/// <returns>The associated presentation</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no presentation has been associated with <c>this</c>
		/// </exception>
		IMediaDataPresentation getPresentation();

		/// <summary>
		/// Associates a <see cref="IMediaDataPresentation"/> with <c>this</c> (as owner) - Initializer
		/// </summary>
		/// <param name="pres">The presentation with which to associate <c>this</c></param>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a presentation</exception>
		void setPresentation(IMediaDataPresentation pres);

		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>).
		/// Convenience for <c><see cref="getPresentation"/>().<see cref="IMediaDataPresentation.getMediaDataFactory"/>()</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataFactory"/></returns>
		IMediaDataFactory getMediaDataFactory();

		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>).
		/// Convenience for <c>getPresentation().getDataProviderManager().getDataProviderFactory()</c>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		IDataProviderFactory getDataProviderFactory();

		/// <summary>
		/// Gets the <see cref="IMediaData"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The <see cref="IMediaData"/> with the given UID or <c>null</c> if no such <see cref="IMediaData"/> exists</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		IMediaData getMediaData(string uid);

		/// <summary>
		/// Gets the UID of a given <see cref="IMediaData"/>
		/// </summary>
		/// <param name="data">The given <see cref="IMediaData"/></param>
		/// <returns>The UID of <see cref="IMediaData"/> <paramref name="data"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <c>this</c> is not the manager of <paramref name="data"/>
		/// </exception>
		string getUidOfMediaData(IMediaData data);

		/// <summary>
		/// Adds a <see cref="IMediaData"/> to the <see cref="IMediaDataManager"/>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to add</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		void addMediaData(IMediaData data);

		/// <summary>
		/// Gets a list of uids of all <see cref="IMediaData"/> managed by <c>this</c>
		/// </summary>
		/// <returns>The list of uids</returns>
		IList<string> getListOfUids();

		/// <summary>
		/// Detaches a <see cref="IMediaData"/> from <c>this</c>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to detach</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <paramref name="data"/> is not managed by <c>this</c>
		/// </exception>
		void detachMediaData(IMediaData data);

		/// <summary>
		/// Deletes a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="uid">The UID of the <see cref="IMediaData"/> to delete</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when no <see cref="IMediaData"/> managed by <c>this</c> has the given UID
		/// </exception>
		void deleteMediaData(string uid);

		/// <summary>
		/// Creates a copy of a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to copy</param>
		/// <returns>The copy</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		IMediaData copyMediaData(IMediaData data);

		/// <summary>
		/// Creates a copy of a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="uid">The UID of the <see cref="IMediaData"/> to copy</param>
		/// <returns>The copy</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when no <see cref="IMediaData"/> managed by <c>this</c> has the given UID
		/// </exception>
		IMediaData copyMediaData(string uid);

		/// <summary>
		/// Gets a list of all <see cref="IMediaData"/> managed by <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		IList<IMediaData> getListOfManagedMediaData();

	}
}
