package org.daisy.urakawa.core;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * <p>
 * Read-only TreeNode methods.
 * </p>
 * 
 * @see org.daisy.urakawa.core.TreeNode
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Clone - org.daisy.urakawa.core.TreeNode
 */
public interface TreeNodeReadOnlyMethods {
	/**
	 * Creates a new TreeNode with identical content (recursively) as this node,
	 * but compatible with the given Presentation (factories, managers,
	 * channels, etc.). The process consist in browsing this node step by step,
	 * and creating copies with identical content, if possible (otherwise the
	 * factory exception is raised). If this TreeNode (or somewhere in its
	 * contents) is not compatible with the given destination Presentation (i.e.
	 * an attempt to create a copy using a factory with a given QName, fails),
	 * then the FactoryCannotCreateTypeException is raised.
	 * 
	 * @param destPres
	 *            the destination Presentation to which this node (and all its
	 *            content, recursively) should be exported.
	 * @return a new TreeNode with identical content (recursively) as this node,
	 *         but compatible with the given Presentation (factories, managers,
	 *         channels, etc.). cannot return null (in case of failure, the
	 *         exception is raised instead)
	 * @throws FactoryCannotCreateTypeException
	 *             if one of the factories in the given Presentation cannot
	 *             create a type based on a QName.
	 */
	public TreeNode export(Presentation destPres)
			throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Returns the the child TreeNode at the given index.
	 * </p>
	 * 
	 * @param index
	 *            must be in bounds: [0..children.size-1]
	 * @return cannot return null.
	 * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if index is out of bounds: [0..children.size-1]
	 */
	public TreeNode getChild(int index)
			throws MethodParameterIsOutOfBoundsException;

	/**
	 * @return the list of child TreeNodes
	 */
	public List<TreeNode> getListOfChildren();

	/**
	 * <p>
	 * Returns the parent of this TreeNode instance.
	 * </p>
	 * 
	 * @return returns NULL is this node is the root of the tree.
	 */
	public TreeNode getParent();

	/**
	 * <p>
	 * Returns the number of children TreeNode
	 * </p>
	 * 
	 * @return an integer greater than or equal to 0.
	 */
	public int getChildCount();

	/**
	 * <p>
	 * Returns the index of a given child TreeNode.
	 * </p>
	 * 
	 * @param node
	 *            cannot be null, must exist as a child
	 * @return an integer greater than or equal to 0.
	 * @tagvalue Exceptions "NodeDoesNotExist-MethodParameterIsNull"
	 * @throws TreeNodeDoesNotExistException
	 *             if the given node does not exist as a child
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public int indexOf(TreeNode node) throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException;

	/**
	 * <p>
	 * Tests if this node is a descendant of the given node.
	 * </p>
	 * 
	 * @param node
	 *            cannot be null;
	 * @return true if this is descendant of the node given as an argument, or
	 *         if this == node.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean isDescendantOf(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Tests if this node is an ancestor of the given node.
	 * </p>
	 * 
	 * @param node
	 *            cannot be null;
	 * @return true if this is ancestor of the node given as an argument, or if
	 *         this == node.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean isAncestorOf(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Tests if this node is a sibling of the given node.
	 * </p>
	 * 
	 * @param node
	 *            cannot be null;
	 * @return true if this is sibling of passed parameter node, or if this ==
	 *         node.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean isSiblingOf(TreeNode node)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Performs a copy of this node.
	 * </p>
	 * 
	 * @param deep
	 *            if true, the full tree fragment is copied and returned,
	 *            including children of children, etc. recursively. Otherwise,
	 *            just [this] node without any children.
	 * @param copyProperties
	 *            if true, attached Property objects are copied as well.
	 * @return a copy of this node, which is in the same Presentation instance.
	 */
	public TreeNode copy(boolean deep, boolean copyProperties);

	/**
	 * <p>
	 * Returns the previous sibling of this node.
	 * </p>
	 * 
	 * @return can be NULL if this node is the first child, or the tree root.
	 */
	public TreeNode getPreviousSibling();

	/**
	 * <p>
	 * Returns the next sibling of this node.
	 * </p>
	 * 
	 * @return can be NULL if this node is the last child, or the tree root.
	 */
	public TreeNode getNextSibling();
}
