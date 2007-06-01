package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.TreeNode;

/**
 * This can be replaced by Application developers:
 * - In Java: java.util.Iterator (if 1.5+ use generics, and the new Collection framework)
 * - In C#: System.Collections.Generic.IEnumerator<TreeNode>
 *
 * Example:
 *
 * TreeNodeIterator iterator;
 * iterator.reset();
 * while (iterator.hasNext()) {
 *     TreeNode node = iterator.getNext();
 *     // Do stuff
 * }
 */
public interface TreeNodeIterator {
    public TreeNode getNext();
    public boolean hasNext();
    public void reset();
}
