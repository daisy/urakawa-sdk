using System;
using urakawa.media.timing;

namespace urakawa.media
{
    /// <summary>
    /// This interface is for referring to time-based segments of external media
    /// </summary>
    public interface IClipped : IContinuous
    {
        /// <summary>
        /// Event fired after the clip (clip begin or clip end) of the <see cref="IClipped"/> has changed
        /// </summary>
        event EventHandler<events.media.ClipChangedEventArgs> ClipChanged;


        /// <summary>
        /// Get the begin <see cref="Time"/> for the clip.
        /// </summary>
        /// <returns>The begin <see cref="Time"/></returns>
        Time ClipBegin { get; set; }

        /// <summary>
        /// Get the end <see cref="Time"/> for the clip.
        /// </summary>
        /// <returns>The end <see cref="Time"/></returns>
        Time ClipEnd { get; set; }
    }
}