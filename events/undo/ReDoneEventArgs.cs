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
        /// Constructor setting the source <see cref="UndoRedoManager"/> and the <see cref="Command"/> that was re-done
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/></param>
        /// <param name="reDoneCmd">The <see cref="Command"/> that was re-done</param>
        public ReDoneEventArgs(UndoRedoManager source, Command reDoneCmd)
            : base(source, reDoneCmd)
        {
        }
    }
}