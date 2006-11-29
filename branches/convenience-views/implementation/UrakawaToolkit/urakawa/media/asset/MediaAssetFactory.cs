using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.asset
{
	public class MediaAssetFactory : IMediaAssetFactory
	{
		#region IMediaAssetFactory Members

		public IMediaAssetPresentation getPresentation()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setPresentation(IMediaAssetPresentation pres)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaAssetManager getMediaAssetManager()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaAsset createMediaAsset(string xukLocalName, string xukNamespaceUri)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaAsset createMediaAsset(Type mediaType)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
