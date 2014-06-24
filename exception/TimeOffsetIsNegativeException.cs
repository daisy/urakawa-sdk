using System;

namespace urakawa.exception
{
    /// <summary>
    /// This exception should be raised when trying to use a time offset that is not allowed to be negative.
    /// </summary>
    public class TimeOffsetIsNegativeException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public TimeOffsetIsNegativeException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public TimeOffsetIsNegativeException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}