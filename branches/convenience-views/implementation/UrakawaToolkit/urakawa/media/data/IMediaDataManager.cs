using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.asset
{
	public interface IMediaAssetManager : xuk.IXukAble
	{
		IMediaAssetPresentation getPresentation();
		void setPresentation(IMediaAssetPresentation pres);

		IMediaAssetFactory getMediaAssetFactory();

		IMediaAsset getAsset(string uid);
		string getUidOfAsset(IMediaAsset asset);

		void addAsset(IMediaAsset asset);
		void removeAsset(IMediaAsset asset);
		IMediaAsset removeAsset(string uid);
		void deleteAsset(IMediaAsset asset);
		IMediaAsset copyAsset(IMediaAsset asset);
		IMediaAsset copyAsset(string uid);

	}
}
