using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.core.events
{
	/// <summary>
	/// Delegate for <see cref="ICoreNode"/> added events
	/// </summary>
	/// <param name="o">The sender of the event</param>
	/// <param name="e">The arguments of the event</param>
	public delegate void CoreNodeRemovedEventHandler(ICoreNodeChangedEventManager o, CoreNodeRemovedEventArgs e);

	/// <summary>
	/// Argument of the <see cref="ICoreNode"/> added events
	/// </summary>
	public class CoreNodeRemovedEventArgs : CoreNodeChangedEventArgs
	{
		/// <summary>
		/// Constructor setting the <see cref="ICoreNode"/> that has been removed, the former parent <see cref="ICoreNode"/>
		/// and the position in the list of children of it's former parent
		/// </summary>
		/// <param name="node">The node that has been removed</param>
		/// <param name="formerParent">The former parent</param>
		/// <param name="formerPosition">The former position</param>
		public CoreNodeRemovedEventArgs(ICoreNode node, ICoreNode formerParent, int formerPosition)
			: base(node)
		{
			if (formerParent == null)
			{
				throw new exception.MethodParameterIsNullException("The former parent of the removed node can not be null");
			}
			mFormerParent = formerParent;
			mFormerPosition = formerPosition;
		}

		private ICoreNode mFormerParent;
		/// <summary>
		/// Gets the former parent of the removed node
		/// </summary>
		/// <returns>The former parent</returns>
		public ICoreNode getFormerParent()
		{
			return mFormerParent;
		}

		private int mFormerPosition;
		/// <summary>
		/// Gets the position of the removed node in the list of children of it's former parent
		/// </summary>
		/// <returns>The position</returns>
		public int getFormerPosition()
		{
			return mFormerPosition;
		}
	}
}