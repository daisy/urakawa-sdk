using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for ChannelsManager.
	/// </summary>
	public class ChannelsManager : IChannelsManager
	{
		public ChannelsManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IChannelsManager Members

		public Channel getChannel(string name)
		{
			// TODO:  Add ChannelsManager.getChannel implementation
			return null;
		}

		public Channel addChannel(Channel channel)
		{
			// TODO:  Add ChannelsManager.addChannel implementation
			return null;
		}

		public Channel addChannel(string name)
		{
			// TODO:  Add ChannelsManager.urakawa.core.IChannelsManager.addChannel implementation
			return null;
		}

		public void removeChannel(Channel channel)
		{
			// TODO:  Add ChannelsManager.removeChannel implementation
		}

		public Channel removeChannel(string name)
		{
			// TODO:  Add ChannelsManager.urakawa.core.IChannelsManager.removeChannel implementation
			return null;
		}

		public void setChannelName(Channel channel, string name)
		{
			// TODO:  Add ChannelsManager.setChannelName implementation
		}

		#endregion
	}
}
