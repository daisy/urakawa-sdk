using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.asset
{
	public interface IMediaAssetPresentation : IMediaPresentation
	{
		IMediaAssetManager getMediaAssetManager();

		IMediaAssetFactory getMediaAssetFactory();
	}
}
