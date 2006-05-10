using System;
using System.Collections;

using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for IChannelsProperty.
	/// </summary>
	public interface IChannelsProperty
	{
		IMedia getMedia(Channel channel);

		void setMedia(Channel channel, IMedia media);

		ArrayList getListOfUsedChannels();
	}
}
