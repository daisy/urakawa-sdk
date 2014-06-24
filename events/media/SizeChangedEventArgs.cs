using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events.media
{
    /// <summary>
    /// Arguments of the <see cref="ISized.SizeChanged"/> event
    /// </summary>
    public class SizeChangedEventArgs : MediaEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Media"/> of the event
        /// and the new+previous hight+width
        /// </summary>
        /// <param name="source">The source <see cref="Media"/> of the event</param>
        /// <param name="newH">The new height</param>
        /// <param name="newW">The new width</param>
        /// <param name="prevH">The height prior to the change</param>
        /// <param name="prevW">The width prior to the change</param>
        public SizeChangedEventArgs(Media source, int newH, int newW, int prevH, int prevW)
            : base(source)
        {
            NewHeight = newH;
            NewWidth = newW;
            PreviousHeight = prevH;
            PreviousWidth = prevW;
        }

        /// <summary>
        /// The new height
        /// </summary>
        public readonly int NewHeight;

        /// <summary>
        /// The new width
        /// </summary>
        public readonly int NewWidth;

        /// <summary>
        /// The height prior to the change
        /// </summary>
        public readonly int PreviousHeight;

        /// <summary>
        /// The width prior to the change
        /// </summary>
        public readonly int PreviousWidth;
    }
}