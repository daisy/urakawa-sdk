using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when a data file used by an object unexpectedly does not exist
    /// </summary>
    public class DataMissingException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public DataMissingException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public DataMissingException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}