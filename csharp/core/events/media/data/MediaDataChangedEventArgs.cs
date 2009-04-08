using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events.media.data
{
    /// <summary>
    /// Arguments of the <see cref="IManagedMedia.mediaDataChanged"/> event
    /// </summary>
	public class MediaDataChangedEventArgs : MediaEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="IManagedMedia"/>, 
        /// the new <see cref="MediaData"/> and the <see cref="MediaData"/> prior to the change
        /// </summary>
        /// <param name="source">The source <see cref="IManagedMedia"/></param>
        /// <param name="newMD">The new <see cref="MediaData"/></param>
        /// <param name="prevMD">The <see cref="MediaData"/> prior to the change</param>
		public MediaDataChangedEventArgs(IManagedMedia source, MediaData newMD, MediaData prevMD)
			: base(source)
		{
			SourceManagedMedia = source;
			NewMediaData = newMD;
			PreviousMediaData = prevMD;
		}
        /// <summary>
        /// The source <see cref="IManagedMedia"/>
        /// </summary>
		public readonly IManagedMedia SourceManagedMedia;
        /// <summary>
        /// The new <see cref="MediaData"/>
        /// </summary>
		public readonly MediaData NewMediaData;
        /// <summary>
        /// The <see cref="MediaData"/> prior to the change
        /// </summary>
		public readonly MediaData PreviousMediaData;
	}
}
