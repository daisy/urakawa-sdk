using System;
using urakawa.core;
using urakawa.media;

namespace urakawa.examples
{
	/// <summary>
	/// Example visitor that collects all <see cref="IMedia"/> in a given channel
	/// </summary>
	public class CollectMediaFromChannelCoreNodeVisitor : ICoreNodeVisitor
	{
		/// <summary>
		/// An integer that indicates the number of <see cref="ICoreNode"/>s visited
		/// </summary>
		public ulong VisitCount = 0;
		private System.Collections.ArrayList mCollectedMedia;

		/// <summary>
		/// Gets an array of the collected <see cref="IMedia"/>
		/// </summary>
		public IMedia[] CollectedMedia
		{
			get
			{
				return (IMedia[])mCollectedMedia.ToArray(typeof(IMedia));
			}
		}
		private IChannel mChannel;

		/// <summary>
		/// Gets the <see cref="IChannel"/> from which <see cref="IMedia"/> is collected
		/// </summary>
		public IChannel CollectorChannel
		{
			get
			{
				return mChannel;
			}
		}

		/// <summary>
		/// Constructor setting the <see cref="IChannel"/> from which media is collected
		/// </summary>
		/// <param name="ch"></param>
		public CollectMediaFromChannelCoreNodeVisitor(IChannel ch)
		{
			mCollectedMedia = new System.Collections.ArrayList();
			mChannel = ch;
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// Pre-visit action:
		/// If <see cref="IMedia"/> is present in <see cref="IChannel"/> <see cref="CollectorChannel"/>,
		/// this is added to <see cref="CollectedMedia"/> and the child <see cref="ICoreNode"/>s are not visited
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> to visit</param>
		/// <returns>
		/// <c>true</c> is no <see cref="IMedia"/> is present in <see cref="IChannel"/> <see cref="CollectorChannel"/>,
		/// <c>false</c> else
		/// </returns>
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

		/// <summary>
		/// Post-visit action: Nothing is done here
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> to visit</param>
		public void postVisit(ICoreNode node)
		{
			// Nothing is done!!!
		}

		#endregion
	}
}
