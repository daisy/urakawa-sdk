using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.core.events
{
	/// <summary>
	/// Delegate for <see cref="CoreNode"/> changed events
	/// </summary>
	/// <param name="o">The sender of the event</param>
	/// <param name="e">The arguments of the event</param>
	public delegate void CoreNodeChangedEventHandler(object o, CoreNodeChangedEventArgs e);

	/// <summary>
	/// Common base class for <see cref="EventArgs"/> of <see cref="CoreNode"/> changed events
	/// </summary>
	public class CoreNodeChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor setting the changed <see cref="CoreNode"/>
		/// </summary>
		/// <param name="node">The changed node</param>
		public CoreNodeChangedEventArgs(CoreNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The changed node can not be null");
			}
			mCoreNode = node;
		}

		private CoreNode mCoreNode;
		/// <summary>
		/// Gets the <see cref="CoreNode"/> that changed
		/// </summary>
		/// <returns>The changed node</returns>
		public CoreNode getCoreNode()
		{
			return mCoreNode;
		}
	}
}
