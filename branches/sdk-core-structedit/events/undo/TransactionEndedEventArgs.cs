using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments of the <see cref="UndoRedoManager.TransactionEnded"/> event
    /// </summary>
    public class TransactionEndedEventArgs : UndoRedoManagerEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/>
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/></param>
        public TransactionEndedEventArgs(UndoRedoManager source, CompositeCommand command)
            : base(source, command)
        {
        }
    }
}