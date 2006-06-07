using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface providing methods to validate operations on <see cref="IXmlProperty"/>s
	/// </summary>
	public interface IXmlPropertyValidator
	{
    /// <summary>
    /// Tests if a given <see cref="IXmlAttribute"/> can be set
    /// </summary>
    /// <param name="newName">The local name of the <see cref="IXmlAttribute"/></param>
    /// <param name="newNamespace">The namespace of the <see cref="IXmlAttribute"/></param>
    /// <param name="newValue">The value of the <see cref="IXmlAttribute"/></param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
		bool canSetAttribute(string newName, string newNamespace, string newValue);

    /// <summary>
    /// Tests if a given <see cref="IXmlAttribute"/> can be removed 
    /// </summary>
    /// <param name="removableName">The local name of the given <see cref="IXmlAttribute"/></param>
    /// <param name="removableNamespace">The namepsace of the given <see cref="IXmlAttribute"/></param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
		bool canRemoveAttribute(string removableName, string removableNamespace);

    /// <summary>
    /// Tests if a given QName can be set
    /// </summary>
    /// <param name="newName">The local name part of the QName</param>
    /// <param name="newNamespace">The namespace part of the QName</param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
		bool canSetQName(string newName, string newNamespace);

    /// <summary>
    /// Tests if a given local name can be set. 
    /// Convenience method, equivlent to 
    /// <c><see cref="canSetQName"/>(newName, <see cref="ICoreNode.getNamespace"/>())</c>
    /// </summary>
    /// <param name="newName">The locan name</param>
    /// <returns>A <see cref="bool"/> indicating the result of the test</returns>
		bool canSetName(string newName);
	}
}
