using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.project
{
    /// <summary>
    /// Base class for arguments of <see cref="Project"/> sourced event
    /// </summary>
    public class ProjectEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Project"/> of the evnet
        /// </summary>
        /// <param name="source">The source <see cref="Project"/> of the event</param>
        public ProjectEventArgs(Project source)
            : base(source)
        {
            SourceProject = source;
        }

        /// <summary>
        /// The source <see cref="Project"/> of the event
        /// </summary>
        public readonly Project SourceProject;
    }
}