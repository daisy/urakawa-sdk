using System;

namespace urakawa.core.media
{
	/// <summary>
	/// Summary description for TextObject.
	/// </summary>
	public class TextObject : IMediaObject
	{
		private string mTextString;

		public TextObject()
		{
		}

		public void setTextString(string textString)
		{
			mTextString = textString;
		}

		public string getTextString()
		{
			return mTextString;
		}
	}
}
