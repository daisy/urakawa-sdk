using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events.media
{
    /// <summary>
    /// Arguments of the <see cref="AbstractTextMedia.TextChanged"/> event
    /// </summary>
    public class TextChangedEventArgs : MediaEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Media"/> of the event
        /// and the new+previous text values
        /// </summary>
        /// <param name="src">The source <see cref="Media"/> of the event</param>
        /// <param name="newTxt">The new text value</param>
        /// <param name="prevTxt">The text value prior to the change</param>
        public TextChangedEventArgs(AbstractTextMedia src, string newTxt, string prevTxt)
            : base(src)
        {
            SourceTextMedia = src;
            NewText = newTxt;
            PreviousText = prevTxt;
        }

        /// <summary>
        /// The source <see cref="Media"/> of the event
        /// </summary>
        public readonly AbstractTextMedia SourceTextMedia;

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