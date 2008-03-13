using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.xml;

namespace urakawa.events.property.xml
{
	public class QNameChangedEventArgs : XmlPropertyEventArgs
	{
		public QNameChangedEventArgs(XmlProperty src, string newLN, string newNS, string prevLN, string prevNS) : base(src)
		{
			NewLocalName = newLN;
			NewNamespaceUri = newNS;
			PreviousLocalName = prevLN;
			PreviousNamespaceUri = prevNS;
		}

		public readonly string NewLocalName;
		public readonly string NewNamespaceUri;
		public readonly string PreviousLocalName;
		public readonly string PreviousNamespaceUri;

	}
}
