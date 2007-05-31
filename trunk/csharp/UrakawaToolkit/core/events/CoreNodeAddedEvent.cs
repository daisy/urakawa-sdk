using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.core.events
{
	/// <summary>
	/// Delegate for <see cref="CoreNode"/> added events
	/// </summary>
	/// <param name="o">The sender of the event</param>
	/// <param name="e">The arguments of the event</param>
	public delegate void CoreNodeAddedEventHandler(ICoreNodeChangedEventManager o, CoreNodeAddedEventArgs e);

	/// <summary>
	/// Argument of the <see cref="CoreNode"/> added events
	/// </summary>
	public class CoreNodeAddedEventArgs : CoreNodeChangedEventArgs
	{
		/// <summary>
		/// Constructor setting the <see cref="CoreNode"/> that has been added
		/// </summary>
		/// <param name="node">The node that has been added</param>
		public CoreNodeAddedEventArgs(CoreNode node) : base(node) 
		{ 
		}
	}
}