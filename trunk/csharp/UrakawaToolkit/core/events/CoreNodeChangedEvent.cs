using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.core.events
{
	/// <summary>
	/// Delegate for <see cref="ICoreNode"/> changed events
	/// </summary>
	/// <param name="o">The sender of the event</param>
	/// <param name="e">The arguments of the event</param>
	public delegate void CoreNodeChangedEventHandler(object o, CoreNodeChangedEventArgs e);

	/// <summary>
	/// Common base class for <see cref="EventArgs"/> of <see cref="ICoreNode"/> changed events
	/// </summary>
	public class CoreNodeChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor setting the changed <see cref="ICoreNode"/>
		/// </summary>
		/// <param name="node">The changed node</param>
		public CoreNodeChangedEventArgs(ICoreNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The changed node can not be null");
			}
			mCoreNode = node;
		}

		private ICoreNode mCoreNode;
		/// <summary>
		/// Gets the <see cref="ICoreNode"/> that changed
		/// </summary>
		/// <returns>The changed node</returns>
		public ICoreNode getCoreNode()
		{
			return mCoreNode;
		}
	}
}
