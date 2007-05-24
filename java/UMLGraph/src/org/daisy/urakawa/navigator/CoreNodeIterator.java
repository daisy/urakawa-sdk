package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.CoreNode;

/**
 * This can be replaced by Application developers:
 * - In Java: java.util.Iterator (if 1.5+ use generics, and the new Collection framework)
 * - In C#: System.Collections.Generic.IEnumerator<CoreNode>
 *
 * Example:
 *
 * CoreNodeIterator iterator;
 * iterator.reset();
 * while (iterator.hasNext()) {
 *     CoreNode node = iterator.getNext();
 *     // Do stuff
 * }
 */
public interface CoreNodeIterator {
    public CoreNode getNext();
    public boolean hasNext();
    public void reset();
}
