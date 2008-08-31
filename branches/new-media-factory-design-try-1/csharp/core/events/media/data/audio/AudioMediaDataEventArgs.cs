using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
    /// <summary>
    /// Base class for arguments of <see cref="AudioMediaData"/> sourced events
    /// </summary>
    public class AudioMediaDataEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="AudioMediaData"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="AudioMediaData"/> of the event</param>
        public AudioMediaDataEventArgs(AudioMediaData source)
            : base(source)
        {
            SourceAudioMediaData = source;
        }

        /// <summary>
        /// The source <see cref="AudioMediaData"/> of the event
        /// </summary>
        public readonly AudioMediaData SourceAudioMediaData;
    }
}