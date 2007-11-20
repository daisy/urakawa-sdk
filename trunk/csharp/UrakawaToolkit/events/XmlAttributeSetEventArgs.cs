using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.xml;

namespace urakawa.events
{
	public class XmlAttributeSetEventArgs : XmlPropertyEventArgs
	{
		public XmlAttributeSetEventArgs(XmlProperty src, string attrLN, string attrNS, string newVal, string prevVal) : base(src)
		{
			AttributeLocalName = attrLN;
			AttributeNamespaceUri = attrNS;
			NewValue = newVal;
			PreviousValue = prevVal;
		}
		public readonly string AttributeLocalName;
		public readonly string AttributeNamespaceUri;
		public readonly string NewValue;
		public readonly string PreviousValue;
	}
}
