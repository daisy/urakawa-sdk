package org.daisy.urakawa.core.visitor;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * A node that is traversable using the visitor pattern.
 * Such a node can be traversed (recursively) either depth or breadth-first.
 * Each method must call the preVisit() and postVisit() methods of the TreeNodeVisitor
 * before and after traversing the children nodes, respectively.
 * The VisitableTreeNode implementation  only handles the tree traversal,
 * the actual business logic action associated to the node is handled
 * by TreeNodeVisitor implementations.
 * -
 * Please refer to the Gang of Four book of Design Patterns for more details on the Visitor pattern.
 * More info:
 * http://www.patterndepot.com/put/8/JavaPatterns.htm
 * http://www.patterndepot.com/put/8/visitor.pdf
 *
 * @depend - - - TreeNodeVisitor
 */
public interface VisitableTreeNode {
    /**
     * Depth-first traversal of the Node.
     * Must call the preVisit() and postVisit() methods of the TreeNodeVisitor
     * before and after recursively traversing children, respectively.
     *
     * @param visitor cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void acceptDepthFirst(TreeNodeVisitor visitor) throws MethodParameterIsNullException;

    /**
     * Breadth-first traversal of the Node. Must call the preVisit() and postVisit() methods
     * of the TreeNodeVisitor before and after recursively traversing children, respectively.
     * Usually trickier to implement than the more straight-forward depth-first traversal.
     *
     * @param visitor cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void acceptBreadthFirst(TreeNodeVisitor visitor) throws MethodParameterIsNullException;
}