using System;
using System.Collections.Generic;
using System.Text;
using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments for the <see cref="UndoRedoManager.CommandReDone"/> event
    /// </summary>
    public class ReDoneEventArgs : UndoRedoManagerEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/> and the <see cref="ICommand"/> that was re-done
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/></param>
        /// <param name="reDoneCmd">The <see cref="ICommand"/> that was re-done</param>
        public ReDoneEventArgs(UndoRedoManager source, ICommand reDoneCmd) : base(source)
        {
            ReDoneCommand = reDoneCmd;
        }

        /// <summary>
        /// The <see cref="ICommand"/> that was re-done
        /// </summary>
        public readonly ICommand ReDoneCommand;
    }
}