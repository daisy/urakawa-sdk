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
		/// <returns>T<he <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has yet been associated with <c>this</c>
		/// </exception>
		IMediaDataPresentation getPresentation();
		void setPresentation(IMediaDataPresentation pres);

		IMediaDataManager getMediaDataManager();

		IMediaData createMediaData(string xukLocalName, string xukNamespaceUri);

		IMediaData createMediaData(Type mediaType);

		
	}
}
