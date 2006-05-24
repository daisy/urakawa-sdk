using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IXmlAttribute.
	/// </summary>
	public interface IXmlAttribute: urakawa.xuk.IXUKable
	{
		IXmlAttribute copy();
		string getValue();
		void setValue(string newValue);

		string getNamespace();
		string getName();

		void setQName(string newNamespace, string newName);
		IXmlProperty getParent();

	}
}
