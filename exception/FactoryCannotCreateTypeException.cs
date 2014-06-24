using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when a factory unexpectedly can not create an object of the desired type
    /// </summary>
    public class FactoryCannotCreateTypeException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public FactoryCannotCreateTypeException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public FactoryCannotCreateTypeException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}