using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an operation is not valid because no undo/redo transaction has been started
    /// </summary>
    public class UndoRedoTransactionIsNotStartedException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public UndoRedoTransactionIsNotStartedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public UndoRedoTransactionIsNotStartedException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}