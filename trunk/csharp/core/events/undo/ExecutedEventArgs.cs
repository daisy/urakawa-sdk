using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments for the <see cref="ICommand.executed"/> event
    /// </summary>
	public class ExecutedEventArgs : CommandEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="ICommand"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="ICommand"/> of the event</param>
		public ExecutedEventArgs(ICommand source) : base(source) { }
	}
}
