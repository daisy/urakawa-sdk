using System;

namespace urakawa.media
{
    //http://www.w3.org/TR/SMIL3/smil-layout.html#adef-fit
    public enum ResizeMode
    {
        Fill,
        Hidden,
        Meet,
        MeetBest,
        Scroll,
        Slice
    }

    /// <summary>
    /// Summary description for ISized.
    /// </summary>
    public interface ISized
    {
        /// <summary>
        /// Event fired after the size (height or width) of the <see cref="ISized"/> has changed
        /// </summary>
        event EventHandler<events.media.SizeChangedEventArgs> SizeChanged;

        ResizeMode ResizeMode { get; set; }

        /// <summary>
        /// Get the width of the <see cref="ISized"/> object.
        /// </summary>
        /// <returns>The width</returns>
        int Width { get; set; }

        /// <summary>
        /// Get the height of the <see cref="ISized"/> object.
        /// </summary>
        /// <returns>The height</returns>
        int Height { get; set; }

        /// <summary>
        /// Sets the size of the <see cref="ISized"/> object.
        /// </summary>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new width or height is negative
        /// </exception>
        void SetSize(int newHeight, int newWidth);
    }
}