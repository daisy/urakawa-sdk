using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.undo
{
    /// <summary>
    /// Classes realizing this interface must store the state of the object(s) affected by the command
    /// execution (including undo/redo). Implementations may choose various techniques suitable in terms
    /// of performance and memory usage (storage of the transition or the full object snapshot.)
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute the reverse command.
        /// </summary>
        /// <exception cref="urakawa.undo.CannotUndoException">Thrown when the command cannot be reversed.</exception>
        void unExecute();

        /// <summary>
        /// Get a human-readable name for the reverse command.
        /// </summary>
        /// <exception cref="urakawa.undo.CannotUndoException">Thrown when the command cannot be reversed.</exception>
        string getUnExecuteShortDescription();

        /// <summary>
        /// Execute the command.
        /// </summary>
        void execute();

        /// <summary>
        /// Get a human-readable name for the command.
        /// </summary>
        string getExecuteShortDescription();
        
        /// <summary>
        /// True if the command is reversible.
        /// </summary>
        bool canUnExecute();
    }
}
