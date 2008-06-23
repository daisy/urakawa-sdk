using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when a factory is missing
    /// </summary>
    public class FactoryIsMissingException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public FactoryIsMissingException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public FactoryIsMissingException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}