using System;

namespace urakawa.media
{
	/// <summary>
	/// Summary description for ITextMedia.
	/// </summary>
	public interface ITextMedia : IMedia
	{
		string getText();
		void setText(string text);
	}
}
