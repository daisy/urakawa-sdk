using System;

namespace urakawa.exception
{
    /// <summary>
    /// This exception should be raised when trying to parse an invalid time string representation
    /// </summary>
    public class TimeStringRepresentationIsInvalidException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public TimeStringRepresentationIsInvalidException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public TimeStringRepresentationIsInvalidException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}