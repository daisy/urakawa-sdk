using System;
using urakawa.core;
using urakawa.media;

namespace urakawa.examples
{
	/// <summary>
	/// Summary description for CollectMediaFromChannelCoreNodeVisitor.
	/// </summary>
	public class CollectMediaFromChannelCoreNodeVisitor : ICoreNodeVisitor
	{
		public ulong VisitCount = 0;
		private System.Collections.ArrayList mCollectedMedia;
		public IMedia[] CollectedMedia
		{
			get
			{
				return (IMedia[])mCollectedMedia.ToArray(typeof(IMedia));
			}
		}
		private IChannel mChannel;
		public IChannel CollectorChannel
		{
			get
			{
				return mChannel;
			}
		}

		public CollectMediaFromChannelCoreNodeVisitor(IChannel ch)
		{
			mCollectedMedia = new System.Collections.ArrayList();
			mChannel = ch;
		}
		#region ICoreNodeVisitor Members

		public bool preVisit(ICoreNode node)
		{
			bool foundMedia = false;
			ChannelsProperty chProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
			if (chProp!=null)
			{
				if (chProp.getMedia(CollectorChannel)!=null)
				{
					foundMedia = true;
					chProp.setMedia(CollectorChannel, null);
				}
			}
			VisitCount++;
			return !foundMedia;
		}

		public void postVisit(ICoreNode node)
		{
			// TODO:  Add CollectMediaFromChannelCoreNodeVisitor.postVisit implementation
		}

		#endregion
	}
}
