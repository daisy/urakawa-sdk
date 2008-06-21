package org.daisy.urakawa.core.visitor;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * A node that is traversable using the visitor pattern. Such a node can be
 * traversed (recursively) either depth or breadth-first. Each method must call
 * the preVisit() and postVisit() methods of the ITreeNodeVisitor before and
 * after traversing the children nodes, respectively. The IVisitableTreeNode
 * implementation only handles the tree traversal, the actual business logic
 * action associated to the node is handled by ITreeNodeVisitor implementations.
 * </p>
 * <p>
 * Please refer to the Gang of Four book of Design Patterns for more details on
 * the Visitor pattern. More info: <a
 * href="http://www.patterndepot.com/put/8/JavaPatterns.htm">http://www.patterndepot.com/put/8/JavaPatterns.htm</a>
 * and <a
 * href="http://www.patterndepot.com/put/8/visitor.pdf">http://www.patterndepot.com/put/8/visitor.pdf</a>
 * </p>
 */
public interface IVisitableTreeNode {
	/**
	 * <p>
	 * Depth-first traversal of the Node. Must call the preVisit() and
	 * postVisit() methods of the ITreeNodeVisitor before and after recursively
	 * traversing children, respectively.
	 * </p>
	 * 
	 * @param visitor
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void acceptDepthFirst(ITreeNodeVisitor visitor)
			throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Breadth-first traversal of the Node. Must call the preVisit() and
	 * postVisit() methods of the ITreeNodeVisitor before and after recursively
	 * traversing children, respectively. Usually trickier to implement than the
	 * more straight-forward depth-first traversal.
	 * </p>
	 * 
	 * @param visitor
	 *            cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void acceptBreadthFirst(ITreeNodeVisitor visitor)
			throws MethodParameterIsNullException;
}