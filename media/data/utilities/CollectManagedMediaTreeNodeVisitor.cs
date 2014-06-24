using System.Collections.Generic;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;
using urakawa.property.alt;

namespace urakawa.media.data.utilities
{
    /// <summary>
    /// Visitor that collects all <see cref="MediaData"/> used by the visited <see cref="TreeNode"/>s.
    /// </summary>
    public class CollectManagedMediaTreeNodeVisitor : ITreeNodeVisitor
    {
        private List<IManaged> mCollectedMedia = new List<IManaged>();

        /// <summary>
        /// Gets the list of collected <see cref="IManaged"/>
        /// </summary>
        /// <returns>The list</returns>
        /// <remarks>
        /// The returned list is a reference to the list in the <see cref="CollectManagedMediaTreeNodeVisitor"/> instance, 
        /// any changes made to the returned list will reflect in 
        /// the <see cref="CollectManagedMediaTreeNodeVisitor"/> instance
        /// </remarks>
        public List<IManaged> CollectedMedia
        {
            get { return mCollectedMedia; }
        }

        #region ITreeNodeVisitor Members

        /// <summary>
        /// Any <see cref="IManaged"/> used by the 
        /// </summary>
        /// <param name="node">The node being visited</param>
        /// <returns><c>true</c></returns>
        public bool PreVisit(TreeNode node)
        {
            foreach (property.Property prop in node.Properties.ContentsAs_Enumerable)
            {
                if (prop is ChannelsProperty)
                {
                    ChannelsProperty chProp = (ChannelsProperty)prop;
                    foreach (Channel ch in chProp.UsedChannels)
                    {
                        if (chProp.GetMedia(ch) is IManaged)
                        {
                            IManaged mm = (IManaged)chProp.GetMedia(ch);
                            if (!mCollectedMedia.Contains(mm))
                            {
                                mCollectedMedia.Add(mm);
                            }
                        }
                    }
                }
                else if (prop is AlternateContentProperty)
                {
                    AlternateContentProperty altProp = (AlternateContentProperty)prop;

                    foreach (AlternateContent ac in altProp.AlternateContents.ContentsAs_Enumerable)
                    {
                        if (ac.Audio != null && !mCollectedMedia.Contains(ac.Audio))
                        {
                            mCollectedMedia.Add(ac.Audio);
                        }
                        if (ac.Image != null && !mCollectedMedia.Contains(ac.Image))
                        {
                            mCollectedMedia.Add(ac.Image);
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
        public void PostVisit(TreeNode node)
        {
            return;
        }

        #endregion
    }
}