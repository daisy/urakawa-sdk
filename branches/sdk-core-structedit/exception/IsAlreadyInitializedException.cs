using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when trying to initialize an object that has already been initialized
    /// </summary>
    public class IsAlreadyInitializedException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public IsAlreadyInitializedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public IsAlreadyInitializedException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}