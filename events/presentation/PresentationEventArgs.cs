using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.presentation
{
    /// <summary>
    /// Base class for arguments of <see cref="Presentation"/> sourced events
    /// </summary>
    public class PresentationEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Presentation"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="Presentation"/> of the event</param>
        public PresentationEventArgs(Presentation source)
            : base(source)
        {
            SourcePresentation = source;
        }

        /// <summary>
        /// The source <see cref="Presentation"/> of the event
        /// </summary>
        public readonly Presentation SourcePresentation;
    }
}