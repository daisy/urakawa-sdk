using System;

namespace urakawa.media
{
    /// <summary>
    /// Interface for <see cref="IMedia"/> of textual type. 
    /// </summary>
    public interface ITextMedia : IMedia
    {
        /// <summary>
        /// Event fired after the text of the <see cref="ITextMedia"/> has changed
        /// </summary>
        event EventHandler<urakawa.events.media.TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Get the text string for the TextMedia.
        /// </summary>
        /// <returns></returns>
        string Text { get; set; }
    }
}