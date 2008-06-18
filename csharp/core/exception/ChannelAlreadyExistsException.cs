using System;

namespace urakawa.exception
{
    /// <summary>
    /// This exception should be thrown when trying to add a Channel
    /// whose localName is already used in the list of current channels.
    /// </summary>
    public class ChannelAlreadyExistsException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public ChannelAlreadyExistsException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public ChannelAlreadyExistsException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}