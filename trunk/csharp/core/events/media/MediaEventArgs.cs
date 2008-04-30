using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media;

namespace urakawa.events.media
{
    /// <summary>
    /// Base class for arguments for <see cref="IMedia"/> related events
    /// </summary>
	public class MediaEventArgs : DataModelChangedEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="IMedia"/>
        /// </summary>
        /// <param name="src">The source</param>
		public MediaEventArgs(IMedia src)
			: base(src)
		{
			SourceMedia = src;
		}
        /// <summary>
        /// The source <see cref="IMedia"/>
        /// </summary>
		public readonly IMedia SourceMedia;
	}
}
