using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for a <see cref="IProperty"/> containing XML structural information
	/// about a <see cref="ICoreNode"/>
	/// </summary>
	public interface IXmlProperty : IProperty
	{
    /// <summary>
    /// Gets the <see cref="XMLType"/> of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The <see cref="XMLType"/></returns>
		XMLType getXMLType();

    /// <summary>
    /// Gets the local name of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The local name</returns>
		string getName();

    /// <summary>
    /// Gets the namespace uri of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The namespace uri</returns>
		string getNamespace();

    /// <summary>
    /// Sets the QName of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <param name="newName">The local name part of the QName</param>
    /// <param name="newNamespace">The namespace uri part of the QName</param>
		void setQName(string newName, string newNamespace);

    /// <summary>
    /// Gets a <see cref="System.Collections.IList"/> of the <see cref="IXmlAttribute"/>s
    /// of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <returns>The <see cref="System.Collections.IList"/></returns>
		System.Collections.Generic.IList<IXmlAttribute> getListOfAttributes();

    /// <summary>
    /// Sets an <see cref="IXmlAttribute"/> of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <param name="newAttribute">The new <see cref="IXmlAttribute"/> </param>
    void setAttribute(IXmlAttribute newAttribute);

    /// <summary>
    /// Sets an <see cref="IXmlAttribute"/> of the <see cref="IXmlProperty"/>
    /// </summary>
    /// <param name="name">The local name of the new <see cref="IXmlAttribute"/></param>
    /// <param name="ns">The namespace of the new <see cref="IXmlAttribute"/></param>
    /// <param name="value">The value of the new <see cref="IXmlAttribute"/></param>
    void setAttribute(string name, string ns, string value);

    /// <summary>
    /// Gets an <see cref="IXmlAttribute"/> by QName
    /// </summary>
    /// <param name="name">The local name of the <see cref="IXmlAttribute"/> to get</param>
    /// <param name="ns">The namespace of the <see cref="IXmlAttribute"/> to get</param>
    /// <returns>The <see cref="IXmlAttribute"/> with the given QName</returns>
    IXmlAttribute getAttribute(string name, string ns);
	}

  /// <summary>
  /// The possible types of <see cref="IXmlProperty"/>s
  /// </summary>
	public enum XMLType{ 
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
