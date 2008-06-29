using System;
using System.Xml;
using urakawa.xuk;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="TreeNodeFactory"/>.
	/// Creates <see cref="TreeNode"/>s belonging to a specific <see cref="Presentation"/>
	/// </summary>
	/// <remarks>
	/// A <see cref="TreeNodeFactory"/> can not create <see cref="TreeNode"/>s
	/// until it has been associated with a <see cref="Presentation"/> using the
	/// <see cref="WithPresentation.Presentation"/> method
	/// </remarks>
	public class TreeNodeFactory : WithPresentation
	{

		/// <summary>
		/// Default constructor
		/// </summary>
		internal protected TreeNodeFactory()
		{
		}

		#region TreeNodeFactory Members

		/// <summary>
		/// Creates a new <see cref="TreeNode"/>
		/// </summary>
		/// <returns>The new <see cref="TreeNode"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> of the 
		/// </exception>
		public TreeNode CreateNode()
		{
			TreeNode newNode = new TreeNode();
			newNode.Presentation = Presentation;
			return newNode;
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
		public virtual TreeNode CreateNode(string localName, string namespaceUri)
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
						return CreateNode();
				}
			}
			return null;
		}

		#endregion

		

	}
}
