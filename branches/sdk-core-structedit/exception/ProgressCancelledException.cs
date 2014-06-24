using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when a progress event emitting operation has been cancelled
    /// </summary>
    public class ProgressCancelledException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public ProgressCancelledException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public ProgressCancelledException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}