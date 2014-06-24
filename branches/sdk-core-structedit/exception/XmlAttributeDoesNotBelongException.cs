using System;

namespace urakawa.exception
{
    /// <summary>
    /// Thrown when trying to remove a <see cref="urakawa.property.xml.XmlAttribute"/> instance 
    /// that does not belong to the <see cref="urakawa.property.xml.XmlProperty"/>
    /// </summary>
    public class XmlAttributeDoesNotBelongException : CheckedException
    {
        /// <summary>
        /// Constructor setting the message of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        public XmlAttributeDoesNotBelongException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Constructor setting the message and inner <see cref="Exception"/> of the exception
        /// </summary>
        /// <param localName="msg">The message</param>
        /// <param localName="inner">The inner exception</param>
        public XmlAttributeDoesNotBelongException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}