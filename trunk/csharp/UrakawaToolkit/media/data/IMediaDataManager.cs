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

		IMediaDataFactory getMediaDataFactory();

		IMediaData getMediaData(string uid);
		string getUidOfMediaData(IMediaData asset);

		void addMediaData(IMediaData asset);
		void removeMediaData(IMediaData asset);
		IMediaData removeMediaData(string uid);
		void deleteMediaData(IMediaData asset);
		IMediaData copyMediaData(IMediaData asset);
		IMediaData copyMediaData(string uid);

	}
}
