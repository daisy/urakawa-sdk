package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;

/**
 * The implementation of the CoreNode interface.
 * Having the CoreNode be an interface leaves the model open for more than one possible CoreNode implementation.
 * Example code is given for the relevant parts that need to be made explicit for helping developers
 * apply the principles and benefits of such architecture.
 * The proposed implementation is incomplete and by no means definitive.
 */
class CoreNodeImpl implements CoreNode {

/**
 * Example of how to store children nodes of the current node, using a Java-based list class.
 */
private List mChildrenNodes = new ArrayList();

/**
 * Example implementation of depth-first traversal, see associated source code written in Java.
 * 
 * @param visitor 
 */
public void acceptDepthFirst(CoreTreeVisitor visitor) {        
    visitor.preVisit(this);
    for (int i = 0; i < getChildrenCount(); i++) {
        VisitableNode childNode = (VisitableNode) getChild(i);
        childNode.acceptDepthFirst(visitor);
    }
    visitor.postVisit(this);
} 

/**
 * Example implementation of how to get the number of children nodes,
 * see associated source code written in Java.
 * 
 * @return 
 */
public int getChildrenCount() {        
    return mChildrenNodes.size();
} 

/**
 * Example implementation of how to get a specific child node at the given index,
 * see associated source code written in Java.
 * 
 * @param i 
 * @return 
 */
public CoreNode getChild(int i) {        
    return (CoreNode) mChildrenNodes.get();
} 
}
