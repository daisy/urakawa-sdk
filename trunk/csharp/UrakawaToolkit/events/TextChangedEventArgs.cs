using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events
{
	public class TextChangedEventArgs : MediaEventArgs
	{
		public TextChangedEventArgs(ITextMedia src, string newTxt, string prevTxt)
			: base(src)
		{
			SourceTextMedia = src;
			NewText = newTxt;
			PreviousText = prevTxt;
		}
		public readonly ITextMedia SourceTextMedia;
		public readonly string NewText;
		public readonly string PreviousText;
	}
}
