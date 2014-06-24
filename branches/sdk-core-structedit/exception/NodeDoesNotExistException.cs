using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when a node does not exists in a child collection
    /// </summary>
    public class NodeDoesNotExistException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public NodeDoesNotExistException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public NodeDoesNotExistException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}