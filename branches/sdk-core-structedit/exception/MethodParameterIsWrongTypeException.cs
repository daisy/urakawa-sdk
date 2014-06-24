using System;

namespace urakawa.exception
{
    /// <summary>
    /// Some methods have stricter type rules than specified by the method signature.
    /// This exception should be raised when such type rules are broken.
    /// </summary>
    public class MethodParameterIsWrongTypeException : MethodParameterIsInvalidException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public MethodParameterIsWrongTypeException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public MethodParameterIsWrongTypeException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}