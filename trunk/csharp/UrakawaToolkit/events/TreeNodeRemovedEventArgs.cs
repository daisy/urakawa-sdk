using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.events
{
	public class TreeNodeRemovedEventArgs : TreeNodeEventArgs
	{
		public TreeNodeRemovedEventArgs(TreeNode notfr, TreeNode formerParnt, int formerPos) : base(notfr)
		{
			mFormerParent = FormerParent;
			mFormatPosition = formerPos;
		}

		private TreeNode mFormerParent;
		public TreeNode FormerParent { get { return mFormerParent; } }

		public int mFormatPosition;
		public int FormerPosition { get { return mFormatPosition; } }
	}
}
