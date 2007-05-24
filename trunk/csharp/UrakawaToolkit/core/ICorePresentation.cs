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
		/// Gets the root <see cref="ICoreNode"/> of the presentation
		/// </summary>
		/// <returns>The root <see cref="ICoreNode"/></returns>
		ICoreNode getRootNode();

		/// <summary>
		/// Sets the root <see cref="ICoreNode"/> of the presentation
		/// </summary>
		/// <param name="newRoot">The new root <see cref="ICoreNode"/> or <c>null</c></param>
		void setRootNode(ICoreNode newRoot);

		/// <summary>
		/// Gets the factory creating <see cref="ICoreNode"/>s
		/// for the presentation
		/// </summary>
		/// <returns>The core node factory</returns>
		ICoreNodeFactory getCoreNodeFactory();

		/// <summary>
		/// Gets the factory creating <see cref="IProperty"/>s
		/// for the presentation
		/// </summary>
		/// <returns>The property factory</returns>
		ICorePropertyFactory getPropertyFactory();
	}
}
