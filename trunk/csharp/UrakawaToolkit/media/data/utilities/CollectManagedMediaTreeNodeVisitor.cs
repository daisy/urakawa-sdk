using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;

namespace urakawa.media.data.utilities
{
	/// <summary>
	/// Visitor that collects all <see cref="MediaData"/> used by the visited <see cref="TreeNode"/>s.
	/// </summary>
	public class CollectManagedMediaTreeNodeVisitor : ITreeNodeVisitor
	{
		private List<IManagedMedia> mCollectedMedia = new List<IManagedMedia>();

		/// <summary>
		/// Gets the list of collected <see cref="IManagedMedia"/>
		/// </summary>
		/// <returns>The list</returns>
		/// <remarks>
		/// The returned list is a reference to the list in the <see cref="CollectManagedMediaTreeNodeVisitor"/> instance, 
		/// any changes made to the returned list will reflect in 
		/// the <see cref="CollectManagedMediaTreeNodeVisitor"/> instance
		/// </remarks>
		public List<IManagedMedia> getListOfCollectedMedia()
		{
			return mCollectedMedia;
		}

		#region ITreeNodeVisitor Members

		/// <summary>
		/// Any <see cref="IManagedMedia"/> used by the 
		/// </summary>
		/// <param name="node">The node being visited</param>
		/// <returns><c>true</c></returns>
		public bool preVisit(TreeNode node)
		{
			foreach (Type propType in node.getListOfUsedPropertyTypes())
			{
				if (propType.IsSubclassOf(typeof(ChannelsProperty)))
				{
					ChannelsProperty chProp = (ChannelsProperty)node.getProperty(propType);
					foreach (Channel ch in chProp.getListOfUsedChannels())
					{
						if (chProp.getMedia(ch) is IManagedMedia)
						{
							IManagedMedia mm = (IManagedMedia)chProp.getMedia(ch);
							if (!mCollectedMedia.Contains(mm)) mCollectedMedia.Add(mm);
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Nothing is done in post visit
		/// </summary>
		/// <param name="node">The node being visited</param>
		public void postVisit(TreeNode node)
		{
			return;
		}

		#endregion
	}
}
