using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when a given <see cref="Object"/> is not managed by the manager
    /// </summary>
    public class IsNotManagerOfException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public IsNotManagerOfException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public IsNotManagerOfException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}