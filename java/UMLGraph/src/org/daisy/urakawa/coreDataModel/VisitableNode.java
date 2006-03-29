package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * A node that is traversable using the visitor pattern.
 * Such a node can be traversed (recursively) either depth or breadth-first.
 * Each method must call the preVisit() and postVisit() methods of the CoreTreeVisitor
 * before and after traversing the children nodes, respectively.
 * The VisitableNode implementation  only handles the tree traversal,
 * the actual business logic action associated to the node is handled
 * by CoreTreeVisitor implementations.
 * 
 * Please refer to the Gang of Four book of Design Patterns for more details on the Visitor pattern.
 * More info:
 * http://www.patterndepot.com/put/8/JavaPatterns.htm
 * http://www.patterndepot.com/put/8/visitor.pdf 
 */
public interface VisitableNode {
    /**
     * Depth-first traversal of the Node.
     * Must call the preVisit() and postVisit() methods of the CoreTreeVisitor
     * before and after recursively traversing children, respectively.
     *
     * @param visitor cannot be null.
     */
    public void acceptDepthFirst(CoreTreeVisitor visitor) throws MethodParameterIsNullException;

    /**
     * Breadth-first traversal of the Node. Must call the preVisit() and postVisit() methods
     * of the CoreTreeVisitor before and after recursively traversing children, respectively.
     * Usually trickier to implement than the more straight-forward depth-first traversal.
     *
     * @param visitor cannot be null.
     */
    public void acceptBreadthFirst(CoreTreeVisitor visitor) throws MethodParameterIsNullException;
}