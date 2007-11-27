using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.xml;

namespace urakawa.events.property.xml
{
	public class XmlPropertyEventArgs : PropertyEventArgs
	{
		public XmlPropertyEventArgs(XmlProperty src)
			: base(src)
		{
			SourceXmlProperty = src;
		}
		public readonly XmlProperty SourceXmlProperty;
	}
}
