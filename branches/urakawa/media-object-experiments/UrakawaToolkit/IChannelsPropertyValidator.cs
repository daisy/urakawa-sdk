using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Interface providing methods for validating operations 
	/// on <see cref="IChannelsProperty"/>s
	/// </summary>
	public interface IChannelsPropertyValidator
	{
    /// <summary>
    /// Determines if a given <see cref="IMedia"/> can be associated
    /// with a given <see cref="IChannel"/> 
    /// without breaking <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="channel">The given <see cref="IChannel"/></param>
    /// <param name="media">The given <see cref="IMedia"/></param>
    /// <returns>A <see cref="bool"/> indicating if the given <see cref="IMedia"/>
    /// can be associated with the given <see cref="IChannel"/></returns>
    bool canSetMedia(IChannel channel, IMedia media);
	}
}
