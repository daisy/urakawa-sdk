using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.asset
{
	/// <summary>
	/// Factory creating <see cref="IMediaAsset"/>s
	/// </summary>
	public interface IMediaAssetFactory
	{
		/// <summary>
		/// Gets the <see cref="IMediaAssetPresentation"/> asociated <c>this</c>
		/// </summary>
		/// <returns>T<he <see cref="IMediaAssetPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaAssetPresentation"/> has yet been associated with <c>this</c>
		/// </exception>
		IMediaAssetPresentation getPresentation();
		void setPresentation(IMediaAssetPresentation pres);

		IMediaAssetManager getMediaAssetManager();

		IMediaAsset createMediaAsset(string xukLocalName, string xukNamespaceUri);

		IMediaAsset createMediaAsset(Type mediaType);

		
	}
}
