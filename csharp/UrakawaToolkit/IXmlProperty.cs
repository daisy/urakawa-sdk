using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IXmlProperty.
	/// </summary>
	public interface IXmlProperty : IProperty
	{
		XMLType getXMLType();
		string getName();
		string getNamespace();
		void setQName(string newNamespace, string newName);
		System.Collections.IList getListOfAttributes();
    void setAttribute(IXmlAttribute newAttribute);
    void setAttribute(string name, string ns, string value);
    IXmlAttribute getAttribute(string name, string ns);
	}

	public enum XMLType{ ELEMENT, TEXT};
}
