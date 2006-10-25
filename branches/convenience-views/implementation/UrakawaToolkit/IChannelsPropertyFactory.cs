using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;

namespace urakawa.properties.channel
{
	public interface IChannelsPropertyFactory : ICorePropertyFactory
	{
		IChannelsProperty createChannelsProperty();
	}
}
