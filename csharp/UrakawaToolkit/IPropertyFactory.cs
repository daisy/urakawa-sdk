using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IPropertyFactory.
	/// </summary>
	public interface IPropertyFactory
	{
		IChannelsProperty createChannelsProperty(IChannelsManager manager);
		IXmlProperty createStructureProperty();
	}
}
