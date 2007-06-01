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
	/**
	 * @hidden
	 */
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

	/**
	 * @hidden
	 */
	public void setParent(TreeNode node) {
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
	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
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
	public List<PropertyType> getListOfUsedPropertyTypes() {
		return null;
	}

	/**
	 * @hidden
	 */
	public boolean setProperty(Property newProp)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public Property removeProperty(PropertyType type)
			throws PropertyTypeIsIllegalException {
		return null;
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
	public void insertBefore(TreeNode node, TreeNode anchorNode)
			throws MethodParameterIsNullException, TreeNodeDoesNotExistException {
	}

	/**
	 * @hidden
	 */
	public void insertAfter(TreeNode node, TreeNode anchorNode)
			throws TreeNodeDoesNotExistException, MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public void appendChild(TreeNode node)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public TreeNode getChild(int index)
			throws MethodParameterIsOutOfBoundsException {
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
	public int indexOf(TreeNode node) throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException {
		return 0;
	}

	/**
	 * @hidden
	 */
	public boolean isDescendantOf(TreeNode node)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isAncestorOf(TreeNode node)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public boolean isSiblingOf(TreeNode node)
			throws MethodParameterIsNullException {
		return false;
	}

	/**
	 * @hidden
	 */
	public TreeNode copy(boolean deep, boolean copyProperties) {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getPreviousSibling() {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode getNextSibling() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void removeChild(TreeNode node) throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException {
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
	public TreeNode splitChildren(int index, boolean copyProperties)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	/**
	 * @hidden
	 */
	public TreeNode removeChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void replaceChild(TreeNode node, TreeNode oldNode)
			throws TreeNodeDoesNotExistException, MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public TreeNode replaceChild(TreeNode node, int index)
			throws MethodParameterIsOutOfBoundsException,
			MethodParameterIsNullException {
		return null;
	}

	/**
	 * @hidden
	 */
	public void appendChildrenOf(TreeNode node)
			throws MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	/**
	 * @hidden
	 */
	public void swapWith(TreeNode node) throws MethodParameterIsNullException,
			TreeNodeIsInDifferentPresentationException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException, TreeNodeIsDescendantException {
	}

	/**
	 * @hidden
	 */
	public void insert(TreeNode node, int insertIndex)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException {
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
	public void acceptBreadthFirst(TreeNodeVisitor visitor)
			throws MethodParameterIsNullException {
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

	/**
	 * @hidden
	 */
	public boolean ValueEquals(TreeNode other)
			throws MethodParameterIsNullException {
		return false;
	}
}
