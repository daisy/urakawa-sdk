package org.daisy.urakawa.core.visitor;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * A visitor specialized to tree structures,
 * in particular TreeNode (this is why it's called TreeNodeVisitor).
 * This specifies the business logic action associated to the tree traversal realized by
 * VisitableTreeNode implementations, via the calls to preVisit()
 * and postVisit() methods before and after recursive traversals of the children nodes, respectively.
 * -
 * Please refer to the Gang of Four book of Design Patterns for more details on the Visitor pattern.
 * More info:
 * http://www.patterndepot.com/put/8/JavaPatterns.htm
 * http://www.patterndepot.com/put/8/visitor.pdf
 *
 * @stereotype ApplicationImplemented
 */
public interface TreeNodeVisitor {
    /**
     * Method called before visiting children nodes of the given TreeNode.
     * Implementations of this interface should define the business logic action
     * to be taken for each traversed node.
     *
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
     */
    public void preVisit(TreeNode node) throws MethodParameterIsNullException;

    /**
     * Method called after visiting children nodes of the given TreeNode.
     * Implementations of this interface should define the business logic
     * action to be taken for each traversed node.
     *
     * @param node cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
     */
    public void postVisit(TreeNode node) throws MethodParameterIsNullException;
}
