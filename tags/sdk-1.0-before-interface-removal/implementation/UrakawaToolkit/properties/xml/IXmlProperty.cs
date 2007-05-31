using System;
using System.Collections.Generic;
using urakawa.core;
using urakawa.core.property;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Interface for a <see cref="IProperty"/> containing XML structural information
	/// about a <see cref="ICoreNode"/>
	/// </summary>
	public interface IXmlProperty : IProperty
	{
    /// <summary>
    /// Gets the <see cref="XmlType"/> of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The <see cref="XmlType"/></returns>
		XmlType getXmlType();

    /// <summary>
    /// Gets the local localName of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The local localName</returns>
		string getLocalName();

    /// <summary>
    /// Gets the namespace uri of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The namespace uri</returns>
		string getNamespaceUri();

    /// <summary>
    /// Sets the QName of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <param name="newName">The local localName part of the QName</param>
    /// <param name="newNamespace">The namespace uri part of the QName</param>
		void setQName(string newName, string newNamespace);

    /// <summary>
		/// Gets a <see cref="List{IXmlAttribute}"/> of the <see cref="IXmlAttribute"/>s
    /// of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The list</returns>
		List<IXmlAttribute> getListOfAttributes();

    /// <summary>
    /// Sets an <see cref="IXmlAttribute"/> of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <param name="newAttribute">The new <see cref="IXmlAttribute"/> </param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="IXmlAttribute"/> was overwritten</returns>
    bool setAttribute(IXmlAttribute newAttribute);

    /// <summary>
    /// Sets an <see cref="IXmlAttribute"/> of the <see cref="IXmlProperty"/>
    /// </summary>
		/// <param name="name">The local localName of the new <see cref="IXmlAttribute"/></param>
		/// <param name="ns">The namespace of the new <see cref="IXmlAttribute"/></param>
    /// <param name="value">The value of the new <see cref="IXmlAttribute"/></param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="IXmlAttribute"/> was overwritten</returns>
		bool setAttribute(string name, string ns, string value);

    /// <summary>
    /// Gets an <see cref="IXmlAttribute"/> by QName
    /// </summary>
		/// <param name="name">The local localName of the <see cref="IXmlAttribute"/> to get</param>
		/// <param name="ns">The namespace of the <see cref="IXmlAttribute"/> to get</param>
    /// <returns>The <see cref="IXmlAttribute"/> with the given QName</returns>
    IXmlAttribute getAttribute(string name, string ns);

		/// <summary>
		/// Gets the <see cref="IXmlPropertyFactory"/> associated with <c>this</c> via the <see cref="ICorePresentation"/>
		/// of the owning <see cref="ICoreNode"/>
		/// </summary>
		/// <returns>The <see cref="IXmlPropertyFactory"/></returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="ICorePropertyFactory"/> of the <see cref="ICorePresentation"/>
		/// of the <see cref="ICoreNode"/> that owns <c>this</c> is not a subclass of <see cref="IXmlPropertyFactory"/>
		/// </exception>
		/// <remarks>
		/// This method is conveniencs for 
		/// <c>(IXmlPropertyFactory)getOwner().getPresentation().getPropertyFactory()</c></remarks>
		IXmlPropertyFactory getXmlPropertyFactory();

		/// <summary>
		/// Creates a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		new IXmlProperty copy();
	}

  /// <summary>
  /// The possible types of <see cref="IXmlProperty"/>s
  /// </summary>
	public enum XmlType{ 
    /// <summary>
    /// Element type - the <see cref="IXmlProperty"/> represents an XML element
    /// </summary>
    ELEMENT, 
    /// <summary>
    /// Text type - the <see cref="IXmlProperty"/> represents an XML text node
    /// </summary>
    TEXT
  };
}
