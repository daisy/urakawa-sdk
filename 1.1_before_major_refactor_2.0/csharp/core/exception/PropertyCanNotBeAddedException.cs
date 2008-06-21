using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to add a <see cref="property.Property"/> to a <see cref="core.TreeNode"/>
    /// that is can not be added to
    /// </summary>
    public class PropertyCanNotBeAddedException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public PropertyCanNotBeAddedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public PropertyCanNotBeAddedException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}