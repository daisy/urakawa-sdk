using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.asset
{
	public interface IMediaAssetLocation : IMediaLocation
	{
		IMediaAsset getMediaAsset();

		void setMediaAsset(IMediaAsset newAsset);
	}
}
