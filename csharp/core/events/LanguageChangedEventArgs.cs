using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
    /// <summary>
    /// Arguments of various <c>LanguageChanged</c>
    /// </summary>
    public class LanguageChangedEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Object"/> of the event
        /// and the previous+new language
        /// </summary>
        /// <param name="src">The source <see cref="Object"/> of the event</param>
        /// <param name="newLang">The new language</param>
        /// <param name="prevLanguage">The language prior to the change</param>
        public LanguageChangedEventArgs(Object src, string newLang, string prevLanguage)
            : base(src)
        {
            Newlanguage = newLang;
            PreviousLanguage = prevLanguage;
        }

        /// <summary>
        /// The new language
        /// </summary>
        public readonly string Newlanguage;

        /// <summary>
        /// The language prior to the change
        /// </summary>
        public readonly string PreviousLanguage;
    }
}