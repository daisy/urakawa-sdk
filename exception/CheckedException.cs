using System;
using System.Diagnostics;

namespace urakawa.exception
{
    /// <summary>
    /// Summary description for CheckedException.
    /// Exceptions of this type must be caught.
    /// </summary>
    public class CheckedException : Exception
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public CheckedException(string msg) : base(msg)
        {
#if DEBUG
            Debugger.Break();
#endif
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public CheckedException(string msg, Exception inner) : base(msg, inner)
        {
#if DEBUG
            Debugger.Break();
#endif
        }
    }
}