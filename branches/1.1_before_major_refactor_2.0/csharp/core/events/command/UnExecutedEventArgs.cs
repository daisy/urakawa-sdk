using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.events.command;

namespace urakawa.events.command
{
    /// <summary>
    /// Arguments of the <see cref="ICommand.unExecuted"/> event
    /// </summary>
    public class UnExecutedEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="ICommand"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="ICommand"/> of the event</param>
        public UnExecutedEventArgs(ICommand source) : base(source) { }
    }
}