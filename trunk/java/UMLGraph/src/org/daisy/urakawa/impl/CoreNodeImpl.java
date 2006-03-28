package org.daisy.urakawa.impl;

import org.daisy.urakawa.coreDataModel.CoreNode;
import org.daisy.urakawa.coreDataModel.CoreTreeVisitor;
import org.daisy.urakawa.coreDataModel.DOMNode;
import org.daisy.urakawa.coreDataModel.Presentation;
import org.daisy.urakawa.coreDataModel.Property;
import org.daisy.urakawa.coreDataModel.VisitableNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsValueOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;

import java.util.ArrayList;
import java.util.List;

/**
 * The implementation of the CoreNode interface.
 * Having the CoreNode be an interface leaves the model open for more than one possible CoreNode implementation.
 * Example code is given for the relevant parts that need to be made explicit for helping developers
 * apply the principles and benefits of such architecture.
 * The proposed implementation is incomplete and by no means definitive.
 */
public class CoreNodeImpl implements CoreNode {
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
        try {
            visitor.preVisit(this);
        } catch (MethodParameterIsNullException methodParameterIsNull) {
            methodParameterIsNull.printStackTrace();
        }
        for (int i = 0; i < getChildrenCount(); i++) {
            VisitableNode childNode = (VisitableNode) getChild(i);
            try {
                childNode.acceptDepthFirst(visitor);
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

    public void acceptBreadthFirst(CoreTreeVisitor visitor) throws MethodParameterIsNullException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    /**
     * Example implementation of how to get the number of children nodes,
     * see associated source code written in Java.
     *
     * @return zz
     */
    public int getChildrenCount() {
        return mChildrenNodes.size();
    }

    /**
     * Example implementation of how to get a specific child node at the given index,
     * see associated source code written in Java.
     *
     * @param i
     * @return zz
     */
    public DOMNode getChild(int i) {
        return (DOMNode) mChildrenNodes.get(i);
    }

    public Presentation getPresentation() {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public Property getProperty(Property.PropertyType type) {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public boolean setProperty(Property newProp) throws MethodParameterIsNullException {
        return false;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public Property removeProperty(Property.PropertyType type) {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public DOMNode getParent() {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void appendChild(DOMNode node) throws MethodParameterIsNullException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public void insertBefore(DOMNode node, DOMNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public void insertAfter(DOMNode node, DOMNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public int getChildCount() {
        return 0;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public int indexOf(DOMNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
        return 0;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void removeChild(DOMNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public DOMNode removeChild(int index) throws MethodParameterIsValueOutOfBoundsException {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void replaceChild(DOMNode node, DOMNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public DOMNode replaceChild(DOMNode node, int index) throws MethodParameterIsValueOutOfBoundsException, MethodParameterIsNullException {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public DOMNode detach() {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }
}
