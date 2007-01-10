using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.navigation
{
	public class TypeFilterNavigator<T> : AbstractFilterNavigator where T : class, ICoreNode
	{
		public override bool isIncluded(ICoreNode node)
		{
			return (node is T);
		}

		public new T getNext(ICoreNode context)
		{
			return (base.getNext(context) as T);
		}
	}
}
