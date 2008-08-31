using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;
using urakawa.media.timing;

namespace urakawa.events.media
{
    /// <summary>
    /// Arguments of the <see cref="IClipped.ClipChanged"/> event
    /// </summary>
    public class ClipChangedEventArgs : MediaEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Media"/> of the event and the new+previous clip begin+end <see cref="Time"/>s
        /// </summary>
        /// <param name="source">The source <see cref="Media"/> of the event</param>
        /// <param name="newCB">The new clip begin value</param>
        /// <param name="newCE">The new clip end value</param>
        /// <param name="prevCB">The clip begin value prior to the change</param>
        /// <param name="prevCE">The clip end value prior to the change</param>
        public ClipChangedEventArgs(Media source, Time newCB, Time newCE, Time prevCB, Time prevCE)
            : base(source)
        {
            NewClipBegin = newCB;
            NewClipEnd = newCE;
            PreviousClipBegin = prevCB;
            PreviousClipEnd = prevCE;
        }

        /// <summary>
        /// The new clip begin value
        /// </summary>
        public readonly Time NewClipBegin;

        /// <summary>
        /// The new clip end value
        /// </summary>
        public readonly Time NewClipEnd;

        /// <summary>
        /// The clip begin value prior to the change
        /// </summary>
        public readonly Time PreviousClipBegin;

        /// <summary>
        /// The clip end value prior to the change
        /// </summary>
        public readonly Time PreviousClipEnd;
    }
}