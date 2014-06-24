using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown if a manager or factory is incompatible with another manager or factory 
    /// </summary>
    public class IncompatibleManagerOrFactoryException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public IncompatibleManagerOrFactoryException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public IncompatibleManagerOrFactoryException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}