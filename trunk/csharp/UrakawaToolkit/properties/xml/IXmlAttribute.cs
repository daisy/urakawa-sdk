using System;
using urakawa.xuk;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Represents an attribute of an <see cref="XmlProperty"/>
	/// </summary>
	public interface IXmlAttribute: IXukAble
	{
    /// <summary>
    /// Creates a copy of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns>The copy</returns>
		IXmlAttribute copy();

    /// <summary>
    /// Gets the value of gthe <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns>The value</returns>
		string getValue();

    /// <summary>
    /// Sets the value of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <param name="newValue">The new value</param>
		void setValue(string newValue);

    /// <summary>
    /// Gets the namespace of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns>The namespace</returns>
		string getNamespaceUri();

    /// <summary>
    /// Gets the local localName of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns>The local localName</returns>
		string getLocalName();

    /// <summary>
    /// Sets the QName of the <see cref="IXmlAttribute"/> 
    /// </summary>
    /// <param name="newNamespace">The namespace part of the new QName</param>
    /// <param name="newName">The localName part of the new QName</param>
		void setQName(string newNamespace, string newName);

    /// <summary>
    /// Gets the parent <see cref="XmlProperty"/> of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns></returns>
		XmlProperty getParent();

		/// <summary>
		/// Sets the parent <see cref="XmlProperty"/> of <c>this</c>
		/// </summary>
		/// <param name="newParent">The new parent</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="newParent"/> is <c>null</c>
		/// </exception>
		void setParent(XmlProperty newParent);
	}
}
