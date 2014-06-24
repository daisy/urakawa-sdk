using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.exception;
using urakawa.media;

namespace urakawa.property.channel
{
    /// <summary>
    /// <see cref="ITreeNodeVisitor"/> for clearing all media within a <see cref="Channel"/>
    /// </summary>
    public class ClearChannelTreeNodeVisitor : ITreeNodeVisitor
    {
        private Channel mChannelToClear;

        /// <summary>
        /// Gets the <see cref="Channel"/> within which to 
        /// clear <see cref="Media"/>
        /// </summary>
        public Channel ChannelToClear
        {
            get { return mChannelToClear; }
        }

        /// <summary>
        /// Constructor setting the <see cref="Channel"/> to clear
        /// </summary>
        /// <param name="chToClear"></param>
        public ClearChannelTreeNodeVisitor(Channel chToClear)
        {
            mChannelToClear = chToClear;
        }

        #region ITreeNodeVisitor Members

        /// <summary>
        /// Pre-visit action: If <see cref="Media"/> is present in <see cref="Channel"/> <see cref="ChannelToClear"/>,
        /// this is removed and the child <see cref="TreeNode"/>s are not visited
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> to visit</param>
        /// <returns>
        /// <c>false</c> if <see cref="Media"/> is found if <see cref="Channel"/> <see cref="ChannelToClear"/>,
        /// <c>false</c> else
        /// </returns>
        public bool PreVisit(TreeNode node)
        {
            ChannelsProperty chProp = node.GetChannelsProperty();
            if (chProp != null)
            {
                urakawa.media.Media m = null;

                //try
                //{
                    m = chProp.GetMedia(ChannelToClear);
//                }
//                catch (ChannelDoesNotExistException ex)
//                {
//#if DEBUG
//                    Debugger.Break();
//#endif
//                    return false;
//                }

                if (m != null)
                {
                    chProp.SetMedia(ChannelToClear, null);
                }
            }

            return true;
        }

        /// <summary>
        /// Post-visit action: Nothing is done here
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> to visit</param>
        public void PostVisit(TreeNode node)
        {
            // Nothing is done in post
        }

        #endregion
    }
}