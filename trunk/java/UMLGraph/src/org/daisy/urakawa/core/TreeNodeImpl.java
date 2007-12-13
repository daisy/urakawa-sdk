package org.daisy.urakawa.core;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.core.visitor.VisitableTreeNode;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;
import org.daisy.urakawa.property.PropertyCannotBeAddedToTreeNodeException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNodeImpl implements TreeNode {
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
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public void appendChildrenOf(TreeNode node)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeIsAncestorException, TreeNodeIsSelfException {
	}

	public TreeNode detach() {
		return null;
	}

	public void insert(TreeNode node, int insertIndex)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public void insertAfter(TreeNode node, TreeNode anchorNode)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public void insertBefore(TreeNode node, TreeNode anchorNode)
			throws MethodParameterIsNullException,
			TreeNodeDoesNotExistException,
			ObjectIsInDifferentPresentationException,
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
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
	}

	public TreeNode replaceChild(TreeNode node, int index)
			throws MethodParameterIsOutOfBoundsException,
			MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
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
			ObjectIsInDifferentPresentationException,
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

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public List<Property> getListOfProperties() {
		return null;
	}

	public List<TreeNode> getListOfChildren() {
		return null;
	}

	/**
	 * DateNode extends TreeNode { Date mDate; public override TreeNode
	 * export(Presentation destPres) throws FactoryCannotCreateTypeException {
	 * TreeNode newNode = super.export(destPres); if (! newNode instanceof
	 * this.getClass()) {throw new FactoryCannotCreateTypeException();} DateNode
	 * actualNode = (DateNode) newNode; actualNode.setDate(mDate); // etc...
	 * return actualNode; } } Presentation presA; Presentation presB; TreeNode
	 * importedNode = presB.getRootNode().export(presA);
	 * presA.setRootNode(importedNode); // OR:
	 * presA.getRootNode().appendChild(importedNode);
	 */
	public TreeNode export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		TreeNode destNode;
		try {
			destNode = destPres.getTreeNodeFactory().createNode(
					this.getXukLocalName(), this.getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			e.printStackTrace();
			return null;
		} catch (MethodParameterIsEmptyStringException e) {
			e.printStackTrace();
			return null;
		}
		if (destNode == null) {
			throw new FactoryCannotCreateTypeException();
		}
		List<Property> props = destNode.getListOfProperties();
		for (Property prop : props) {
			Property newProp;
			try {
				newProp = prop.export(destPres);
			} catch (MethodParameterIsNullException e1) {
				e1.printStackTrace();
				return null;
			}
			if (newProp == null) {
				return null;
			}
			try {
				destNode.addProperty(newProp);
			} catch (MethodParameterIsNullException e) {
				e.printStackTrace();
				return null;
			} catch (PropertyAlreadyHasOwnerException e) {
				e.printStackTrace();
				return null;
			}
		}
		for (TreeNode childNode : getListOfChildren()) {
			TreeNode childNodeEx = childNode.export(destPres);
			if (childNodeEx == null) {
				return null;
			}
			try {
				destNode.appendChild(childNodeEx);
			} catch (MethodParameterIsNullException e) {
				e.printStackTrace();
				return null;
			} catch (ObjectIsInDifferentPresentationException e) {
				e.printStackTrace();
				return null;
			} catch (TreeNodeHasParentException e) {
				e.printStackTrace();
				return null;
			} catch (TreeNodeIsAncestorException e) {
				e.printStackTrace();
				return null;
			} catch (TreeNodeIsSelfException e) {
				e.printStackTrace();
				return null;
			}
		}
		return destNode;
	}

	public void addProperties(List<Property> list)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException {
	}

	public void addProperty(Property prop)
			throws MethodParameterIsNullException,
			PropertyAlreadyHasOwnerException {
	}

	public List<Property> getListOfProperties(Class<Property> type)
			throws MethodParameterIsNullException {
		return null;
	}

	public Property getProperty(Class<Property> type)
			throws MethodParameterIsNullException {
		return null;
	}

	public boolean hasProperties(Class<Property> type)
			throws MethodParameterIsNullException {
		return false;
	}

	public boolean hasProperties() {
		return false;
	}

	public boolean hasProperty(Property prop)
			throws MethodParameterIsNullException {
		return false;
	}

	public List<Property> removeProperties(Class<Property> type)
			throws MethodParameterIsNullException {
		return null;
	}

	public List<Property> removeProperties() {
		return null;
	}

	public void removeProperty(Property prop)
			throws MethodParameterIsNullException {
	}

	public TreeNode getRoot() {
		return (getParent() != null ? getParent().getRoot() : this);
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}
}
