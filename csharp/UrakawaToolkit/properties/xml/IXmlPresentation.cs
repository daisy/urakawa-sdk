using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Interface for a presentation that support <see cref="XmlProperty"/>s
	/// </summary>
	public interface IXmlPresentation : core.ICorePresentation
	{
		/// <summary>
		/// Gets the factory creating <see cref="XmlProperty"/>s and <see cref="XmlAttribute"/>s used by theese
		/// </summary>
		/// <returns>The factory</returns>
		new IXmlPropertyFactory getPropertyFactory();
	}
}
