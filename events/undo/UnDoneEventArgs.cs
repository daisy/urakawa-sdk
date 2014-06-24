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
        /// Constructor setting the source <see cref="UndoRedoManager"/> and <see cref="Command"/> that was un-done
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/> of the event</param>
        /// <param name="unDoneCmd">The <see cref="Command"/> that was un-done</param>
        public UnDoneEventArgs(UndoRedoManager source, Command unDoneCmd)
            : base(source, unDoneCmd)
        {
        }
    }
}