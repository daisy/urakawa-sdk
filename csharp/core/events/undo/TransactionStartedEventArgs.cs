using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments of the <see cref="UndoRedoManager.TransactionStarted"/> event
    /// </summary>
    public class TransactionStartedEventArgs : UndoRedoManagerEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/> of the event</param>
        public TransactionStartedEventArgs(UndoRedoManager source) : base(source)
        {
        }
    }
}