using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
    /// <summary>
    /// Arguments of the <see cref="Metadata.contentChanged"/> event
    /// </summary>
	public class ContentChangedEventArgs : MetadataEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="Metadata"/> of the event
        /// and the new+previous content
        /// </summary>
        /// <param name="source">The source <see cref="Metadata"/> of the event</param>
        /// <param name="newCntnt">The new content</param>
        /// <param name="prevContent">The content prior to the change</param>
		public ContentChangedEventArgs(Metadata source, string newCntnt, string prevContent)
			: base(source)
		{
			NewContent = newCntnt;
			PreviousContent = prevContent;
		}
        /// <summary>
        /// The new content
        /// </summary>
		public readonly string NewContent;
        /// <summary>
        /// The content prior to the change
        /// </summary>
		public readonly string PreviousContent;
	}
}
