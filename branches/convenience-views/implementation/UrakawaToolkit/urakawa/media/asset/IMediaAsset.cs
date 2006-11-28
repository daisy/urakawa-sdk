using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.asset
{
	public interface IMediaAsset
	{
		MediaType getMediaType();
		IMediaAssetManager getAssetManager();
		string getUid();
	}
}
