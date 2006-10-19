package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exceptions.CoreNodeNotIncludedByFilterNavigatorException;

/**
 * This navigator allows to navigate a read-only virtual tree,
 * which node positions are managed by the few navigation methods specified here.
 * Nodes of the original CoreNode tree are filtered using the isIncluded() method, which has to be overriden.
 * As a result, any node passed as a parameter of the navigation methods in this specification MUST be authorized by the filter,
 * otherwise the CoreNodeNotIncludedByFilterNavigatorException is raised.
 */
public interface FilterNavigator {
    public boolean isIncluded(CoreNode node);

    /**
     * @param node the base corenode for which to return the parent, according to the position in the virtual, filtered tree.
     * @return the parent of the given corenode, relative to the virtual filtered tree represented by this navigator.
     * @throws CoreNodeNotIncludedByFilterNavigatorException the given node must pass the isIncluded(node) test.
     * @tagvalue Exceptions "CoreNodeNotIncludedByFilterNavigator"
     */
    public CoreNode getParent(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNode getPreviousSibling(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNode getNextSibling(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNode getNumberOfChildren(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNode getChildAt(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNode getPreviousInDepthFirstOrder(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNode getNextInDepthFirstOrder(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;

    public CoreNodeIterator getDepthFirstOrderIterator(CoreNode node) throws CoreNodeNotIncludedByFilterNavigatorException;
}
