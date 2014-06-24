using System;

namespace urakawa.exception
{
    /// <summary>
    /// This exception should be raised when trying to use a MediaType that is not legal in the current context.
    /// </summary>
    public class MediaNotAcceptable : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public MediaNotAcceptable(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public MediaNotAcceptable(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}