using System;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChannelManager
	{
		Channel getChannel(string name);
		Channel addChannel(Channel channel);
		Channel addChannel(string name);
		void removeChannel(Channel channel);
		Channel removeChannel(string name);
		void setChannelName(Channel channel, string name);
	}
}
