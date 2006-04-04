package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;
import org.daisy.urakawa.visitors.CoreNodeVisitor;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * Generally speaking, an end-user would not need to use this class directly.
 * They would just manipulate the corresponding abstract interface and use the provided
 * default factory implementation to create this class instances transparently.
 * -
 * However, this is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such (it's public after all),
 * or they can sub-class it in order to specialize their application.
 * -
 * In addition, an end-user would be able to implement its own factory
 * in order to create instances from its own implementations.
 *
 * @see CoreNodeFactory
 */
public class CoreNodeImpl implements CoreNode {
    /**
     * @hidden
     */
    public Presentation getPresentation() {
        return null;
    }

    /**
     * @hidden
     */
    public Property getProperty(PropertyType type) {
        return null;
    }

    /**
     * @hidden
     */
    public boolean setProperty(Property newProp) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public TreeNode getParent() {
        return null;
    }

    /**
     * @hidden
     */
    public void appendChild(TreeNode node) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void insertBefore(TreeNode node, TreeNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException {
    }

    /**
     * @hidden
     */
    public void insertAfter(TreeNode node, TreeNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public TreeNode getChild(int index) throws MethodParameterIsOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public int getChildCount() {
        return 0;
    }

    /**
     * @hidden
     */
    public int indexOf(TreeNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
        return 0;
    }

    /**
     * @hidden
     */
    public void removeChild(TreeNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public TreeNode removeChild(int index) throws MethodParameterIsOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public void appendChild(BasicTreeNode node) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void insertBefore(BasicTreeNode node, int anchorNodeIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException {
    }

    /**
     * @hidden
     */
    public void replaceChild(TreeNode node, TreeNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public TreeNode replaceChild(TreeNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException {
        return null;
    }

    /**
     * @hidden
     */
    public TreeNode detach() {
        return null;
    }

    /**
     * @hidden
     */
    public void acceptDepthFirst(CoreNodeVisitor visitor) throws MethodParameterIsNullException {
        try {
            visitor.preVisit(this);
        } catch (MethodParameterIsNullException methodParameterIsNull) {
            methodParameterIsNull.printStackTrace();
        }
        for (int i = 0; i < getChildCount(); i++) {
            VisitableCoreNode childCoreNode = null;
            try {
                childCoreNode = (VisitableCoreNode) getChild(i);
            } catch (MethodParameterIsOutOfBoundsException e) {
                e.printStackTrace();
            }
            try {
                childCoreNode.acceptDepthFirst(visitor);
            } catch (MethodParameterIsNullException methodParameterIsNull) {
                methodParameterIsNull.printStackTrace();
            }
        }
        try {
            visitor.postVisit(this);
        } catch (MethodParameterIsNullException methodParameterIsNull) {
            methodParameterIsNull.printStackTrace();
        }
    }

    /**
     * @hidden
     */
    public void acceptBreadthFirst(CoreNodeVisitor visitor) throws MethodParameterIsNullException {
    }
}
