using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property;

namespace urakawa.property.xml
{
	/// <summary>
	/// <see cref="PropertyFactory"/> that supports creation of <see cref="XmlProperty"/>s 
	/// and <see cref="XmlAttribute"/>s
	/// </summary>
	public interface IXmlPropertyFactory : IGenericPropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="XmlProperty"/> of default type
		/// </summary>
		/// <returns>The created <see cref="XmlProperty"/></returns>
		XmlProperty createXmlProperty();

		/// <summary>
		/// Creates a <see cref="XmlAttribute"/> of default type
		/// with a given parent <see cref="XmlProperty"/>
		/// </summary>
		/// <param name="parent">The parent <see cref="XmlProperty"/></param>
		/// <returns>The created <see cref="XmlAttribute"/></returns>
		XmlAttribute createXmlAttribute(XmlProperty parent);

		/// <summary>
		/// Creates a <see cref="XmlAttribute"/> of type matching a given QName 
		/// with a given parent <see cref="XmlProperty"/>
		/// </summary>
		/// <param name="localName">The local localName part of the given QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given QName</param>
		/// <param name="parent">The parent <see cref="XmlProperty"/></param>
		/// <returns>The created <see cref="XmlAttribute"/>, <c>null</c> if the given QName is not recognized</returns>
		XmlAttribute createXmlAttribute(XmlProperty parent, string localName, string namespaceUri);
	}
}
