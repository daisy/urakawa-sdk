using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to remove an <see cref="urakawa.property.xml.XmlAttribute"/>
    /// that does not exists on a <see cref="urakawa.property.xml.XmlProperty"/>
    /// </summary>
    public class XmlAttributeDoesNotExistsException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public XmlAttributeDoesNotExistsException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public XmlAttributeDoesNotExistsException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}