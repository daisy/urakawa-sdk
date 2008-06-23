using System;

namespace urakawa.exception
{
    /// <summary>
    /// Abstract class to encapsulate errors related to wrong values for method parameters.
    /// This class cannot be instanciated and should be sub-classed.
    /// The aim is to avoid situations where values that are potentially
    /// detrimental to software integrity are silently ignored, or "swallowed".
    /// </summary>
    public abstract class MethodParameterIsInvalidException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        protected MethodParameterIsInvalidException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        protected MethodParameterIsInvalidException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}