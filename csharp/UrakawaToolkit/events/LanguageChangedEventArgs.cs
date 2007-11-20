using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class LanguageChangedEventArgs : DataModelChangeEventArgs
	{
		public LanguageChangedEventArgs(Object src, string newLang, string prevLanguage)
			: base(src)
		{
			Newlanguage = newLang;
			PreviousLanguage = prevLanguage;
		}

		public readonly string Newlanguage;
		public readonly string PreviousLanguage;
	}
}
