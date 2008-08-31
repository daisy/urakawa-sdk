using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when errors occur during XukIn/Out
    /// </summary>
    public class XukException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public XukException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public XukException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}