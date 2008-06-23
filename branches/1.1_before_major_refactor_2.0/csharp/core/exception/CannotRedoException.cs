using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an operation cannot be redone.
    /// </summary>
    public class CannotRedoException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public CannotRedoException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public CannotRedoException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}