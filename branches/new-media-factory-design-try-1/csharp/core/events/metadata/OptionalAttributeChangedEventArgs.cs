using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace urakawa.events.metadata
{
    /// <summary>
    /// Arguments of the <see cref="Metadata.OptionalAttributeChanged"/> event
    /// </summary>
    public class OptionalAttributeChangedEventArgs : MetadataEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Metadata"/> of the event,
        /// the name of the optional attribute that changed
        /// and previous+new value of the optional attribute
        /// </summary>
        /// <param name="source">The source <see cref="Metadata"/> of the event</param>
        /// <param name="nm">The name of the optional attribute that changed</param>
        /// <param name="newVal">The new value of the optional attribute</param>
        /// <param name="prevValue">The value of the optional attribute prior to the change</param>
        public OptionalAttributeChangedEventArgs(Metadata source, string nm, string newVal, string prevValue)
            : base(source)
        {
            Name = nm;
            NewValue = newVal;
            PreviousValue = prevValue;
        }

        /// <summary>
        /// The name of the optional attribute that changed
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The new value of the optional attribute
        /// </summary>
        public readonly string NewValue;

        /// <summary>
        /// The value of the optional attribute prior to the change
        /// </summary>
        public readonly string PreviousValue;
    }
}