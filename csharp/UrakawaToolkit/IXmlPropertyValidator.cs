using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface providing methods to validate operations on <see cref="IXmlProperty"/>s
	/// </summary>
	public interface IXmlPropertyValidator
	{
    /// <summary>
    /// Determines if a given <see cref="IXmlAttribute"/> can be set
    /// </summary>
    /// <param name="newNamespace">The namespace of the <see cref="IXmlAttribute"/></param>
    /// <param name="newName">The local name of the <see cref="IXmlAttribute"/></param>
    /// <param name="newValue">The value of the <see cref="IXmlAttribute"/></param>
    /// <returns></returns>
		bool canSetAttribute(string newNamespace, string newName, string newValue);
		bool canRemoveAttribute(string removableNamespace, string removableName);

		bool canSetQName(string newNamespace, string newName);
		bool canSetName(string newName);
	}
}
