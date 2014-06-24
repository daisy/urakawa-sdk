using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.events;

namespace urakawa.events.command
{
    /// <summary>
    /// Base class for arguments of <see cref="Command"/> sourced events
    /// </summary>
    public class CommandEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Command"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="Command"/> of the event</param>
        public CommandEventArgs(Command source)
            : base(source)
        {
            SourceCommand = source;
        }

        /// <summary>
        /// The source <see cref="Command"/> of the event
        /// </summary>
        public readonly Command SourceCommand;
    }
}