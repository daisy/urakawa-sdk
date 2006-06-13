using System;
using urakawa.xuk;

namespace urakawa.core
{
	/// <summary>
	/// The presentation
	/// </summary>
	public interface IPresentation:IXUKable
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


    /// <summary>
    /// Gets the <see cref="ICoreNodeFactory"/> creating <see cref="ICoreNode"/>s
    /// for the <see cref="IPresentation"/>
    /// </summary>
    /// <returns>The <see cref="ICoreNodeFactory"/></returns>
    ICoreNodeFactory getCoreNodeFactory();

    /// <summary>
    /// Gets the <see cref="IPropertyFactory"/> creating <see cref="IProperty"/>s
    /// for the <see cref="IPresentation"/>
    /// </summary>
    /// <returns>The <see cref="IPropertyFactory"/></returns>
    IPropertyFactory getPropertyFactory();

    /// <summary>
    /// Gets the <see cref="urakawa.media.IMediaFactory"/> creating <see cref="urakawa.media.IMedia"/>
    /// for the <see cref="IPresentation"/>
    /// </summary>
    /// <returns></returns>
    urakawa.media.IMediaFactory getMediaFactory();
	}
}
