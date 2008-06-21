using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments for the <see cref="UndoRedoManager.commandDone"/> event
    /// </summary>
	public class DoneEventArgs : UndoRedoManagerEventArgs
	{
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/> and the <see cref="ICommand"/> that was done
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/> of the event</param>
        /// <param name="doneCmd">The <see cref="ICommand"/> that was done</param>
		public DoneEventArgs(UndoRedoManager source, ICommand doneCmd)
			: base(source)
		{
			DoneCommand = doneCmd;
		}
        /// <summary>
        /// The <see cref="ICommand"/> that was done
        /// </summary>
		public readonly ICommand DoneCommand;
	}
}
