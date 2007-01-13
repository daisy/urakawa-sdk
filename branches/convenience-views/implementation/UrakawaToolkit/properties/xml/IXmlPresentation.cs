using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.properties.xml
{
	public interface IXmlPresentation : core.ICorePresentation
	{
		IXmlPropertyFactory getXmlPropertyFactory();
	}
}
