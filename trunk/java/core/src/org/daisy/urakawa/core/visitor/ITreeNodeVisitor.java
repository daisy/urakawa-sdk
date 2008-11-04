package org.daisy.urakawa.core.visitor;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * A visitor specialized to tree structures, ITreeNode in particular (this is
 * why it's called ITreeNodeVisitor). This specifies the business logic action
 * associated to the tree traversal realized by IVisitableTreeNode
 * implementations, via the calls to preVisit() and postVisit() methods before
 * and after recursive traversals of the children nodes, respectively.
 * </p>
 * <p>
 * Please refer to the Gang of Four book of Design Patterns for more details on
 * the Visitor pattern. More info: <a
 * href="http://www.patterndepot.com/put/8/JavaPatterns.htm"
 * >http://www.patterndepot.com/put/8/JavaPatterns.htm</a> and <a
 * href="http://www.patterndepot.com/put/8/visitor.pdf"
 * >http://www.patterndepot.com/put/8/visitor.pdf</a>
 * </p>
 * 
 * @stereotype ApplicationImplemented
 */
public interface ITreeNodeVisitor
{
    /**
     * <p>
     * Method called before visiting children nodes of the given ITreeNode.
     * Implementations of this interface should define the business logic action
     * to be taken for each traversed node.
     * </p>
     * 
     * @param node
     *        cannot be null.
     * @return if true, the children will be visited
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public boolean preVisit(ITreeNode node)
            throws MethodParameterIsNullException;

    /**
     * <p>
     * Method called after visiting children nodes of the given ITreeNode.
     * Implementations of this interface should define the business logic action
     * to be taken for each traversed node.
     * </p>
     * 
     * @param node
     *        cannot be null.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public void postVisit(ITreeNode node) throws MethodParameterIsNullException;
}
