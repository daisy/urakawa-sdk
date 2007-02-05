using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.data
{

	/// <summary>
	/// Manager for <see cref="IMediaData"/>. 
	/// </summary>
	public interface IMediaDataManager : xuk.IXukAble
	{
		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has been associated with <c>this</c>
		/// </exception>
		IMediaDataPresentation getPresentation();

		/// <summary>
		/// Associates a <see cref="IMediaDataPresentation"/> with <c>this</c>
		/// </summary>
		/// <param name="pres">The <see cref="IMediaDataPresentation"/> with which to associate <c>this</c></param>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="IMediaDataPresentation"/></exception>
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
		/// Convenience for <c><see cref="getPresentation"/>().<see cref="IMediaDataPresentation.getDataProviderFactory"/>()</c>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		IDataProviderFactory getDataProviderFactory();

		/// <summary>
		/// Gets the <see cref="IMediaData"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The <see cref="IMediaData"/> with the given UID</returns>
		IMediaData getMediaData(string uid);

		/// <summary>
		/// Gets the UID of a given <see cref="IMediaData"/>
		/// </summary>
		/// <param name="data">The given <see cref="IMediaData"/></param>
		/// <returns>The UID of <see cref="IMediaData"/> <paramref name="data"/></returns>
		string getUidOfMediaData(IMediaData data);

		/// <summary>
		/// Adds a <see cref="IMediaData"/> to the <see cref="IMediaDataManager"/>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to add</param>
		void addMediaData(IMediaData data);

		/// <summary>
		/// Removes a <see cref="IMediaData"/> from the <see cref="IMediaDataManager"/>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to remove</param>
		void removeMediaData(IMediaData data);

		/// <summary>
		/// Removes a <see cref="IMediaData"/> from the <see cref="IMediaDataManager"/>
		/// </summary>
		/// <param name="uid">The UID of the <see cref="IMediaData"/> to remove</param>
		/// <returns>The removed <see cref="IMediaData"/></returns>
		IMediaData removeMediaData(string uid);

		/// <summary>
		/// Deletes a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="data"></param>
		void deleteMediaData(IMediaData data);
		void deleteMediaData(string uid);
		IMediaData copyMediaData(IMediaData data);
		IMediaData copyMediaData(string uid);

	}
}
