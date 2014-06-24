using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
    /// <summary>
    /// Arguments of the <see cref="AudioMediaData.AudioDataRemoved"/> event
    /// </summary>
    public class AudioDataRemovedEventArgs : AudioMediaDataEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="AudioMediaData"/> of the event,
        /// the point from which the audio data was removed and the duration of the audio data that was removed
        /// </summary>
        /// <param name="source">The source <see cref="AudioMediaData"/> of the event</param>
        /// <param name="fromPoint">The point from which the audio data was removed</param>
        /// <param name="dur">The duration of the audio data that was removed</param>
        public AudioDataRemovedEventArgs(AudioMediaData source, Time fromPoint, Time dur) : base(source)
        {
            RemovedFromPoint = fromPoint.Copy();
            Duration = dur.Copy();
        }

        /// <summary>
        /// The point from which the audio data was removed
        /// </summary>
        public readonly Time RemovedFromPoint;

        /// <summary>
        /// The duration of the audio data that was removed
        /// </summary>
        public readonly Time Duration;
    }
}