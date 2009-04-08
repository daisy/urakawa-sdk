using System;

namespace urakawa.exception
{
    /// <summary>
    /// Some methods forbid passing NULL values.
    /// This exception should be raised when NULL values are passed.
    /// </summary>
    public class MethodParameterIsNullException : MethodParameterIsInvalidException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public MethodParameterIsNullException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public MethodParameterIsNullException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}