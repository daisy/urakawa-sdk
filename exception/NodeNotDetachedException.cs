using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when trying to insert a node that is not detached
    /// </summary>
    public class NodeNotDetachedException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public NodeNotDetachedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public NodeNotDetachedException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}