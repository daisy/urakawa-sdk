package org.daisy.urakawa.core;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.property.PropertyType;
import org.daisy.urakawa.core.visitor.CoreNodeVisitor;
import org.daisy.urakawa.core.visitor.VisitableCoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exceptions.NodeDoesNotExistException;
import org.daisy.urakawa.exceptions.PropertyTypeIsIllegalException;
import org.daisy.urakawa.exceptions.NodeIsInDifferentPresentationException;
import org.daisy.urakawa.exceptions.NodeIsAncestorException;
import org.daisy.urakawa.exceptions.NodeIsSelfException;
import org.daisy.urakawa.exceptions.NodeIsDescendantException;

import java.util.List;

/**
 * @depend - "Composition\n(children)" 0..n CoreNode
 * @depend - "Aggregation\n(parent)" 1 CoreNode
 * @depend - "Aggregation" 1 Presentation
 * @depend - Composition 1..n Property
 * @see CoreNodeFactory
 */
public class CoreNodeImpl implements CoreNode {
    public CoreNodeImpl(Presentation presentation) {
        try {
            setPresentation(presentation);
        } catch (MethodParameterIsNullException e) {
            e.printStackTrace();
        }
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
            if (childCoreNode != null) {
                try {
                    childCoreNode.acceptDepthFirst(visitor);
                } catch (MethodParameterIsNullException methodParameterIsNull) {
                    methodParameterIsNull.printStackTrace();
                }
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
    public void setParent(CoreNode node) {
        ;
    }

    /**
     * @hidden
     */
    public Presentation getPresentation() {
        return null;
    }

    /**
     * @hidden
     */
    public void setPresentation(CorePresentation presentation) throws MethodParameterIsNullException {
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
    public List getListOfUsedPropertyTypes() {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode copy(boolean deep) {
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
    public Property removeProperty(PropertyType type) throws PropertyTypeIsIllegalException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getParent() {
        return null;
    }

    /**
     * @hidden
     */
    public void insertBefore(CoreNode node, CoreNode anchorNode) throws MethodParameterIsNullException, NodeDoesNotExistException {
    }

    /**
     * @hidden
     */
    public void insertAfter(CoreNode node, CoreNode anchorNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void appendChild(CoreNode node) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public CoreNode getChild(int index) throws MethodParameterIsOutOfBoundsException {
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
    public int indexOf(CoreNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
        return 0;
    }

    /**
     * @hidden
     */
    public boolean isDescendantOf(CoreNode node) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean isAncestorOf(CoreNode node) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean isSiblingOf(CoreNode node) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public CoreNode copy(boolean deep, boolean copyProperties) {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getPreviousSibling() {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode getNextSibling() {
        return null;
    }

    /**
     * @hidden
     */
    public void removeChild(CoreNode node) throws NodeDoesNotExistException, MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public boolean swapWithPreviousSibling() {
        return false;
    }

    /**
     * @hidden
     */
    public boolean swapWithNextSibling() {
        return false;
    }

    /**
     * @hidden
     */
    public CoreNode splitChildren(int index, boolean copyProperties) throws MethodParameterIsOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNode removeChild(int index) throws MethodParameterIsOutOfBoundsException {
        return null;
    }

    /**
     * @hidden
     */
    public void replaceChild(CoreNode node, CoreNode oldNode) throws NodeDoesNotExistException, MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public CoreNode replaceChild(CoreNode node, int index) throws MethodParameterIsOutOfBoundsException, MethodParameterIsNullException {
        return null;
    }

    /**
     * @hidden
     */
    public void appendChildrenOf(CoreNode node) throws MethodParameterIsNullException, NodeIsInDifferentPresentationException, NodeIsAncestorException, NodeIsSelfException {
    }

    /**
     * @hidden
     */
    public void swapWith(CoreNode node) throws MethodParameterIsNullException, NodeIsInDifferentPresentationException, NodeIsAncestorException, NodeIsSelfException, NodeIsDescendantException {
    }

    /**
     * @hidden
     */
    public void insert(CoreNode node, int insertIndex) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException {
    }

    /**
     * @hidden
     */
    public CoreNode detach() {
        return null;
    }

    /**
     * @hidden
     */
    public void acceptBreadthFirst(CoreNodeVisitor visitor) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public boolean XukIn(XmlDataReader source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XukOut(XmlDataWriter destination) {
        return false;
    }

    /**
     * @hidden
     */
    public String getXukLocalName() {
        return null;
    }

    /**
     * @hidden
     */
    public String getXukNamespaceURI() {
        return null;
    }
}
