using System;

namespace urakawa.media
{
    /// <summary>
    /// Interface for <see cref="Media"/> of textual type. 
    /// </summary>
    public abstract class AbstractTextMedia : Media
    {
        /// <summary>
        /// Event fired after the text of the <see cref="ExternalTextMedia"/> has changed
        /// </summary>
        public event EventHandler<urakawa.events.media.TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Fires the <see cref="TextChanged"/> event
        /// </summary>
        /// <param name="newText">The new text value</param>
        /// <param name="prevText">Thye text value prior to the change</param>
        protected void NotifyTextChanged(string newText, string prevText)
        {
            EventHandler<urakawa.events.media.TextChangedEventArgs> d = TextChanged;
            if (d != null) d(this, new urakawa.events.media.TextChangedEventArgs(this, newText, prevText));
        }


        private void this_TextChanged(object sender, urakawa.events.media.TextChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="AbstractTextMedia"/>s should only be created via. the <see cref="MediaFactory"/>
        /// </summary>
        protected AbstractTextMedia()
        {
            TextChanged += this_TextChanged;
        }

        /// <summary>
        /// Gets or setsthe text string for the TextMedia.
        /// </summary>
        public abstract string Text { get; set; }
    }
}