using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.project
{
    /// <summary>
    /// Arguments of the <see cref="Project.PresentationAdded"/> event
    /// </summary>
    public class PresentationAddedEventArgs : ProjectEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Project"/> of the event
        /// and the added <see cref="Presentation"/>
        /// </summary>
        /// <param name="source">The source <see cref="Project"/> of the event</param>
        /// <param name="addee">The added <see cref="Presentation"/></param>
        public PresentationAddedEventArgs(Project source, Presentation addee)
            : base(source)
        {
            AddedPresentation = addee;
        }

        /// <summary>
        /// The added <see cref="Presentation"/>
        /// </summary>
        public readonly Presentation AddedPresentation;
    }
}