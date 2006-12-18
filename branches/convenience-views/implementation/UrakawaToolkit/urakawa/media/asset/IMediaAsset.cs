using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.asset
{
	public interface IMediaAsset : xuk.IXukAble
	{
		IMediaAssetManager getAssetManager();
		string getUid();
		string getName();
		void setName(string newName);

		IDataProvider getDataProvider();
	}
}
