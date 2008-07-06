using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.events.core
{
    /// <summary>
    /// Base class for arguments for <see cref="TreeNode"/> sourced events
    /// </summary>
    public class TreeNodeEventArgs : DataModelChangedEventArgs
    {
        /// <summary>
        /// Constructor setting source <see cref="TreeNode"/> of the event
        /// </summary>
        /// <param name="src">The source <see cref="TreeNode"/> of the event</param>
        public TreeNodeEventArgs(TreeNode src) : base(src)
        {
            SourceTreeNode = src;
        }

        /// <summary>
        /// The source <see cref="TreeNode"/> of the event
        /// </summary>
        public readonly TreeNode SourceTreeNode;
    }
}