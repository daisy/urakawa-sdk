using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events
{
	public class RootNodeChangedEventArgs : PresentationEventArgs
	{
		public RootNodeChangedEventArgs(Presentation source, core.TreeNode newRoot, core.TreeNode prevRoot)
			: base(source)
		{
			NewRootNode = newRoot;
			PreviousRootNode = prevRoot;
		}

		public readonly core.TreeNode NewRootNode;
		public readonly core.TreeNode PreviousRootNode;


	}
}
