using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.media
{
    /// <summary>
    /// Arguments for event occuring when the <c>src</c> of a <see cref="urakawa.media.ExternalMedia"/> has changed
    /// </summary>
	public class SrcChangedEventArgs : MediaEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="urakawa.media.ExternalMedia"/> and previous+new <c>src</c> values
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="newSrcVal">The new src value</param>
        /// <param name="prevSrcVal">The previous src value</param>
		public SrcChangedEventArgs(urakawa.media.ExternalMedia source, string newSrcVal, string prevSrcVal)
			: base(source)
		{
			SourceExternalMedia = source;
			NewSrc = newSrcVal;
			PreviousSrc = prevSrcVal;
		}
        /// <summary>
        /// The source media
        /// </summary>
		public readonly urakawa.media.ExternalMedia SourceExternalMedia;
        /// <summary>
        /// The new value of <c>src</c>
        /// </summary>
		public readonly string NewSrc;
        /// <summary>
        /// The value of <c>src</c> before the change
        /// </summary>
		public readonly string PreviousSrc;
	}
}
