using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core.property;
using urakawa.xuk;

namespace urakawa.core
{
	/// <summary>
	/// Interface for a basic core presentation
	/// </summary>
	public interface ICorePresentation : events.ICoreNodeChangedEventManager, IXukAble
	{
		/// <summary>
		/// Gets the root <see cref="CoreNode"/> of the presentation
		/// </summary>
		/// <returns>The root <see cref="CoreNode"/></returns>
		CoreNode getRootNode();

		/// <summary>
		/// Sets the root <see cref="CoreNode"/> of the presentation
		/// </summary>
		/// <param name="newRoot">The new root <see cref="CoreNode"/> or <c>null</c></param>
		void setRootNode(CoreNode newRoot);

		/// <summary>
		/// Gets the factory creating <see cref="CoreNode"/>s
		/// for the presentation
		/// </summary>
		/// <returns>The core node factory</returns>
		CoreNodeFactory getCoreNodeFactory();

		/// <summary>
		/// Gets the factory creating <see cref="Property"/>s
		/// for the presentation
		/// </summary>
		/// <returns>The property factory</returns>
		ICorePropertyFactory getPropertyFactory();
	}
}
