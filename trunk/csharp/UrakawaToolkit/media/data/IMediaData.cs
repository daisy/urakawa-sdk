using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.media.data
{
	public interface IMediaData : xuk.IXukAble
	{
		IMediaDataManager getDataManager();
		string getUid();
		string getName();
		void setName(string newName);
		IMediaData copy();
	}
}
