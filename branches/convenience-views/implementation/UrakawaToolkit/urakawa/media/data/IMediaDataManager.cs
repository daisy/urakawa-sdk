using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.data
{
	public interface IMediaDataManager : xuk.IXukAble
	{
		IMediaDataPresentation getPresentation();
		void setPresentation(IMediaDataPresentation pres);

		IMediaDataFactory getMediaAssetFactory();

		IMediaData getAsset(string uid);
		string getUidOfAsset(IMediaData asset);

		void addAsset(IMediaData asset);
		void removeAsset(IMediaData asset);
		IMediaData removeAsset(string uid);
		void deleteAsset(IMediaData asset);
		IMediaData copyAsset(IMediaData asset);
		IMediaData copyAsset(string uid);

	}
}
