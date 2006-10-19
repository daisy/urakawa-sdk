package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exceptions.CoreNodeNotIncludedByNavigatorException;

/**
 * An extension of Navigator to determine what CoreNodes are part of the tree based on filtering/selection criteria implemented by isIncluded(node).
 */
public abstract class FilterNavigator implements Navigator {
    /**
     * @param node the node to check
     * @return true if the node is included in the resulting tree, based on the filtering/selection criteria implemented by this method.
     */
    public abstract boolean isIncluded(CoreNode node);

    public CoreNode getParent(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    public CoreNode getPreviousSibling(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    public CoreNode getNextSibling(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    public int getNumberOfChildren(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return 0;
    }

    public CoreNode getChild(CoreNode node, int index) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    public CoreNode getPreviousInDepthFirstOrder(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    public CoreNode getNextInDepthFirstOrder(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }

    public CoreNodeIterator getDepthFirstOrderIterator(CoreNode node) throws CoreNodeNotIncludedByNavigatorException {
        return null;
    }
}
