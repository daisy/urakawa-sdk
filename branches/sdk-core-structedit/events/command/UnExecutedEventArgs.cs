using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.events.command;

namespace urakawa.events.command
{
    /// <summary>
    /// Arguments of the <see cref="Command.UnExecuted"/> event
    /// </summary>
    public class UnExecutedEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Command"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="Command"/> of the event</param>
        public UnExecutedEventArgs(Command source) : base(source)
        {
        }
    }
}