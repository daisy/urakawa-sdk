using System;

namespace urakawa.core
{
	/// <summary>
	/// Represents an attribute of an <see cref="XmlProperty"/>
	/// </summary>
	public interface IXmlAttribute: urakawa.core.IXUKable
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
		string getNamespace();

    /// <summary>
    /// Gets the local name of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns>The local name</returns>
		string getName();

    /// <summary>
    /// Sets the QName of the <see cref="IXmlAttribute"/> 
    /// </summary>
    /// <param name="newNamespace">The namespace part of the new QName</param>
    /// <param name="newName">The name part of the new QName</param>
		void setQName(string newNamespace, string newName);

    /// <summary>
    /// Gets the parent <see cref="IXmlProperty"/> of the <see cref="IXmlAttribute"/>
    /// </summary>
    /// <returns></returns>
		IXmlProperty getParent();

	}
}
