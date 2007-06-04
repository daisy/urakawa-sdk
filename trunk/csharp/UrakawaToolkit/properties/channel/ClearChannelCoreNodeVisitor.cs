using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;

namespace urakawa.properties.channel
{
	/// <summary>
	/// <see cref="ICoreNodeVisitor"/> for clearing all media within a <see cref="Channel"/>
	/// </summary>
	public class ClearChannelCoreNodeVisitor : ICoreNodeVisitor
	{
		private Channel mChannelToClear;

		/// <summary>
		/// Gets the <see cref="Channel"/> within which to 
		/// clear <see cref="urakawa.media.IMedia"/>
		/// </summary>
		public Channel ChannelToClear
		{
			get
			{
				return mChannelToClear;
			}
		}

		/// <summary>
		/// Constructor setting the <see cref="Channel"/> to clear
		/// </summary>
		/// <param name="chToClear"></param>
		public ClearChannelCoreNodeVisitor(Channel chToClear)
		{
			mChannelToClear = chToClear;
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// Pre-visit action: If <see cref="urakawa.media.IMedia"/> is present in <see cref="Channel"/> <see cref="ChannelToClear"/>,
		/// this is removed and the child <see cref="TreeNode"/>s are not visited
		/// </summary>
		/// <param name="node">The <see cref="TreeNode"/> to visit</param>
		/// <returns>
		/// <c>false</c> if <see cref="urakawa.media.IMedia"/> is found if <see cref="Channel"/> <see cref="ChannelToClear"/>,
		/// <c>false</c> else
		/// </returns>
		public bool preVisit(TreeNode node)
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
		/// <param name="node">The <see cref="TreeNode"/> to visit</param>
		public void postVisit(TreeNode node)
		{
			// Nothing is done in post
		}

		#endregion
	}
}
