using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an operation is invalid because is undo/redo transaction is currently active (i.e has not ended)
    /// </summary>
    public class UndoRedoTransactionHasNotEndedException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public UndoRedoTransactionHasNotEndedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public UndoRedoTransactionHasNotEndedException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}