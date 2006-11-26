using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;

namespace urakawa.properties.channel
{
	/// <summary>
	/// <see cref="IPropertyFactory"/> that supports creation of <see cref="IChannelsProperty"/>s
	/// </summary>
	public interface IChannelsPropertyFactory : ICorePropertyFactory
	{
		/// <summary>
		/// Creates a <see cref="IChannelsProperty"/> of default type
		/// </summary>
		/// <returns>The created <see cref="IChannelsProperty"/></returns>
		IChannelsProperty createChannelsProperty();
	}
}
