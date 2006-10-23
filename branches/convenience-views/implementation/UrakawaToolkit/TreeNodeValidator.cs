using System;
using urakawa.core;
using urakawa.core.property;

namespace urakawa.validation.core
{
	/// <summary>
	/// Summary description for TreeNodeValidator.
	/// </summary>
	public class TreeNodeValidator : ICoreNodeValidator
	{
		internal TreeNodeValidator()
		{
		}

		#region ICoreNodeValidator Members

		/// <summary>
		/// Determine if the given node can accept the new property
		/// </summary>
		/// <param name="newProp">The new property</param>
		/// <param name="contextNode">The node who should get this property</param>
		/// <returns></returns>
		public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
		{
			if (newProp == null)
			{
				throw new exception.MethodParameterIsNullException("Property cannot be null");
			}
			if (contextNode == null) 
			{
				throw new exception.MethodParameterIsNullException("Context node cannot be null");
			}

			return true;
		}
		
		/// <summary>
		/// Determine if the given node can be removed from the tree
		/// </summary>
		/// <param name="node">The child who should be removed</param>
		/// <returns></returns>
		public bool canRemoveChild(ICoreNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("Node cannot be null");
			}

			//the node's parent must be valid
			if (node.getParent() == null)
			{
				return false;
			}

			return true;
		}
		
		/// <summary>
		/// Determine if the given node can be inserted at the child index of the specified context (parent) node
		/// </summary>
		/// <param name="node">The new child node</param>
		/// <param name="insertIndex">The insertion index</param>
		/// <param name="contextNode">The parent node</param>
		/// <returns></returns>
		public bool canInsert(ICoreNode node, int insertIndex, ICoreNode contextNode)
		{
			if (contextNode == null)
			{
				throw new exception.MethodParameterIsNullException("Parent node cannot be null");
			}

			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("New child node cannot be null");
			}

			//the node must not have a parent already
			if (node.getParent() != null)
			{
				return false;
			}

			if (insertIndex <  0 || contextNode.getChildCount() < insertIndex)
			{
				throw new exception.MethodParameterIsOutOfBoundsException("Index is out of bounds");
			}

			return true;
		}

		/// <summary>
		/// Determine if the given node can be inserted before another
		/// </summary>
		/// <param name="node">The node to be inserted</param>
		/// <param name="anchorNode">The node after the insertion point (the new node's future sibling)</param>
		/// <returns></returns>
		public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("Node cannot be null");
			}

			if (anchorNode == null)
			{
				throw new exception.MethodParameterIsNullException("Anchor node cannot be null");
			}

			//the relative node's parent must be valid
			if (anchorNode.getParent() == null)
			{
				return false;
			}

			//the node must not have a parent already
			if (node.getParent() != null)
			{
				return false;
			}

			return true;
		}
		
		/// <summary>
		/// Determine if the given node can be inserted after the anchor node
		/// </summary>
		/// <param name="node">The node to be inserted</param>
		/// <param name="anchorNode">The node before the insertion point (the new node's future sibling)</param>
		/// <returns></returns>
		public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("Node cannot be null");
			}

			if (anchorNode == null)
			{
				throw new exception.MethodParameterIsNullException("Anchor node cannot be null");
			}

			//the relative node's parent must be valid
			if (anchorNode.getParent() == null)
			{
				return false;
			}

			//the node must not have a parent already
			if (node.getParent() != null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Determine if the given node can replace the old node
		/// </summary>
		/// <param name="node">The new child node</param>
		/// <param name="oldNode">The child node that is being replaced</param>
		/// <returns></returns>
		public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("New child node cannot be null");
			}

			if (oldNode == null)
			{
				throw new exception.MethodParameterIsNullException("Old child node cannot be null");
			}

			//the old node's parent must be valid
			if (oldNode.getParent() == null)
			{
				return false;
			}

			//the node must not have a parent already
			if (node.getParent() != null)
			{
				return false;
			}

			return true;
		}
		
		/// <summary>
		/// Determine if the given node can replace a child (specified by index) of the context node
		/// </summary>
		/// <param name="node">The new child node</param>
		/// <param name="index">The index of the child that is being replaced</param>
		/// <param name="contextNode">The parent</param>
		/// <returns></returns>
		public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
			if (contextNode == null)
			{
				throw new exception.MethodParameterIsNullException("Parent node cannot be null");
			}

			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("New child node cannot be null");
			}

			if (index <  0 || index > contextNode.getChildCount() - 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException("Index is out of bounds");
			}

			//the node we're trying to replace doesn't exist
			if (contextNode.getChild(index) == null)
			{
				throw new exception.NodeDoesNotExistException("The node to be replaced does not exist");
			}

			//the node must not have a parent already
			if (node.getParent() != null)
			{
				return false;
			}



			return true;
		}
		
		/// <summary>
		/// Determine if the node at the specified index can be removed from its parent
		/// </summary>
		/// <param name="index">Index at which to remove a child node</param>
		/// <param name="contextNode">The parent node</param>
		/// <returns></returns>
		public bool canRemoveChild(int index, ICoreNode contextNode)
		{
			if (contextNode == null)
			{
				throw new exception.MethodParameterIsNullException("Parent node cannot be null");
			}

			if (index <  0 || index > contextNode.getChildCount() - 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException("Index is out of bounds");
			}

			if (contextNode.getChild(index) == null)
			{
				throw new exception.NodeDoesNotExistException("Node to be removed does not exist");
			}

			return true;
		}

		/// <summary>
		/// Determine if the given node can be appended as a child of the context node
		/// </summary>
		/// <param name="node">The node to be appended</param>
		/// <param name="contextNode">The parent node</param>
		/// <returns></returns>
		public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
			if (contextNode == null)
			{
				throw new exception.MethodParameterIsNullException("Parent node cannot be null");
			}

			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("New child node cannot be null");
			}

			//the node must not have a parent already
			if (node.getParent() != null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Determine if the given node can be detached from its parent
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool canDetach(ICoreNode node)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("Node cannot be null");
			}

			//see if it's already detached
			if (node.getParent() == null)
			{
				return false;
			}

			return true;
		}

		#endregion
	}
}
