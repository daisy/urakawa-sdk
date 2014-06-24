using System;
using urakawa.data;
using urakawa.media.data;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to open a second output <see cref="System.IO.Stream"/> 
    /// from a <see cref="DataProvider"/>
    /// </summary>
    public class OutputStreamOpenException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public OutputStreamOpenException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public OutputStreamOpenException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}