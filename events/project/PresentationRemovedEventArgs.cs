using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.project
{
    /// <summary>
    /// Arguments of the <see cref="Project.PresentationRemoved"/> event
    /// </summary>
    public class PresentationRemovedEventArgs : ProjectEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Project"/> of the event
        /// and the removed <see cref="Presentation"/>
        /// </summary>
        /// <param name="source">The source <see cref="Project"/> of the event</param>
        /// <param name="removee">The removed <see cref="Presentation"/></param>
        public PresentationRemovedEventArgs(Project source, Presentation removee)
            : base(source)
        {
            RemovedPresentation = removee;
        }

        /// <summary>
        /// The removed <see cref="Presentation"/>
        /// </summary>
        public readonly Presentation RemovedPresentation;
    }
}