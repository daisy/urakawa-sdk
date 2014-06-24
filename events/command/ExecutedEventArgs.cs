using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.events.command;

namespace urakawa.events.command
{
    /// <summary>
    /// Arguments for the <see cref="Command.Executed"/> event
    /// </summary>
    public class ExecutedEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Command"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="Command"/> of the event</param>
        public ExecutedEventArgs(Command source) : base(source)
        {
        }
    }
}