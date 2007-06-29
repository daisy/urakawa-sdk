using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.property;
using urakawa.media;

namespace urakawa.property.channel
{
	/// <summary>
	/// Interface for <see cref="ITreePresentation"/>s that supports <see cref="Channel"/>s
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
		/// for <see cref="TreeNode"/>s for the <see cref="IChannelPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IChannelsPropertyFactory"/></returns>
		new IChannelsPropertyFactory getPropertyFactory();


	}
}
