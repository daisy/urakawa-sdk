using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events.media.data
{
    /// <summary>
    /// Base class for arguments of <see cref="MediaData"/> sourced events
    /// </summary>
    public class MediaDataEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="MediaData"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="MediaData"/> of the event</param>
        public MediaDataEventArgs(MediaData source)
            : base(source)
        {
            SourceMediaData = source;
        }

        /// <summary>
        /// The source <see cref="MediaData"/> of the event
        /// </summary>
        public readonly MediaData SourceMediaData;
    }
}