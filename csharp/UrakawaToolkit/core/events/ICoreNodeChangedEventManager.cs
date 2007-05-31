using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.core.events
{
	/// <summary>
	/// Interface for a <see cref="CoreNode"/> changed event manager
	/// </summary>
	public interface ICoreNodeChangedEventManager
	{
		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is changed, i.e. added as a child of 
		/// or removed as the child of another <see cref="CoreNode"/>
		/// </summary>
		event CoreNodeChangedEventHandler coreNodeChanged;

		/// <summary>
		/// Fires the <see cref="coreNodeChanged"/> event
		/// </summary>
		/// <param name="changedNode">The node that changed</param>
		void notifyCoreNodeChanged(CoreNode changedNode);

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is added as a child of another <see cref="CoreNode"/>
		/// </summary>
		event CoreNodeAddedEventHandler coreNodeAdded;

		/// <summary>
		/// Fires the <see cref="coreNodeAdded"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="addedNode">The node that has been added</param>
		void notifyCoreNodeAdded(CoreNode addedNode);

		/// <summary>
		/// Event fired whenever a <see cref="CoreNode"/> is added as a child of another <see cref="CoreNode"/>
		/// </summary>
		event CoreNodeRemovedEventHandler coreNodeRemoved;


		/// <summary>
		/// Fires the <see cref="coreNodeRemoved"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="removedNode">The node that has been removed</param>
		/// <param name="formerParent">The parent node from which the node was removed as a child of</param>
		/// <param name="formerPosition">The position the node previously had of the list of children of it's former parent</param>
		void notifyCoreNodeRemoved(CoreNode removedNode, CoreNode formerParent, int formerPosition);
	}
}
