using System;
using urakawa.data;
using urakawa.media.data;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to open an output <see cref="System.IO.Stream"/>
    /// from a <see cref="DataProvider"/> 
    /// while one or more input <see cref="System.IO.Stream"/>s are open
    /// </summary>
    public class InputStreamsOpenException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public InputStreamsOpenException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public InputStreamsOpenException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}