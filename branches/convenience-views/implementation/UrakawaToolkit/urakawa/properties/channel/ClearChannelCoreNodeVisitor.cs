using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;

namespace urakawa.properties.channel
{
	/// <summary>
	/// <see cref="ICoreNodeVisitor"/> for clearing all media within a <see cref="IChannel"/>
	/// </summary>
	public class ClearChannelCoreNodeVisitor : ICoreNodeVisitor
	{
		private IChannel mChannelToClear;

		/// <summary>
		/// Gets the <see cref="IChannel"/> within which to 
		/// clear <see cref="urakawa.media.IMedia"/>
		/// </summary>
		public IChannel ChannelToClear
		{
			get
			{
				return mChannelToClear;
			}
		}

		/// <summary>
		/// Constructor setting the <see cref="IChannel"/> to clear
		/// </summary>
		/// <param localName="chToClear"></param>
		public ClearChannelCoreNodeVisitor(IChannel chToClear)
		{
			mChannelToClear = chToClear;
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// Pre-visit action: If <see cref="urakawa.media.IMedia"/> is present in <see cref="IChannel"/> <see cref="ChannelToClear"/>,
		/// this is removed and the child <see cref="ICoreNode"/>s are not visited
		/// </summary>
		/// <param localName="node">The <see cref="ICoreNode"/> to visit</param>
		/// <returns>
		/// <c>false</c> if <see cref="urakawa.media.IMedia"/> is found if <see cref="IChannel"/> <see cref="ChannelToClear"/>,
		/// <c>false</c> else
		/// </returns>
		public bool preVisit(ICoreNode node)
		{
			bool foundMedia = false;
			ChannelsProperty chProp =
				(ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
			if (chProp != null)
			{
				urakawa.media.IMedia m = chProp.getMedia(ChannelToClear);
				if (m != null)
				{
					chProp.setMedia(ChannelToClear, null);
				}
			}
			return !foundMedia;
		}

		/// <summary>
		/// Post-visit action: Nothing is done here
		/// </summary>
		/// <param localName="node">The <see cref="ICoreNode"/> to visit</param>
		public void postVisit(ICoreNode node)
		{
			// Nothing is done in post
		}

		#endregion
	}
}
