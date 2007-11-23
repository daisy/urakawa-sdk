using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events
{
	public class MediaDataChangedEventArgs : MediaEventArgs
	{
		public MediaDataChangedEventArgs(IManagedMedia source, MediaData newMD, MediaData prevMD)
			: base(source)
		{
			SourceManagedMedia = source;
			NewMediaData = newMD;
			PreviousMediaData = prevMD;
		}

		public readonly IManagedMedia SourceManagedMedia;
		public readonly MediaData NewMediaData;
		public readonly MediaData PreviousMediaData;
	}
}
