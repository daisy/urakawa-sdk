using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an external file can not be read from
    /// </summary>
    public class CannotReadFromExternalFileException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public CannotReadFromExternalFileException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public CannotReadFromExternalFileException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}