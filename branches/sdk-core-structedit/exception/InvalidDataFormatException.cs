using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when data does not conform to an expected data format,
    /// eg. when encountering an invalid WAVE header
    /// </summary>
    public class InvalidDataFormatException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public InvalidDataFormatException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public InvalidDataFormatException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}