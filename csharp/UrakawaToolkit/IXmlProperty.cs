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
		XmlAttributeList getListOfAttributes();
	}

	public enum XMLType{ ELEMENT, TEXT};
}
