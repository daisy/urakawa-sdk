using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for TextObject.
	/// </summary>
	public class TextMedia : ITextMedia
	{
		private string mTextString;

		public TextMedia()
		{
		}

		#region ITextMedia Members

		public string getText()
		{
			return mTextString;
		}

		public void setText(string text)
		{
			mTextString = text;
		}

		#endregion

		#region IMedia Members

		public bool isContinuous()
		{
			return false;
		}

		public bool isDiscrete()
		{
			return true;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.TEXT;
		}

		#endregion
	}
}
