using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;
using urakawa.events;

namespace urakawa.events.undo
{
    /// <summary>
    /// Base class for arguments of <see cref="ICommand"/> sourced events
    /// </summary>
	public class CommandEventArgs : DataModelChangedEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="ICommand"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="ICommand"/> of the event</param>
		public CommandEventArgs(ICommand source)
			: base(source)
		{
			SourceCommand = source;
		}
        /// <summary>
        /// The source <see cref="ICommand"/> of the event
        /// </summary>
		public readonly ICommand SourceCommand;
	}
}
