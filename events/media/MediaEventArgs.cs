using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events.media
{
    /// <summary>
    /// Base class for arguments for <see cref="Media"/> related events
    /// </summary>
    public class MediaEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Media"/>
        /// </summary>
        /// <param name="src">The source</param>
        public MediaEventArgs(Media src)
            : base(src)
        {
            SourceMedia = src;
        }

        /// <summary>
        /// The source <see cref="Media"/>
        /// </summary>
        public readonly Media SourceMedia;
    }
}