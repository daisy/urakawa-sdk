using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;

namespace urakawa.properties.xml
{
	/// <summary>
	/// <see cref="IPropertyFactory"/> that supports creation of <see cref="IXmlProperty"/>s 
	/// and <see cref="IXmlAttribute"/>s
	/// </summary>
	public interface IXmlPropertyFactory : ICorePropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="IXmlProperty"/> of default type
		/// </summary>
		/// <returns>The created <see cref="IXmlProperty"/></returns>
		IXmlProperty createXmlProperty();

		/// <summary>
		/// Creates a <see cref="IXmlAttribute"/> of default type
		/// with a given parent <see cref="IXmlProperty"/>
		/// </summary>
		/// <param localName="parent">The parent <see cref="IXmlProperty"/></param>
		/// <returns>The created <see cref="IXmlAttribute"/></returns>
		IXmlAttribute createXmlAttribute(IXmlProperty parent);

		/// <summary>
		/// Creates a <see cref="IXmlAttribute"/> of type matching a given QName 
		/// with a given parent <see cref="IXmlProperty"/>
		/// </summary>
		/// <param localName="localName">The local localName part of the given QName</param>
		/// <param localName="namespaceUri">The namespace uri part of the given QName</param>
		/// <param localName="parent">The parent <see cref="IXmlProperty"/></param>
		/// <returns>The created <see cref="IXmlAttribute"/>, <c>null</c> if the given QName is not recognized</returns>
		IXmlAttribute createXmlAttribute(IXmlProperty parent, string localName, string namespaceUri);
	}
}
