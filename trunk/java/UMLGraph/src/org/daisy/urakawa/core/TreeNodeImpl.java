package org.daisy.urakawa.core;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.core.property.Property;
import org.daisy.urakawa.core.property.PropertyType;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.core.visitor.VisitableTreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

import java.util.List;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNodeImpl implements TreeNode {
	public List<PropertyType> getListOfUsedPropertyTypes() {
		return null;
	}

	public Property getProperty(PropertyType type)
			throws MethodParameterIsNullException {
		return null;
	}

	public Property removeProperty(PropertyType type)
			throws PropertyTypeIsIllegalException,
			MethodParameterIsNullException {
		return null;
	}

	public boolean setProperty(Property newProp)
			throws MethodParameterIsNullException {
		return false;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	public TreeNode copy(boolean deep, boolean copyProperties) {
		return null;
	}

	public TreeNode getChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	public int getChildCount() {
		return 0;
	}

	public TreeNode getNextSibling() {
		return null;
	}

	public TreeNode getParent() {
		return null;
	}

	public TreeNode getPreviousSibling() {
		return null;
	}

	public int indexOf(TreeNode node) throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException {
		return 0;
	}

	public boolean isAncestorOf(TreeNode node)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isDescendantOf(TreeNode node)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean isSiblingOf(TreeNode node)
			throws MethodParameterIsNullException {
		return false;
	}

	public void appendChild(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public void appendChildrenOf(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeIsAncestorException, TreeNodeIsSelfException {
	}

	public TreeNode detach() {
		return null;
	}

	public void insert(TreeNode node, int insertIndex)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public void insertAfter(TreeNode node, TreeNode anchorNode)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public void insertBefore(TreeNode node, TreeNode anchorNode)
			throws MethodParameterIsNullException,
			TreeNodeDoesNotExistException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public TreeNode removeChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	public void removeChild(TreeNode node)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException {
	}

	public void replaceChild(TreeNode node, TreeNode oldNode)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public TreeNode replaceChild(TreeNode node, int index)
			throws MethodParameterIsOutOfBoundsException,
			MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
		return null;
	}

	public void setParent(TreeNode node) {
	}

	public TreeNode splitChildren(int index, boolean copyProperties)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	public void swapWith(TreeNode node) throws MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException,
			TreeNodeIsAncestorException, TreeNodeIsSelfException,
			TreeNodeIsDescendantException, TreeNodeHasNoParentException {
	}

	public boolean swapWithNextSibling() {
		return false;
	}

	public boolean swapWithPreviousSibling() {
		return false;
	}

	public void acceptBreadthFirst(TreeNodeVisitor visitor)
			throws MethodParameterIsNullException {
	}

	public void acceptDepthFirst(TreeNodeVisitor visitor)
			throws MethodParameterIsNullException {
		try {
			visitor.preVisit(this);
		} catch (MethodParameterIsNullException methodParameterIsNull) {
			methodParameterIsNull.printStackTrace();
		}
		for (int i = 0; i < getChildCount(); i++) {
			VisitableTreeNode childTreeNode = null;
			try {
				childTreeNode = (VisitableTreeNode) getChild(i);
			} catch (MethodParameterIsOutOfBoundsException e) {
				e.printStackTrace();
			}
			if (childTreeNode != null) {
				try {
					childTreeNode.acceptDepthFirst(visitor);
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

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		return false;
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(TreeNode other)
			throws MethodParameterIsNullException {
		return false;
	}
}
