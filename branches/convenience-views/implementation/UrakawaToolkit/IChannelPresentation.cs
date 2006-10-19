using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.properties.channel
{
	/// <summary>
	/// Interface for <see cref="ICorePresentation"/>s that supports <see cref="Channel"/>s
	/// </summary>
	public interface IChannelPresentation : ICorePresentation
	{

		/// <summary>
		/// Gets the <see cref="IChannelFactory"/> that creates <see cref="IChannel"/>s for the presentation
		/// </summary>
		/// <returns>The <see cref="IChannelFactory"/></returns>
		IChannelFactory getChannelFactory();

		/// <summary>
		/// Gets the <see cref="IChannelsManager"/> managing the list of <see cref="IChannel"/>s
		/// in the <see cref="IPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsManager"/></returns>
		IChannelsManager getChannelsManager();
	}
}
