using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an operation cannot be undone.
    /// </summary>
    public class CannotUndoException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public CannotUndoException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public CannotUndoException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}