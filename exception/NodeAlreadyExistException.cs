using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when a previous matching node exists in a child collection, where nodes are supposed to be exclusive
    /// </summary>
    public class NodeAlreadyExistException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public NodeAlreadyExistException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public NodeAlreadyExistException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}