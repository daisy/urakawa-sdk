using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments of the <see cref="UndoRedoManager.CommandUnDone"/> event
    /// </summary>
    public class UnDoneEventArgs : UndoRedoManagerEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/> and <see cref="ICommand"/> that was un-done
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/> of the event</param>
        /// <param name="unDoneCmd">The <see cref="ICommand"/> that was un-done</param>
        public UnDoneEventArgs(UndoRedoManager source, ICommand unDoneCmd) : base(source)
        {
            UnDoneCommand = unDoneCmd;
        }

        /// <summary>
        /// The <see cref="ICommand"/> that was un-done
        /// </summary>
        public readonly ICommand UnDoneCommand;
    }
}