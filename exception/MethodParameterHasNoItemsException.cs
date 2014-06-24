using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when a collection/enumeration of items passed as a parameter unexpectedly contains no items
    /// </summary>
    public class MethodParameterHasNoItemsException : MethodParameterIsInvalidException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public MethodParameterHasNoItemsException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public MethodParameterHasNoItemsException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}