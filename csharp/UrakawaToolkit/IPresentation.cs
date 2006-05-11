using System;

namespace urakawa.core
{
	/// <summary>
	/// The presentation
	/// </summary>
	public interface IPresentation
	{
    /// <summary>
    /// Gets the root <see cref="ICoreNode"/> of the <see cref="IPresentation"/>
    /// </summary>
    /// <returns>The root <see cref="ICoreNode"/></returns>
    ICoreNode getRootNode();

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
