using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IChannelsPropertyValidator.
	/// </summary>
	public interface IChannelsPropertyValidator
	{
		bool canSetMedia(Channel channel, IMedia media);
	}
}
