using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.presentation
{
	public class RootNodeChangedEventArgs : PresentationEventArgs
	{
		public RootNodeChangedEventArgs(Presentation source, urakawa.core.TreeNode newRoot, urakawa.core.TreeNode prevRoot)
			: base(source)
		{
			NewRootNode = newRoot;
			PreviousRootNode = prevRoot;
		}

		public readonly urakawa.core.TreeNode NewRootNode;
		public readonly urakawa.core.TreeNode PreviousRootNode;


	}
}
