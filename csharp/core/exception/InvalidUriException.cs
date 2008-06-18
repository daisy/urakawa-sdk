using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when a given <see cref="Uri"/> is invalid
    /// </summary>
    public class InvalidUriException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public InvalidUriException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public InvalidUriException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}