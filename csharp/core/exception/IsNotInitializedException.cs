using System;

namespace urakawa.exception
{
    /// <summary>
    /// Exception thrown when an object is being used before it has been initialized
    /// </summary>
    public class IsNotInitializedException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public IsNotInitializedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public IsNotInitializedException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}