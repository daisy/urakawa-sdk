using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data.audio;

namespace urakawa.events.media.data.audio
{
    /// <summary>
    /// Arguments of the <see cref="AudioMediaData.PCMFormatChanged"/> event
    /// </summary>
    public class PCMFormatChangedEventArgs : AudioMediaDataEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="AudioMediaData"/> of the event
        /// and the previous+new PCMFormat
        /// </summary>
        /// <param name="source">The source <see cref="AudioMediaData"/> of the event</param>
        /// <param name="newFormat">The new PCMFormat</param>
        /// <param name="prevFormat">The PCMFormat prior to the change</param>
        public PCMFormatChangedEventArgs(AudioMediaData source, PCMFormatInfo newFormat, PCMFormatInfo prevFormat)
            : base(source)
        {
            NewPCMFormat = newFormat;
            PreviousPCMFormat = prevFormat;
        }

        /// <summary>
        /// The new PCMFormat
        /// </summary>
        public readonly PCMFormatInfo NewPCMFormat;

        /// <summary>
        /// The PCMFormat prior to the change
        /// </summary>
        public readonly PCMFormatInfo PreviousPCMFormat;
    }
}