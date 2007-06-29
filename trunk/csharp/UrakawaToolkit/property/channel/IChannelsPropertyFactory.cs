using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property;

namespace urakawa.property.channel
{
	/// <summary>
	/// <see cref="PropertyFactory"/> that supports creation of <see cref="ChannelsProperty"/>s
	/// </summary>
	public interface IChannelsPropertyFactory : IGenericPropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="ChannelsProperty"/> of default type
		/// </summary>
		/// <returns>The created <see cref="ChannelsProperty"/></returns>
		ChannelsProperty createChannelsProperty();
	}
}
