using System;

namespace urakawa.exception
{
    /// <summary>
    /// Some methods forbid passing empty String values.
    ///  This exception should be thrown when empty String values are passed.
    /// </summary>
    public class MethodParameterIsEmptyStringException : MethodParameterIsInvalidException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public MethodParameterIsEmptyStringException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public MethodParameterIsEmptyStringException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}