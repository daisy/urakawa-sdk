using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	public interface INavigator
	{
		ICoreNode getParent(ICoreNode context);
		ICoreNode getPreviousSibling(ICoreNode context);
		ICoreNode getNextSibling(ICoreNode context);
	}
}
