using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public interface IMediaAssetLocation : IMediaLocation
	{
		IMediaData getMediaAsset();

		void setMediaAsset(IMediaData newAsset);
	}
}
