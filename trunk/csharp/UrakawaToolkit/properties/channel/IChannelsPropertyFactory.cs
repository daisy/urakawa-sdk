using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;

namespace urakawa.properties.channel
{
	/// <summary>
	/// <see cref="PropertyFactory"/> that supports creation of <see cref="ChannelsProperty"/>s
	/// </summary>
	public interface IChannelsPropertyFactory : ICorePropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/> of default type
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		ChannelsProperty createChannelsProperty();
	}
}
