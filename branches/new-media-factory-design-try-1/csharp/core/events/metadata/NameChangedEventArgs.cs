using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
    /// <summary>
    /// Arguments of the <see cref="Metadata.NameChanged"/> event
    /// </summary>
    public class NameChangedEventArgs : MetadataEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Metadata"/> of the event
        /// and the new+previous name
        /// </summary>
        /// <param name="source">The source <see cref="Metadata"/> of the event</param>
        /// <param name="newNM">The new name</param>
        /// <param name="prevName">The name prior to the change</param>
        public NameChangedEventArgs(Metadata source, string newNM, string prevName)
            : base(source)
        {
            NewName = newNM;
            PreviousName = prevName;
        }

        /// <summary>
        /// The new name
        /// </summary>
        public readonly string NewName;

        /// <summary>
        /// The name prior to the change
        /// </summary>
        public readonly string PreviousName;
    }
}