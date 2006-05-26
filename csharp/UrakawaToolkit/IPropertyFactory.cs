using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IPropertyFactory.
	/// </summary>
	public interface IPropertyFactory
	{
		IChannelsProperty createChannelsProperty();
    IXmlProperty createXmlProperty(string name, string ns);
	}
}
