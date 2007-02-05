using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Factory creating <see cref="IMediaData"/>s
	/// </summary>
	public interface IMediaDataFactory
	{
		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> asociated <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has yet been associated with <c>this</c>
		/// </exception>
		IMediaDataPresentation getPresentation();

		/// <summary>
		/// Associates a <see cref="IMediaDataPresentation"/> with <c>this</c>
		/// </summary>
		/// <param name="pres">The <see cref="IMediaDataPresentation"/></param>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="IMediaDataPresentation"/></exception>
		void setPresentation(IMediaDataPresentation pres);

		/// <summary>
		/// Gets the <see cref="IMediaDataManager"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>.
		/// This is convenience for <c>this.<see cref="getPresentation"/>().<see cref="getMediaDataManager"/>getMediaDataManager()</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataManager"/></returns>
		IMediaDataManager getMediaDataManager();

		/// <summary>
		/// Creates a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="xukLocalName"></param>
		/// <param name="xukNamespaceUri"></param>
		/// <returns></returns>
		IMediaData createMediaData(string xukLocalName, string xukNamespaceUri);

		IMediaData createMediaData(Type mediaType);

		IDataProvider createDataProvider();

		
	}
}
