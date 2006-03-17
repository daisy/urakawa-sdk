using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for ImageObject.
	/// </summary>
	public class ImageObject : IExtAssetMedia
	{
		private MediaAssetIdentifier mAssetId = new MediaAssetIdentifier();

		public ImageObject()
		{
		}
		#region IExtAssetMedia Members

		public MediaAssetIdentifier getAssetIdentifier()
		{
			return mAssetId;
		}

		public void setAssetIdentifier(MediaAssetIdentifier assetId)
		{
			mAssetId = assetId;
		}

		#endregion
	}
}
