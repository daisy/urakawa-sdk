using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for IExtAssetMedia.
	/// </summary>
	public interface IExtAssetMedia : IMediaObject
	{
		MediaAssetIdentifier getAssetIdentifier();
		void setAssetIdentifier(MediaAssetIdentifier assetId);
	}
}
