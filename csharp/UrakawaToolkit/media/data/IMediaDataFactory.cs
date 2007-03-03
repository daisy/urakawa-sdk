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
		/// Gets the <see cref="IMediaDataPresentation"/> asociated <c>this</c>.
		/// Convenience for <c>this.getMediaDataManager().getPresentation()</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has yet been associated with <c>this</c>
		/// </exception>
		IMediaDataPresentation getPresentation();

		/// <summary>
		/// Gets the <see cref="IMediaDataManager"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>.
		/// This is convenience for <c>this.<see cref="getPresentation"/>().<see cref="getMediaDataManager"/>getMediaDataManager()</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataManager"/></returns>
		IMediaDataManager getMediaDataManager();

		/// <summary>
		/// Initializer associating the <c>this</c> with an owning media data manager
		/// </summary>
		/// <param name="mngr">The manager</param>
		void setMediaDataManager(IMediaDataManager mngr);

		/// <summary>
		/// Creates a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="xukLocalName"></param>
		/// <param name="xukNamespaceUri"></param>
		/// <returns></returns>
		IMediaData createMediaData(string xukLocalName, string xukNamespaceUri);

		/// <summary>
		/// Creates a <see cref="IMediaData"/> a given type
		/// </summary>
		/// <param name="mediaType">The given type</param>
		/// <returns>The created media data</returns>
		IMediaData createMediaData(Type mediaType);

		
	}
}
