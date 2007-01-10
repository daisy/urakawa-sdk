using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	public class TypeFilterNavigator<T> : AbstractFilterNavigator
	{
		public override bool isIncluded(urakawa.core.ICoreNode node)
		{
			return (node is T);
		}

		public new T getNext(ICoreNode context)
		{
			return (base.getNext(context) as T);
		}
	}
}
