using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for PropertyFactory.
	/// </summary>
	public class PropertyFactory : IPropertyFactory 
	{
		public PropertyFactory()
		{
		}

		#region IPropertyFactory Members

		public IChannelsProperty createChannelsProperty(IChannelsManager manager)
		{
			return new ChannelsProperty(manager);
		}

		public IXmlProperty createStructureProperty()
		{
			return new XmlProperty();
		}

		#endregion
	}
}
