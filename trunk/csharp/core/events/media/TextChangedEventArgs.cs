using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events.media
{
    /// <summary>
    /// Arguments of the <see cref="ITextMedia.TextChanged"/> event
    /// </summary>
    public class TextChangedEventArgs : MediaEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="IMedia"/> of the event
        /// and the new+previous text values
        /// </summary>
        /// <param name="src">The source <see cref="IMedia"/> of the event</param>
        /// <param name="newTxt">The new text value</param>
        /// <param name="prevTxt">The text value prior to the change</param>
        public TextChangedEventArgs(ITextMedia src, string newTxt, string prevTxt)
            : base(src)
        {
            SourceTextMedia = src;
            NewText = newTxt;
            PreviousText = prevTxt;
        }

        /// <summary>
        /// The source <see cref="IMedia"/> of the event
        /// </summary>
        public readonly ITextMedia SourceTextMedia;

        /// <summary>
        /// The new text value
        /// </summary>
        public readonly string NewText;

        /// <summary>
        /// The text value prior to the change
        /// </summary>
        public readonly string PreviousText;
    }
}