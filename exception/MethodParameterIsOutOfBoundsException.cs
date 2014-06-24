using System;

namespace urakawa.exception
{
    /// <summary>
    /// Some methods have parameters of numeric type (float, int, uint, etc.).
    /// This exception should be thrown when values are out of allowed bounds.
    /// </summary>
    public class MethodParameterIsOutOfBoundsException : MethodParameterIsInvalidException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public MethodParameterIsOutOfBoundsException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public MethodParameterIsOutOfBoundsException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}