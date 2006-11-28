using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.asset
{
	public interface IMediaAssetManager
	{
		IMediaPresentation getPresentation();

		void addAsset(IMediaAsset asset);
		void removeAsset(IMediaAsset asset);
		IMediaAsset removeAsset(string uid);
		void deleteAsset(IMediaAsset asset);
		IMediaAsset copyAsset(IMediaAsset asset);
		IMediaAsset copyAsset(string uid);
		IMediaAsset getAsset(string uid);
		string getUidOfAsset(IMediaAsset asset);

	}
}
