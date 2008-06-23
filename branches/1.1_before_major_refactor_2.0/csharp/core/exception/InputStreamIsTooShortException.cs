using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an input <see cref="System.IO.Stream"/> is shorter than excepted,
    /// that is there are too few <see cref="byte"/>s between the current <see cref="System.IO.Stream.Position"/> 
    /// and the end of the <see cref="System.IO.Stream"/> (<see cref="System.IO.Stream.Length"/>)
    /// </summary>
    public class InputStreamIsTooShortException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public InputStreamIsTooShortException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public InputStreamIsTooShortException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}