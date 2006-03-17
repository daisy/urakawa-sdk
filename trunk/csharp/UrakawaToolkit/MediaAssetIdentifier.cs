using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for MediaAssetIdentifier.
	/// </summary>
	public class MediaAssetIdentifier
	{
		private string mAssetId;

		public MediaAssetIdentifier()
		{
		}

		public void setMediaAssetIdentifier(string assetId)
		{
			mAssetId = assetId;
		}

		public string getMediaAssetIdentifier()
		{
			return mAssetId;
		}
	}
}
