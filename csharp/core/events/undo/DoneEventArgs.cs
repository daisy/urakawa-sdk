using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Arguments for the <see cref="UndoRedoManager.CommandDone"/> event
    /// </summary>
    public class DoneEventArgs : UndoRedoManagerEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/> and the <see cref="Command"/> that was done
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/> of the event</param>
        /// <param name="doneCmd">The <see cref="Command"/> that was done</param>
        public DoneEventArgs(UndoRedoManager source, Command doneCmd)
            : base(source, doneCmd)
        {
        }
    }
}