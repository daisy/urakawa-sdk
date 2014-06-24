using System;
using urakawa.command;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when a <see cref="Command"/> cannot execute
    /// </summary>
    public class CannotExecuteException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public CannotExecuteException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public CannotExecuteException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}