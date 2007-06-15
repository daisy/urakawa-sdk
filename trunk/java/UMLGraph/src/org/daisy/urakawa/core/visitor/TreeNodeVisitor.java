package org.daisy.urakawa.core.visitor;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * A visitor specialized to tree structures, TreeNode in particular (this is why
 * it's called TreeNodeVisitor). This specifies the business logic action
 * associated to the tree traversal realized by VisitableTreeNode
 * implementations, via the calls to preVisit() and postVisit() methods before
 * and after recursive traversals of the children nodes, respectively.
 * </p>
 * <p>
 * Please refer to the Gang of Four book of Design Patterns for more details on
 * the Visitor pattern. More info: <a
 * href="http://www.patterndepot.com/put/8/JavaPatterns.htm">http://www.patterndepot.com/put/8/JavaPatterns.htm</a>
 * and <a
 * href="http://www.patterndepot.com/put/8/visitor.pdf">http://www.patterndepot.com/put/8/visitor.pdf</a>
 * </p>
 * 
 * @stereotype ApplicationImplemented
 */
public interface TreeNodeVisitor {
	/**
	 * <p>
	 * Method called before visiting children nodes of the given TreeNode.
	 * Implementations of this interface should define the business logic action
	 * to be taken for each traversed node.
	 * </p>
	 * 
	 * @param node
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void preVisit(TreeNode node) throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Method called after visiting children nodes of the given TreeNode.
	 * Implementations of this interface should define the business logic action
	 * to be taken for each traversed node.
	 * </p>
	 * 
	 * @param node
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void postVisit(TreeNode node) throws MethodParameterIsNullException;
}
