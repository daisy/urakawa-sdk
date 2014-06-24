using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to assign to a <see cref="urakawa.core.TreeNode"/> a <see cref="urakawa.property.Property"/> 
    /// that is already to another <see cref="urakawa.core.TreeNode"/>
    /// </summary>
    public class PropertyAlreadyHasOwnerException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public PropertyAlreadyHasOwnerException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public PropertyAlreadyHasOwnerException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}