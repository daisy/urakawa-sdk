package org.daisy.urakawa.core;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;
import org.daisy.urakawa.property.PropertyCannotBeAddedToTreeNodeException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNodeImpl extends WithPresentationImpl implements TreeNode {
	private List<Property> mProperties;
	private List<TreeNode> mChildren;
	private TreeNode mParent;

	/**
	 * 
	 */
	public TreeNodeImpl() {
		mProperties = new LinkedList<Property>();
		mChildren = new LinkedList<TreeNode>();
	}

	public void copyChildren(TreeNode destinationNode)
			throws MethodParameterIsNullException {
		if (destinationNode == null)
			throw new MethodParameterIsNullException();
		for (int i = 0; i < this.getChildCount(); i++) {
			destinationNode.appendChild(getChild(i).copy(true));
		}
	}

	public boolean hasProperties() {
		return (mProperties.size() > 0);
	}

	public boolean hasProperty(Property prop)
			throws MethodParameterIsNullException {
		if (prop == null)
			throw new MethodParameterIsNullException();
		return mProperties.contains(prop);
	}

	public <T extends Property> boolean hasProperties(Class<T> klass)
			throws MethodParameterIsNullException {
		if (klass == null)
			throw new MethodParameterIsNullException();
		for (Property p : getListOfProperties()) {
			if (p.getClass() == klass)
				return true;
		}
		return false;
	}

	public void removeProperty(Property prop)
			throws MethodParameterIsNullException {
		if (prop == null)
			throw new MethodParameterIsNullException();
		if (mProperties.contains(prop)) {
			mProperties.remove(prop);
			try {
				prop.setTreeNodeOwner(null);
			} catch (PropertyAlreadyHasOwnerException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (ObjectIsInDifferentPresentationException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public void removeProperties() {
		for (Property p : getListOfProperties()) {
			try {
				removeProperty(p);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public <T extends Property> List<T> removeProperties(Class<T> propType)
			throws MethodParameterIsNullException {
		if (propType == null) {
			throw new MethodParameterIsNullException();
		}
		List<T> remProps;
		remProps = getListOfProperties(propType);
		for (Property p : remProps) {
			removeProperty(p);
		}
		return remProps;
	}

	public <T extends Property> T getProperty(Class<T> klass)
			throws MethodParameterIsNullException {
		if (klass == null) {
			throw new MethodParameterIsNullException();
		}
		List<T> props = null;
		props = getListOfProperties(klass);
		if (props.size() > 0)
			return props.get(0);
		return null;
	}

	public <T extends Property> void addProperties(List<T> props)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException {
		if (props == null)
			throw new MethodParameterIsNullException();
		for (T p : props) {
			addProperty(p);
		}
	}

	public <T extends Property> void addProperty(T prop)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException {
		if (prop == null)
			throw new MethodParameterIsNullException();
		if (!mProperties.contains(prop)) {
			if (!prop.canBeAddedTo(this)) {
				throw new PropertyCannotBeAddedToTreeNodeException();
			}
			try {
				prop.setTreeNodeOwner(this);
			} catch (ObjectIsInDifferentPresentationException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			mProperties.add(prop);
		}
	}

	@SuppressWarnings("unchecked")
	public <T extends Property> List<Class<T>> getListOfUsedPropertyTypes() {
		List<Class<T>> res = new LinkedList<Class<T>>();
		for (Property p : (List<T>) getListOfProperties()) {
			if (!res.contains(p.getClass()))
				res.add((Class<T>) p.getClass());
		}
		return res;
	}

	@SuppressWarnings("unchecked")
	public <T extends Property> List<T> getListOfProperties() {
		List<T> list = new LinkedList<T>();
		list.addAll((List<T>) mProperties);
		return list;
	}

	@SuppressWarnings("unchecked")
	public <T extends Property> List<T> getListOfProperties(Class<T> klass)
			throws MethodParameterIsNullException {
		if (klass == null) {
			throw new MethodParameterIsNullException();
		}
		List<T> res = new LinkedList<T>();
		for (Property p : getListOfProperties()) {
			if (p.getClass() == klass)
				res.add((T) p);
		}
		return res;
	}

	public void acceptDepthFirst(TreeNodeVisitor visitor)
			throws MethodParameterIsNullException {
		if (visitor == null)
			throw new MethodParameterIsNullException();
		visitor.preVisit(this);
		for (int i = 0; i < getChildCount(); i++) {
			try {
				getChild(i).acceptDepthFirst(visitor);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		visitor.postVisit(this);
	}

	public void acceptBreadthFirst(TreeNodeVisitor visitor)
			throws MethodParameterIsNullException {
		if (visitor == null)
			throw new MethodParameterIsNullException();
		Queue<TreeNode> nodeQueue = new LinkedList<TreeNode>();
		nodeQueue.offer(this);
		while (nodeQueue.size() > 0) {
			TreeNode next = nodeQueue.poll();
			visitor.preVisit(next);
			for (int i = 0; i < next.getChildCount(); i++) {
				try {
					nodeQueue.offer(next.getChild(i));
				} catch (MethodParameterIsOutOfBoundsException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}
	}

	@Override
	public void clear() {
		for (TreeNode child : getListOfChildren()) {
			try {
				removeChild(child);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (TreeNodeDoesNotExistException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		for (Property prop : getListOfProperties()) {
			try {
				removeProperty(prop);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		super.clear();
	}

	private void xukInProperties(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null)
			throw new MethodParameterIsNullException();
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					Property newProp;
					try {
						newProp = getPresentation().getPropertyFactory()
								.createProperty(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (IsNotInitializedException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (newProp != null) {
						try {
							addProperty(newProp);
						} catch (PropertyCannotBeAddedToTreeNodeException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (PropertyAlreadyHasOwnerException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						newProp.xukIn(source);
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	private void xukInChildren(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null)
			throw new MethodParameterIsNullException();
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					TreeNode newChild;
					try {
						newChild = getPresentation().getTreeNodeFactory()
								.createNode(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (IsNotInitializedException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (newChild != null) {
						try {
							appendChild(newChild);
						} catch (ObjectIsInDifferentPresentationException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (TreeNodeHasParentException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (TreeNodeIsAncestorException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (TreeNodeIsSelfException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						newChild.xukIn(source);
					} else if (!source.isEmptyElement()) {
						// Read past unidentified element
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	public void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null)
			throw new MethodParameterIsNullException();
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
			readItem = true;
			String str = source.getLocalName();
			if (str == "mProperties") {
				xukInProperties(source);
			} else if (str == "mChildren") {
				xukInChildren(source);
			} else {
				readItem = false;
			}
		}
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();
		}
	}

	@Override
	public void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null)
			throw new MethodParameterIsNullException();
		destination.writeStartElement("mProperties", XukAbleImpl.XUK_NS);
		for (Property prop : getListOfProperties()) {
			prop.xukOut(destination, baseUri);
		}
		destination.writeEndElement();
		destination.writeStartElement("mChildren", XukAbleImpl.XUK_NS);
		for (int i = 0; i < this.getChildCount(); i++) {
			try {
				getChild(i).xukOut(destination, baseUri);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		destination.writeEndElement();
		super.xukOutChildren(destination, baseUri);
	}
}
