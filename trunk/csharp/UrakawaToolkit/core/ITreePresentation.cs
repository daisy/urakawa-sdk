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
	public interface ITreePresentation : events.ITreeNodeChangedEventManager, IXukAble
	{
		/// <summary>
		/// Gets the root <see cref="TreeNode"/> of the presentation
		/// </summary>
		/// <returns>The root <see cref="TreeNode"/></returns>
		TreeNode getRootNode();

		/// <summary>
		/// Sets the root <see cref="TreeNode"/> of the presentation
		/// </summary>
		/// <param name="newRoot">The new root <see cref="TreeNode"/> or <c>null</c></param>
		void setRootNode(TreeNode newRoot);

		/// <summary>
		/// Gets the factory creating <see cref="TreeNode"/>s
		/// for the presentation
		/// </summary>
		/// <returns>The core node factory</returns>
		TreeNodeFactory getTreeNodeFactory();

		/// <summary>
		/// Gets the factory creating <see cref="Property"/>s
		/// for the presentation
		/// </summary>
		/// <returns>The property factory</returns>
		IGenericPropertyFactory getPropertyFactory();
	}
}
