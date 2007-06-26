using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.undo
{
    /// <summary>
    /// A "mega-command" made of a series of "smaller" commands. Useful for merging small commands into one such as:
    /// user typing text letter by letter (the undo/redo would work on full word or sentence, not for each character.)
    /// </summary>
    public class CompositeCommand : ICommand
    {
        private List<ICommand> mCommands;
        private string mUndoString;
        private string mRedoString;

        /// <summary>
        /// Create an empty composite command.
        /// </summary>
        /// <param name="undoString">Human-readable name for the unexecute command.</param>
        /// <param name="redoString">Human-readable name for the (re)execute command.</param>
        public CompositeCommand(string undoString, string redoString)
        {
            mCommands = new List<ICommand>();
            mUndoString = undoString;
            mRedoString = redoString;
        }

        /// <summary>
        /// Insert the given command as a child of this node, at the given index. Does NOT replace the existing child,
        /// but increments (+1) the indices of the all children which index >= insertIndex. If insertIndex == children.size
        /// (no following children), then the given node is appended at the end of the existing children list.
        /// </summary>
        /// <param name="command">Cannot be null.</param>
        /// <param name="index">Must be within bounds [0 .. children.size]</param>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">Thrown when the index is out of bounds.</exception>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when a null command is given.</exception>
        public void insert(ICommand command, int index)
        {
            if (command == null) throw new exception.MethodParameterIsNullException("Cannot insert a null command.");
            if (index < 0 || index > mCommands.Count)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format("Cannot insert at index {0}; expected index in range [0 .. {1}]", index, mCommands.Count));
            }
            mCommands.Insert(index, command);
        }

        #region ICommand Members

        /// <summary>
        /// Execute the reverse command by executing the reverse commands for all the contained commands.
        /// The commands are undone in reverse order.
        /// </summary>
        /// <exception cref="urakawa.undo.CannotUndoException">Thrown when the command cannot be reversed; either because
        /// the composite command is empty or one of its contained command cannot be undone. In the latter case, the original
        /// exception is passed as the inner exception of the thrown exception.</exception>
        public void unExecute()
        {
            if (mCommands.Count == 0) throw new CannotUndoException("Composite command is empty.");
            try
            {
                for (int i = mCommands.Count - 1; i >= 0; --i) mCommands[i].unExecute();
            }
            catch (CannotUndoException e)
            {
                throw new CannotUndoException("Contained command could not be undone", e);
            }
        }

        /// <summary>
        /// Return the provided undo string.
        /// </summary>
        public string getUnExecuteShortDescription()
        {
            return mUndoString;
        }

        /// <summary>
        /// Execute all contained commands in order.
        /// </summary>
        /// <exception cref="urakawa.undo.CannotRedoException">Thrown when the command cannot be executed; either because
        /// the composite command is empty or one of its contained command cannot be executed. In the latter case, the original
        /// exception is passed as the inner exception of the thrown exception.</exception>
        public void execute()
        {
            if (mCommands.Count == 0) throw new CannotRedoException("Composite command is empty.");
            try
            {
                foreach (ICommand command in mCommands) command.execute();
            }
            catch (CannotRedoException e)
            {
                throw new CannotRedoException("Contained command could not be executed.", e);
            }
        }

        /// <summary>
        /// Return the provided redo string.
        /// </summary>
        public string getExecuteShortDescription()
        {
            return mRedoString;
        }

        /// <summary>
        /// The composite command is reversible if it contains commmands, and all of its contained command are.
        /// </summary>
        public bool canUnExecute()
        {
            return mCommands.Count > 0 && mCommands.TrueForAll(delegate(ICommand c) { return c.canUnExecute(); });
        }

        #endregion
    }
}
