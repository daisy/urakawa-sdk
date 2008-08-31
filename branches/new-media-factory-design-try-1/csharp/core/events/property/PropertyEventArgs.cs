using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property;

namespace urakawa.events.property
{
    /// <summary>
    /// Base class for arguments of <see cref="Property"/> sourced events
    /// </summary>
    public class PropertyEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Property"/> of the event
        /// </summary>
        /// <param name="src">The source <see cref="Property"/> of the event</param>
        public PropertyEventArgs(Property src)
            : base(src)
        {
            SourceProperty = src;
        }

        /// <summary>
        /// The source <see cref="Property"/> of the event
        /// </summary>
        public readonly Property SourceProperty;
    }
}