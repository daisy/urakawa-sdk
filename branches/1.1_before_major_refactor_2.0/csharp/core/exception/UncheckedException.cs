using System;

namespace urakawa.exception
{
    /// <summary>
    /// Unchecked exceptions do not require catching and handling
    /// </summary>
    public class UncheckedException : Exception
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public UncheckedException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public UncheckedException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}