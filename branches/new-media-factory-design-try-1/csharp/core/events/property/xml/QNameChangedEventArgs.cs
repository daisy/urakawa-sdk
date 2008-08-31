using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.xml;

namespace urakawa.events.property.xml
{
    /// <summary>
    /// Arguments of the <see cref="XmlProperty.QNameChanged"/> event
    /// </summary>
    public class QNameChangedEventArgs : XmlPropertyEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="XmlProperty"/> of the event
        /// and the previous+new QName 
        /// </summary>
        /// <param name="src">The source <see cref="XmlProperty"/> of the event</param>
        /// <param name="newLN">The local name part of the new QName</param>
        /// <param name="newNS">The namespace uri part of the new QName</param>
        /// <param name="prevLN">The local name part of the QName prior to the change</param>
        /// <param name="prevNS">The namespace uri part of the QName prior to the change</param>
        public QNameChangedEventArgs(XmlProperty src, string newLN, string newNS, string prevLN, string prevNS)
            : base(src)
        {
            NewLocalName = newLN;
            NewNamespaceUri = newNS;
            PreviousLocalName = prevLN;
            PreviousNamespaceUri = prevNS;
        }

        /// <summary>
        /// The local name part of the new QName
        /// </summary>
        public readonly string NewLocalName;

        /// <summary>
        /// The namespace uri part of the new QName
        /// </summary>
        public readonly string NewNamespaceUri;

        /// <summary>
        /// The local name part of the QName prior to the change
        /// </summary>
        public readonly string PreviousLocalName;

        /// <summary>
        /// The namespace uri part of the QName prior to the change
        /// </summary>
        public readonly string PreviousNamespaceUri;
    }
}