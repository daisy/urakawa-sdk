using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for ClippedMedia.
	/// </summary>
	public class ClippedMedia : IClippedMedia
	{
		private MediaAssetIdentifier mAssetId = new MediaAssetIdentifier();
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();

		public ClippedMedia()
		{
		}

		#region IClippedMedia Members

		public TimeDelta getDuration()
		{
			return mClipBegin.getDelta(mClipEnd);
		}

		public Time getClipBegin()
		{
			return mClipBegin;
		}

		public Time getClipEnd()
		{
			return mClipEnd;
		}

		public void setClipBegin(Time beginPoint)
		{
			mClipBegin = beginPoint;
		}

		public void setClipEnd(Time endPoint)
		{
			mClipEnd = endPoint;
		}

		
		/// <summary>
		/// this function resizes this object from mBegin to splitPoint
		/// and creates a new object from splitPoint to mEnd
		/// </summary>
		/// <param name="splitPoint"></param>
		/// <returns></returns>
		public IClippedMedia split(Time splitPoint)
		{
			ClippedMedia splitMedia = new ClippedMedia();
			splitMedia.setClipBegin(splitPoint);
			splitMedia.setClipEnd(mClipEnd);

			this.setClipEnd(splitPoint);

			return splitMedia;

		}

		#endregion

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
