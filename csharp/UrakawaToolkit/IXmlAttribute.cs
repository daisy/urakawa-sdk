using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IXmlAttribute.
	/// </summary>
	public interface IXmlAttribute
	{
		string Value{get;set;}
		string Namespace{get;}
		string Name{get;}

	}
}
