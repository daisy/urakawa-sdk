using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for Presentation.
	/// </summary>
	public class Presentation : IChannelManager
	{
		CoreNode mRootNode;

		public Presentation()
		{
		}

		CoreNode getRootNode()
		{
			return mRootNode;
		}

		#region IChannelManager Members

		public Channel getChannel(string name)
		{
			// TODO:  Add Presentation.getChannel implementation
			return null;
		}

		public Channel addChannel(Channel channel)
		{
			// TODO:  Add Presentation.addChannel implementation
			return null;
		}

		Channel urakawa.core.IChannelManager.addChannel(string name)
		{
			// TODO:  Add Presentation.urakawa.core.IChannelManager.addChannel implementation
			return null;
		}

		public void removeChannel(Channel channel)
		{
			// TODO:  Add Presentation.removeChannel implementation
		}

		Channel urakawa.core.IChannelManager.removeChannel(string name)
		{
			// TODO:  Add Presentation.urakawa.core.IChannelManager.removeChannel implementation
			return null;
		}

		public void setChannelName(Channel channel, string name)
		{
			// TODO:  Add Presentation.setChannelName implementation
		}

		#endregion
	}
}
