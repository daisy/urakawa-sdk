using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.events.media.data
{
    /// <summary>
    /// Arguments of the <see cref="MediaData.NameChanged"/> event
    /// </summary>
    public class NameChangedEventArgs : MediaDataEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="MediaData"/> and the new+previous name of the <see cref="MediaData"/>
        /// </summary>
        /// <param name="source">The source <see cref="MediaData"/> of the event</param>
        /// <param name="newNameValue">The new name after the change</param>
        /// <param name="prevNameValue">The name prior to the change</param>
        public NameChangedEventArgs(MediaData source, string newNameValue, string prevNameValue)
            : base(source)
        {
            NewName = newNameValue;
            PreviousName = prevNameValue;
        }

        /// <summary>
        /// The new name after the change
        /// </summary>
        public readonly string NewName;

        /// <summary>
        /// The name prior to the change
        /// </summary>
        public readonly string PreviousName;
    }
}