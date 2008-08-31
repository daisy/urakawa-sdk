using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.xml;

namespace urakawa.events.property.xml
{
    /// <summary>
    /// Base class for arguments of <see cref="XmlProperty"/> sourced events
    /// </summary>
    public class XmlPropertyEventArgs : PropertyEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="XmlProperty"/> of the event
        /// </summary>
        /// <param name="src">The source <see cref="XmlProperty"/> of the event</param>
        public XmlPropertyEventArgs(XmlProperty src)
            : base(src)
        {
            SourceXmlProperty = src;
        }

        /// <summary>
        /// The source <see cref="XmlProperty"/> of the event
        /// </summary>
        public readonly XmlProperty SourceXmlProperty;
    }
}