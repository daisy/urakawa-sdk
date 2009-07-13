using urakawa.command;
using urakawa.undo;

namespace urakawa.events.undo
{
    /// <summary>
    /// Base class for arguments of <see cref="UndoRedoManager"/> sourced events
    /// </summary>
    public class UndoRedoManagerEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="UndoRedoManager"/> of the event
        /// </summary>
        /// <param name="source">The source <see cref="UndoRedoManager"/> of the event</param>
        public UndoRedoManagerEventArgs(UndoRedoManager source, Command command)
            : base(source)
        {
            SourceUndoRedoManager = source;
            Command = command;
        }

        /// <summary>
        /// The source <see cref="UndoRedoManager"/> of the event
        /// </summary>
        public readonly UndoRedoManager SourceUndoRedoManager;

        /// <summary>
        /// The <see cref="Command"/> of the event
        /// </summary>
        public readonly Command Command;
    }
}