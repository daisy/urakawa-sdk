using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
    /// <summary>
    /// Arguments of the <see cref="AudioMediaData.AudioDataInserted"/> event
    /// </summary>
    public class AudioDataInsertedEventArgs : AudioMediaDataEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="AudioMediaData"/> of the event,
        /// the insertion point and the duration of the audio data that was inserted
        /// </summary>
        /// <param name="source">The source <see cref="AudioMediaData"/> of the event</param>
        /// <param name="insPoint">The insertion point at which the audio data was inserted</param>
        /// <param name="dur">The duration of the data that was inserted</param>
        public AudioDataInsertedEventArgs(AudioMediaData source, Time insPoint, Time dur)
            : base(source)
        {
            InsertPoint = insPoint.Copy();
            Duration = dur.Copy();
        }

        /// <summary>
        /// The insertion point at which the audio data was inserted
        /// </summary>
        public readonly Time InsertPoint;

        /// <summary>
        /// The duration of the data that was inserted
        /// </summary>
        public readonly Time Duration;
    }
}