using System;

namespace urakawa.exception
{
    /// <summary>
    /// This exception should be thrown when trying to remove a Channel
    /// whose localName does not exist in the list of current channels.
    /// </summary>
    public class ChannelDoesNotExistException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        public ChannelDoesNotExistException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="inner">The inner exception</param>
        public ChannelDoesNotExistException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}