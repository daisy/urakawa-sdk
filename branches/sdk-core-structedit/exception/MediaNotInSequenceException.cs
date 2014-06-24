using System;
using urakawa.media;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when an <see cref="Media"/> is not part of an <see cref="urakawa.media.SequenceMedia"/>
    /// </summary>
    public class MediaNotInSequenceException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public MediaNotInSequenceException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public MediaNotInSequenceException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}