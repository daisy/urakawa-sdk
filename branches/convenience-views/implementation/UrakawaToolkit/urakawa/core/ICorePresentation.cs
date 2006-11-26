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
	public interface ICorePresentation : IXukAble
	{
		/// <summary>
		/// Gets the root <see cref="ICoreNode"/> of the <see cref="ICorePresentation"/>
		/// </summary>
		/// <returns>The root <see cref="ICoreNode"/></returns>
		ICoreNode getRootNode();

		/// <summary>
		/// Sets the root <see cref="ICoreNode"/> of the <see cref="ICorePresentation"/>
		/// </summary>
		/// <param localName="newRoot">The new root <see cref="ICoreNode"/> or <c>null</c></param>
		void setRootNode(ICoreNode newRoot);

		/// <summary>
		/// Gets the <see cref="ICoreNodeFactory"/> creating <see cref="ICoreNode"/>s
		/// for the <see cref="ICorePresentation"/>
		/// </summary>
		/// <returns>The <see cref="ICoreNodeFactory"/></returns>
		ICoreNodeFactory getCoreNodeFactory();

		/// <summary>
		/// Gets the <see cref="ICorePropertyFactory"/> creating <see cref="IProperty"/>s
		/// for the <see cref="ICorePresentation"/>
		/// </summary>
		/// <returns>The <see cref="ICorePropertyFactory"/></returns>
		ICorePropertyFactory getPropertyFactory();
	}
}
