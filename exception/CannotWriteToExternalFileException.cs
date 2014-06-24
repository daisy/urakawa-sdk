using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an external file could not be written to
    /// </summary>
    public class CannotWriteToExternalFileException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public CannotWriteToExternalFileException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public CannotWriteToExternalFileException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}