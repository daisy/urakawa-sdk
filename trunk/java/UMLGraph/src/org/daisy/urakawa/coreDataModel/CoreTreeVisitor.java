package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * A visitor specialized to tree structures,
 * in particular CoreNode (this is why it's called CoreTreeVisitor).
 * This specifies the business logic action associated to the tree traversal realized by
 * VisitableNode implementations, via the calls to preVisit()
 * and postVisit() methods before and after recursive traversals of the children nodes, respectively.
 * 
 * Please refer to the Gang of Four book of Design Patterns for more details on the Visitor pattern.
 * More info:
 * http://www.patterndepot.com/put/8/JavaPatterns.htm
 * http://www.patterndepot.com/put/8/visitor.pdf 
 */
public interface CoreTreeVisitor {
    /**
     * Method called before visiting children nodes of the given CoreNode.
     * Implementations of this interface should define the business logic action
     * to be taken for each traversed node.
     *
     * @param node cannot be null.
     */
    public void preVisit(CoreNode node) throws MethodParameterIsNullException;

    /**
     * Method called after visiting children nodes of the given CoreNode.
     * Implementations of this interface should define the business logic
     * action to be taken for each traversed node.
     *
     * @param node cannot be null.
     */
    public void postVisit(CoreNode node) throws MethodParameterIsNullException;
}
