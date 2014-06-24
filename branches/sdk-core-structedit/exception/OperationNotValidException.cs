using System;

namespace urakawa.exception
{
    /// <summary>
    /// (from the design docs)
    /// This exception should be thrown/raised when trying to
    /// call an operation (aka class method) on an object that does not
    /// allow a specific modification of the state in the current context.
    /// ...
    /// Wherever a "canDoXXX()" method can be found, the corresponding operation "doXXX()"
    /// should use this exception/error to let the user-agent of the API/Toolkit
    /// know about the non-permitted operation for which there was an attempt to execute.
    /// </summary>
    public class OperationNotValidException : UncheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public OperationNotValidException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public OperationNotValidException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}