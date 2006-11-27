using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.core.property;
using urakawa.media;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Interface for <see cref="ICorePresentation"/>s that supports <see cref="Channel"/>s
	/// </summary>
	public interface IChannelPresentation : IMediaPresentation
	{

		/// <summary>
		/// Gets the <see cref="IChannelFactory"/> that creates <see cref="IChannel"/>s 
		/// for the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelFactory"/></returns>
		IChannelFactory getChannelFactory();

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> managing the list of <see cref="IChannel"/>s
		/// in the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		IChannelsManager getChannelsManager();

		/// <summary>
		/// Gets the <see cref="IChannelsPropertyFactory"/> creating <see cref="IProperty"/>s 
		/// for <see cref="ICoreNode"/>s for the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsPropertyFactory"/></returns>
		IChannelsPropertyFactory getChannelsPropertyFactory();


	}
}
