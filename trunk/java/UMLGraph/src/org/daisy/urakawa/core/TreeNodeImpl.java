package org.daisy.urakawa.core;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.event.ChangeListener;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.ChangeNotifierImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.LanguageChangedEvent;
import org.daisy.urakawa.event.core.ChildAddedEvent;
import org.daisy.urakawa.event.core.ChildRemovedEvent;
import org.daisy.urakawa.event.core.PropertyAddedEvent;
import org.daisy.urakawa.event.core.PropertyRemovedEvent;
import org.daisy.urakawa.event.core.TreeNodeEvent;
import org.daisy.urakawa.event.property.PropertyEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;
import org.daisy.urakawa.property.PropertyCannotBeAddedToTreeNodeException;
import org.daisy.urakawa.xuk.XukAble;
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
	protected ChangeNotifier<DataModelChangedEvent> mGenericEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mTreeNodeEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mPropertyEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mChildAddedEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mChildRemovedEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mPropertyAddedEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mPropertyRemovedEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mLanguageChangedEventNotifier = new ChangeNotifierImpl();

	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (LanguageChangedEvent.class.isAssignableFrom(event.getClass())) {
			mLanguageChangedEventNotifier.notifyListeners(event);
		}
		if (PropertyAddedEvent.class.isAssignableFrom(event.getClass())) {
			mPropertyAddedEventNotifier.notifyListeners(event);
		}
		if (PropertyRemovedEvent.class.isAssignableFrom(event.getClass())) {
			mPropertyRemovedEventNotifier.notifyListeners(event);
		}
		if (PropertyEvent.class.isAssignableFrom(event.getClass())) {
			mPropertyEventNotifier.notifyListeners(event);
		}
		if (ChildAddedEvent.class.isAssignableFrom(event.getClass())) {
			mChildAddedEventNotifier.notifyListeners(event);
		}
		if (ChildRemovedEvent.class.isAssignableFrom(event.getClass())) {
			mChildRemovedEventNotifier.notifyListeners(event);
		}
		if (TreeNodeEvent.class.isAssignableFrom(event.getClass())) {
			mTreeNodeEventNotifier.notifyListeners(event);
		}
		mGenericEventNotifier.notifyListeners(event);
	}

	public <K extends DataModelChangedEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (LanguageChangedEvent.class.isAssignableFrom(klass)) {
			mLanguageChangedEventNotifier.registerListener(listener, klass);
		}
		if (PropertyAddedEvent.class.isAssignableFrom(klass)) {
			mPropertyAddedEventNotifier.registerListener(listener, klass);
		} else if (PropertyRemovedEvent.class.isAssignableFrom(klass)) {
			mPropertyRemovedEventNotifier.registerListener(listener, klass);
		} else if (PropertyEvent.class.isAssignableFrom(klass)) {
			mPropertyEventNotifier.registerListener(listener, klass);
		}
		if (ChildAddedEvent.class.isAssignableFrom(klass)) {
			mChildAddedEventNotifier.registerListener(listener, klass);
		} else if (ChildRemovedEvent.class.isAssignableFrom(klass)) {
			mChildRemovedEventNotifier.registerListener(listener, klass);
		} else if (TreeNodeEvent.class.isAssignableFrom(klass)) {
			mTreeNodeEventNotifier.registerListener(listener, klass);
		}
		mGenericEventNotifier.registerListener(listener, klass);
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (LanguageChangedEvent.class.isAssignableFrom(klass)) {
			mLanguageChangedEventNotifier.unregisterListener(listener, klass);
		}
		if (PropertyAddedEvent.class.isAssignableFrom(klass)) {
			mPropertyAddedEventNotifier.unregisterListener(listener, klass);
		} else if (PropertyRemovedEvent.class.isAssignableFrom(klass)) {
			mPropertyRemovedEventNotifier.unregisterListener(listener, klass);
		} else if (PropertyEvent.class.isAssignableFrom(klass)) {
			mPropertyEventNotifier.unregisterListener(listener, klass);
		}
		if (ChildAddedEvent.class.isAssignableFrom(klass)) {
			mChildAddedEventNotifier.unregisterListener(listener, klass);
		} else if (ChildRemovedEvent.class.isAssignableFrom(klass)) {
			mChildRemovedEventNotifier.unregisterListener(listener, klass);
		} else if (TreeNodeEvent.class.isAssignableFrom(klass)) {
			mTreeNodeEventNotifier.unregisterListener(listener, klass);
		}
		mGenericEventNotifier.unregisterListener(listener, klass);
	}

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_LanguageChangedEventListener(LanguageChangedEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<LanguageChangedEvent> mLanguageChangedEventListener = new ChangeListener<LanguageChangedEvent>() {
		@Override
		public <K extends LanguageChangedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_LanguageChangedEventListener(event);
		}
	};

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_ChildAddedEventListener(ChildAddedEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<ChildAddedEvent> mChildAddedEventListener = new ChangeListener<ChildAddedEvent>() {
		@Override
		public <K extends ChildAddedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_ChildAddedEventListener(event);
		}
	};

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_ChildRemovedEventListener(ChildRemovedEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<ChildRemovedEvent> mChildRemovedEventListener = new ChangeListener<ChildRemovedEvent>() {
		@Override
		public <K extends ChildRemovedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_ChildRemovedEventListener(event);
		}
	};

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_PropertyAddedEventListener(PropertyAddedEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<PropertyAddedEvent> mPropertyAddedEventListener = new ChangeListener<PropertyAddedEvent>() {
		@Override
		public <K extends PropertyAddedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_PropertyAddedEventListener(event);
		}
	};

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_PropertyRemovedEventListener(PropertyRemovedEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<PropertyRemovedEvent> mPropertyRemovedEventListener = new ChangeListener<PropertyRemovedEvent>() {
		@Override
		public <K extends PropertyRemovedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_PropertyRemovedEventListener(event);
		}
	};

	/**
	 * 
	 */
	public TreeNodeImpl() {
		mProperties = new LinkedList<Property>();
		mChildren = new LinkedList<TreeNode>();
		try {
			mLanguageChangedEventNotifier.registerListener(
					mLanguageChangedEventListener, LanguageChangedEvent.class);
			mChildAddedEventNotifier.registerListener(mChildAddedEventListener,
					ChildAddedEvent.class);
			mChildRemovedEventNotifier.registerListener(
					mChildRemovedEventListener, ChildRemovedEvent.class);
			mPropertyAddedEventNotifier.registerListener(
					mPropertyAddedEventListener, PropertyAddedEvent.class);
			mPropertyRemovedEventNotifier.registerListener(
					mPropertyRemovedEventListener, PropertyRemovedEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void copyChildren(TreeNode destinationNode)
			throws MethodParameterIsNullException {
		if (destinationNode == null)
			throw new MethodParameterIsNullException();
		for (int i = 0; i < this.getChildCount(); i++) {
			try {
				destinationNode.appendChild(getChild(i).copy(true));
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
			notifyListeners(new PropertyRemovedEvent(this, prop));
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
			notifyListeners(new PropertyAddedEvent(this, prop));
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
		if (source.getNamespaceURI() == XukAble.XUK_NS) {
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
		destination.writeStartElement("mProperties", XukAble.XUK_NS);
		for (Property prop : getListOfProperties()) {
			prop.xukOut(destination, baseUri);
		}
		destination.writeEndElement();
		destination.writeStartElement("mChildren", XukAble.XUK_NS);
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

	public int indexOf(TreeNode node) throws MethodParameterIsNullException,
			TreeNodeDoesNotExistException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mChildren.contains(node)) {
			throw new TreeNodeDoesNotExistException();
		}
		return mChildren.indexOf(node);
	}

	public TreeNode getChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || mChildren.size() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		return mChildren.get(index);
	}

	public TreeNode getParent() {
		return mParent;
	}

	public int getChildCount() {
		return mChildren.size();
	}

	public List<TreeNode> getListOfChildren() {
		return new LinkedList<TreeNode>(mChildren);
	}

	protected void copyProperties(TreeNode destinationNode)
			throws MethodParameterIsNullException {
		if (destinationNode == null) {
			throw new MethodParameterIsNullException();
		}
		for (Property prop : getListOfProperties()) {
			try {
				destinationNode.addProperty(prop.copy());
			} catch (PropertyCannotBeAddedToTreeNodeException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (PropertyAlreadyHasOwnerException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (FactoryCannotCreateTypeException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	protected TreeNode copyProtected(boolean deep, boolean inclProperties) {
		TreeNode theCopy;
		try {
			theCopy = getPresentation().getTreeNodeFactory().createNode(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (inclProperties) {
			try {
				copyProperties(theCopy);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		if (deep) {
			try {
				copyChildren(theCopy);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return theCopy;
	}

	public TreeNode copy(boolean deep, boolean inclProperties) {
		return copyProtected(deep, inclProperties);
	}

	public TreeNode copy(boolean deep) {
		return copy(deep, true);
	}

	public TreeNode copy() {
		return copy(true, true);
	}

	public TreeNode export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return exportProtected(destPres);
	}

	protected TreeNode exportProtected(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		TreeNode exportedNode;
		try {
			exportedNode = destPres.getTreeNodeFactory().createNode(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (exportedNode == null) {
			throw new FactoryCannotCreateTypeException();
		}
		for (Property prop : getListOfProperties()) {
			try {
				exportedNode.addProperty(prop.export(destPres));
			} catch (PropertyCannotBeAddedToTreeNodeException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (PropertyAlreadyHasOwnerException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		for (TreeNode child : getListOfChildren()) {
			try {
				exportedNode.appendChild(child.export(destPres));
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
		}
		return exportedNode;
	}

	public TreeNode getNextSibling() {
		TreeNode p = getParent();
		if (p == null)
			return null;
		int i;
		try {
			i = p.indexOf(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (i + 1 >= p.getChildCount())
			return null;
		try {
			return p.getChild(i + 1);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public TreeNode getPreviousSibling() {
		TreeNode p = getParent();
		if (p == null)
			return null;
		int i;
		try {
			i = p.indexOf(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (i == 0)
			return null;
		try {
			return p.getChild(i - 1);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public boolean isSiblingOf(TreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		TreeNode p = getParent();
		return (p != null && p == node.getParent());
	}

	public boolean isAncestorOf(TreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		TreeNode p = getParent();
		if (p == null) {
			return false;
		} else if (p == node) {
			return true;
		} else {
			return p.isAncestorOf(node);
		}
	}

	public boolean isDescendantOf(TreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		return node.isAncestorOf(this);
	}

	public void insert(TreeNode node, int insertIndex)
			throws MethodParameterIsNullException, TreeNodeHasParentException,
			MethodParameterIsOutOfBoundsException,
			ObjectIsInDifferentPresentationException,
			TreeNodeIsAncestorException, TreeNodeIsSelfException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (node.getParent() != null) {
			throw new TreeNodeHasParentException();
		}
		if (node == this) {
			throw new TreeNodeIsSelfException();
		}
		if (node.isAncestorOf(this)) {
			throw new TreeNodeIsAncestorException();
		}
		if (insertIndex < 0 || mChildren.size() < insertIndex) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		try {
			if (node.getPresentation() != getPresentation()) {
				throw new ObjectIsInDifferentPresentationException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		mChildren.add(insertIndex, node);
		node.setParent(this);
		try {
			getPresentation().notifyListeners(new ChildAddedEvent(this, node));
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		notifyListeners(new ChildAddedEvent(this, node));
	}

	public TreeNode detach() {
		try {
			mParent.removeChild(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return this;
	}

	public TreeNode removeChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		TreeNode removedChild = getChild(index);
		removedChild.setParent(null);
		mChildren.remove(index);
		try {
			getPresentation().notifyListeners(
					new ChildRemovedEvent(this, removedChild, index));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			notifyListeners(new ChildRemovedEvent(this, removedChild, index));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return removedChild;
	}

	public TreeNode removeChild(TreeNode node)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		int index = indexOf(node);
		try {
			return removeChild(index);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void insertBefore(TreeNode node, TreeNode anchorNode)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
		if (node == null || anchorNode == null) {
			throw new MethodParameterIsNullException();
		}
		if (node.getParent() != null) {
			throw new TreeNodeHasParentException();
		}
		if (node == this) {
			throw new TreeNodeIsSelfException();
		}
		if (node.isAncestorOf(this)) {
			throw new TreeNodeIsAncestorException();
		}
		int index = indexOf(anchorNode);
		try {
			insert(node, index);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void insertAfter(TreeNode node, TreeNode anchorNode)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
		if (node == null || anchorNode == null) {
			throw new MethodParameterIsNullException();
		}
		if (node.getParent() != null) {
			throw new TreeNodeHasParentException();
		}
		if (node == this) {
			throw new TreeNodeIsSelfException();
		}
		if (node.isAncestorOf(this)) {
			throw new TreeNodeIsAncestorException();
		}
		int index = indexOf(anchorNode) + 1;
		try {
			insert(node, index);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public TreeNode replaceChild(TreeNode node, int index)
			throws MethodParameterIsOutOfBoundsException,
			MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (node.getParent() != null) {
			throw new TreeNodeHasParentException();
		}
		if (node == this) {
			throw new TreeNodeIsSelfException();
		}
		if (node.isAncestorOf(this)) {
			throw new TreeNodeIsAncestorException();
		}
		TreeNode replacedChild = getChild(index);
		insert(node, index);
		replacedChild.detach();
		return replacedChild;
	}

	public TreeNode replaceChild(TreeNode node, TreeNode oldNode)
			throws TreeNodeDoesNotExistException,
			MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
		try {
			return replaceChild(node, indexOf(oldNode));
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void appendChild(TreeNode node)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeHasParentException, TreeNodeIsAncestorException,
			TreeNodeIsSelfException {
		try {
			insert(node, getChildCount());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void appendChildrenOf(TreeNode node)
			throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeIsAncestorException, TreeNodeIsSelfException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			if (getPresentation() != node.getPresentation()) {
				throw new ObjectIsInDifferentPresentationException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (node == this) {
			throw new TreeNodeIsSelfException();
		}
		if (node.isAncestorOf(this)) {
			throw new TreeNodeIsAncestorException();
		}
		while (node.getChildCount() > 0) {
			try {
				appendChild(node.removeChild(0));
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (TreeNodeHasParentException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public void swapWith(TreeNode node) throws MethodParameterIsNullException,
			ObjectIsInDifferentPresentationException,
			TreeNodeIsAncestorException, TreeNodeIsSelfException,
			TreeNodeIsDescendantException, TreeNodeHasNoParentException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (getParent() == null || node.getParent() == null) {
			throw new TreeNodeHasNoParentException();
		}
		try {
			if (getPresentation() != node.getPresentation()) {
				throw new ObjectIsInDifferentPresentationException();
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (node == this) {
			throw new TreeNodeIsSelfException();
		}
		if (isAncestorOf(node)) {
			throw new TreeNodeIsAncestorException();
		}
		if (isDescendantOf(node)) {
			throw new TreeNodeIsDescendantException();
		}
		TreeNode thisParent = getParent();
		int thisIndex;
		try {
			thisIndex = thisParent.indexOf(this);
		} catch (TreeNodeDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		detach();
		TreeNode nodeParent = node.getParent();
		try {
			nodeParent.insertAfter(this, node);
		} catch (TreeNodeDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeHasParentException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			thisParent.insert(node, thisIndex);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeHasParentException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public TreeNode splitChildren(int index, boolean copyProperties)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || getChildCount() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		TreeNode res = copy(false, copyProperties);
		while (index < getChildCount()) {
			try {
				res.appendChild(removeChild(index));
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
		}
		return res;
	}

	public boolean swapWithPreviousSibling() {
		TreeNode nextSibling = getNextSibling();
		if (nextSibling == null)
			return false;
		try {
			swapWith(nextSibling);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (ObjectIsInDifferentPresentationException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeIsAncestorException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeIsSelfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeIsDescendantException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeHasNoParentException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return true;
	}

	public boolean swapWithNextSibling() {
		TreeNode prevSibling = getPreviousSibling();
		if (prevSibling == null)
			return false;
		try {
			swapWith(prevSibling);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (ObjectIsInDifferentPresentationException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeIsAncestorException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeIsSelfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeIsDescendantException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TreeNodeHasNoParentException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return true;
	}

	public boolean ValueEquals(TreeNode other)
			throws MethodParameterIsNullException {
		if (other == null)
			throw new MethodParameterIsNullException();
		if (other.getClass() != this.getClass())
			return false;
		List<Class<Property>> thisProps = getListOfUsedPropertyTypes();
		List<Class<Property>> otherProps = other.getListOfUsedPropertyTypes();
		if (thisProps.size() != otherProps.size())
			return false;
		for (Class<Property> pt : thisProps) {
			List<Property> thisPs = getListOfProperties(pt);
			List<Property> otherPs = other.getListOfProperties(pt);
			if (thisPs.size() != otherPs.size())
				return false;
			for (int i = 0; i < thisPs.size(); i++) {
				if (!thisPs.get(i).ValueEquals(otherPs.get(i)))
					return false;
			}
		}
		if (getChildCount() != other.getChildCount())
			return false;
		for (int i = 0; i < getChildCount(); i++) {
			try {
				if (!getChild(i).ValueEquals(other.getChild(i)))
					return false;
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return true;
	}

	public TreeNode getRoot() {
		TreeNode parent = getParent();
		if (parent == null) {
			return this;
		} else {
			return parent.getParent();
		}
	}

	public void setParent(TreeNode node) {
		mParent = node;
	}
}
