using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.xml;

namespace urakawa.events.property.xml
{
    /// <summary>
    /// Arguments of the <see cref="XmlProperty.XmlAttributeSet"/> event
    /// </summary>
    public class XmlAttributeSetEventArgs : XmlPropertyEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="XmlProperty"/> of the event,
        /// the QName of the attribute and the previous+new value
        /// </summary>
        /// <param name="src">The source <see cref="XmlProperty"/> of the event</param>
        /// <param name="attrLN">The local name part of the OName of the attribute that was set</param>
        /// <param name="attrNS">The namespace uri part of the OName of the attribute that was set</param>
        /// <param name="newVal">The new value of the attribute</param>
        /// <param name="prevVal">The value of the attribute prior to being set</param>
        public XmlAttributeSetEventArgs(XmlProperty src, string attrLN, string attrNS, string newVal, string prevVal)
            : base(src)
        {
            AttributeLocalName = attrLN;
            AttributeNamespaceUri = attrNS;
            NewValue = newVal;
            PreviousValue = prevVal;
        }

        /// <summary>
        /// The local name part of the OName of the attribute that was set
        /// </summary>
        public readonly string AttributeLocalName;

        /// <summary>
        /// The namespace uri part of the OName of the attribute that was set
        /// </summary>
        public readonly string AttributeNamespaceUri;

        /// <summary>
        /// The new value of the attribute
        /// </summary>
        public readonly string NewValue;

        /// <summary>
        /// The value of the attribute prior to being set
        /// </summary>
        public readonly string PreviousValue;
    }
}