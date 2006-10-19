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
	public interface ICorePresentation : IXUKAble
	{
		/// <summary>
		/// Gets the root <see cref="ICoreNode"/> of the <see cref="IPresentation"/>
		/// </summary>
		/// <returns>The root <see cref="ICoreNode"/></returns>
		ICoreNode getRootNode();

		/// <summary>
		/// Gets the <see cref="ICoreNodeFactory"/> creating <see cref="ICoreNode"/>s
		/// for the <see cref="IPresentation"/>
		/// </summary>
		/// <returns>The <see cref="ICoreNodeFactory"/></returns>
		ICoreNodeFactory getCoreNodeFactory();

		/// <summary>
		/// Gets the <see cref="IPropertyFactory"/> creating <see cref="IProperty"/>s
		/// for the <see cref="IPresentation"/>
		/// </summary>
		/// <returns>The <see cref="IPropertyFactory"/></returns>
		IPropertyFactory getPropertyFactory();
	}
}
