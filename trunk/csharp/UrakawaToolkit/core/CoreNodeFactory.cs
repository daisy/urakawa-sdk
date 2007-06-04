using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="TreeNodeFactory"/>.
	/// Creates <see cref="TreeNode"/>s belonging to a specific <see cref="ITreePresentation"/>
	/// </summary>
	/// <remarks>
	/// A <see cref="TreeNodeFactory"/> can not create <see cref="TreeNode"/>s
	/// until it has been associated with a <see cref="Presentation"/> using the
	/// <see cref="setPresentation"/> method
	/// </remarks>
	public class TreeNodeFactory
	{
		/// <summary>
		/// The <see cref="Presentation"/> to which any created <see cref="TreeNode"/>s belongs
    /// </summary>
    private Presentation mPresentation;

		/// <summary>
		/// Default constructor
		/// </summary>
		public TreeNodeFactory()
		{
		}

		#region TreeNodeFactory Members

		/// <summary>
		/// Creates a new <see cref="TreeNode"/>
		/// </summary>
		/// <returns>The new <see cref="TreeNode"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="ITreePresentation"/> of the 
		/// </exception>
		public TreeNode createNode()
		{
			return new TreeNode(getPresentation());
		}

		/// <summary>
		/// Creates a new <see cref="TreeNode"/> instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="TreeNode"/> or <c>null</c> if the QN</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when one of the parameters <paramref localName="localName"/> <paramref name="namespaceUri"/> and  is <c>null</c>
		/// </exception>
		public virtual TreeNode createNode(string localName, string namespaceUri)
		{
			if (localName == null || namespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the QName can be null");
			}
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "TreeNode":
						return createNode();
				}
			}
			return null;
		}


		/// <summary>
		/// Gets the <see cref="ITreePresentation"/> to which created nodes belong
		/// </summary>
		/// <returns>The <see cref="ITreePresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// When no <see cref="ITreePresentation"/> has yet been associated with <c>this</c>
		/// </exception>
		public Presentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"No core presentation has yet been associated with the core node factory");
			}
			return mPresentation;
		}


		/// <summary>
		/// Sets the <see cref="ITreePresentation"/> to which <see cref="TreeNode"/>s created by the factory belongs
		/// </summary>
		/// <param name="pres">The presentation</param>
		/// <remarks>This method should only be used during initialization</remarks>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when parameter <paramref localName="pres"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been set
		/// </exception>
		public void setPresentation(Presentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The presentation associated with a core node factory can not be null");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The presentation has already been set");
			}
			mPresentation = pres;
		}

		#endregion
	}
}
