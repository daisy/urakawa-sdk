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
		/// Gets the <see cref="ChannelFactory"/> that creates <see cref="Channel"/>s 
		/// for the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="ChannelFactory"/></returns>
		ChannelFactory getChannelFactory();

		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> managing the list of <see cref="Channel"/>s
		/// in the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		ChannelsManager getChannelsManager();

		/// <summary>
		/// Gets the <see cref="IChannelsPropertyFactory"/> creating <see cref="Property"/>s 
		/// for <see cref="CoreNode"/>s for the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsPropertyFactory"/></returns>
		new IChannelsPropertyFactory getPropertyFactory();


	}
}
