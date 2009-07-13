using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments of the <see cref="UndoRedoManager.TransactionCancelled"/> command
    /// </summary>
    public class TransactionCancelledEventArgs : UndoRedoManagerEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/>
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/></param>
        public TransactionCancelledEventArgs(UndoRedoManager source, CompositeCommand command)
            : base(source, command)
        {
        }
    }
}