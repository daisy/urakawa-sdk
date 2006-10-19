package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exceptions.CoreNodeNotIncludedByNavigatorException;

/**
 * An extension of Navigator to determine what CoreNodes are part of the tree based on filtering/selection criteria implemented by isIncluded(node).
 */
public abstract class AbstractFilterNavigator implements Navigator {
    /**
     * @param node the node to check
     * @return true if the node is included in the resulting tree, based on the filtering/selection criteria implemented by this method.
     */
    public abstract boolean isIncluded(CoreNode node);

    /**
     * @hidden
     */
    public CoreNode getParent(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getPreviousSibling(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getNextSibling(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    /**
     * @hidden
     */
    public int getNumberOfChildren(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return 0;
    }

    /**
     * @hidden
     */
    public CoreNode getChild(CoreNode node, int index) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getPreviousInDepthFirstOrder(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getNextInDepthFirstOrder(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNodeIterator getDepthFirstOrderIterator(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }
}
