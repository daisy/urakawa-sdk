using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.presentation
{
    /// <summary>
    /// Arguments of the <see cref="Presentation.RootUriChanged"/> event
    /// </summary>
    public class RootUriChangedEventArgs : PresentationEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Presentation"/> of the event
        /// and the new+previous root <see cref="Uri"/>
        /// </summary>
        /// <param name="source">The source <see cref="Presentation"/> of the event</param>
        /// <param name="newUriVal">The new <see cref="Uri"/></param>
        /// <param name="prevUriVal">The <see cref="Uri"/> prior to the change</param>
        public RootUriChangedEventArgs(Presentation source, Uri newUriVal, Uri prevUriVal)
            : base(source)
        {
            NewUri = newUriVal;
            PreviousUri = prevUriVal;
        }

        /// <summary>
        /// The new <see cref="Uri"/>
        /// </summary>
        public readonly Uri NewUri;

        /// <summary>
        /// The <see cref="Uri"/> prior to the change
        /// </summary>
        public readonly Uri PreviousUri;
    }
}