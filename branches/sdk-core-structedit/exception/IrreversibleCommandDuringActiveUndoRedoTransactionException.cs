using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to execute an irreversible command while an undo/redo transaction is active
    /// </summary>
    public class IrreversibleCommandDuringActiveUndoRedoTransactionException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public IrreversibleCommandDuringActiveUndoRedoTransactionException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public IrreversibleCommandDuringActiveUndoRedoTransactionException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}