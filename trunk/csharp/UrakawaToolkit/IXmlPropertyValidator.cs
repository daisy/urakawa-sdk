using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IXmlPropertyValidator.
	/// </summary>
	public interface IXmlPropertyValidator
	{
		bool canSetAttribute(string newNamespace, string newName, string newValue);
		bool canRemoveAttribute(string removableNamespace, string removableName);

		bool canSetQName(string newNamespace, string newName);
		bool canSetName(string newName);
	}
}
