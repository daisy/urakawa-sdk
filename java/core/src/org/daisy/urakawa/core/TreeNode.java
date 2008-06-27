package org.daisy.urakawa.core;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.core.visitor.ITreeNodeVisitor;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.core.ChildAddedEvent;
import org.daisy.urakawa.event.core.ChildRemovedEvent;
import org.daisy.urakawa.event.core.PropertyAddedEvent;
import org.daisy.urakawa.event.core.PropertyRemovedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.property.IProperty;
import org.daisy.urakawa.property.PropertyAlreadyHasOwnerException;
import org.daisy.urakawa.property.PropertyCannotBeAddedToTreeNodeException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class TreeNode extends WithPresentation implements ITreeNode {
	private List<IProperty> mProperties;
	private List<ITreeNode> mChildren;
	private ITreeNode mParent;
	// This event bus receives all the events that are raised from within the
	// Data Model of the underlying objects that make this sub-tree (i.e.
	// the sub-tree of TreeNodes, properties and media), including the above
	// built-in events.
	// IF this TreeNodeis the root of a IPresentation, that IPresentation
	// instance
	// automatically
	// register a listener on this generic
	// event bus, behind the scenes. This is how events are forwarded from this
	// tree level to the upper
	// IPresentation level.
	protected IEventHandler<Event> mDataModelEventNotifier = new EventHandler();
	// The 5 event bus below handle events related to node and property change
	// events.
	// Please note that this class automatically adds a listener for the
	// ChildAddedEvent, ChildRemovedEvent, PropertyAddedEvent,
	// PropertyRemovedEvent events,
	// in order to handle the (de)registration of a special listener
	// (mBubbleEventListener) which
	// forwards the bubbling events from the nodes in this sub-tree. See comment
	// for
	// mBubbleEventListener.
	protected IEventHandler<Event> mChildAddedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mChildRemovedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mPropertyAddedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mPropertyRemovedEventNotifier = new EventHandler();

	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (PropertyAddedEvent.class.isAssignableFrom(event.getClass())) {
			mPropertyAddedEventNotifier.notifyListeners(event);
		} else if (PropertyRemovedEvent.class
				.isAssignableFrom(event.getClass())) {
			mPropertyRemovedEventNotifier.notifyListeners(event);
		} else if (ChildAddedEvent.class.isAssignableFrom(event.getClass())) {
			mChildAddedEventNotifier.notifyListeners(event);
		} else if (ChildRemovedEvent.class.isAssignableFrom(event.getClass())) {
			mChildRemovedEventNotifier.notifyListeners(event);
		}
		mDataModelEventNotifier.notifyListeners(event);
	}

	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (PropertyAddedEvent.class.isAssignableFrom(klass)) {
			mPropertyAddedEventNotifier.registerListener(listener, klass);
		} else if (PropertyRemovedEvent.class.isAssignableFrom(klass)) {
			mPropertyRemovedEventNotifier.registerListener(listener, klass);
		} else if (ChildAddedEvent.class.isAssignableFrom(klass)) {
			mChildAddedEventNotifier.registerListener(listener, klass);
		} else if (ChildRemovedEvent.class.isAssignableFrom(klass)) {
			mChildRemovedEventNotifier.registerListener(listener, klass);
		} else {
			mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (PropertyAddedEvent.class.isAssignableFrom(klass)) {
			mPropertyAddedEventNotifier.unregisterListener(listener, klass);
		} else if (PropertyRemovedEvent.class.isAssignableFrom(klass)) {
			mPropertyRemovedEventNotifier.unregisterListener(listener, klass);
		} else if (ChildAddedEvent.class.isAssignableFrom(klass)) {
			mChildAddedEventNotifier.unregisterListener(listener, klass);
		} else if (ChildRemovedEvent.class.isAssignableFrom(klass)) {
			mChildRemovedEventNotifier.unregisterListener(listener, klass);
		} else {
			mDataModelEventNotifier.unregisterListener(listener, klass);
		}
	}

	protected IEventListener<DataModelChangedEvent> mBubbleEventListener = new IEventListener<DataModelChangedEvent>() {
		public <K extends DataModelChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			notifyListeners(event);
		}
	};
	protected IEventListener<ChildAddedEvent> mChildAddedEventListener = new IEventListener<ChildAddedEvent>() {
		public <K extends ChildAddedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceTreeNode() == TreeNode.this) {
				event.getAddedChild().registerListener(mBubbleEventListener,
						DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};
	protected IEventListener<ChildRemovedEvent> mChildRemovedEventListener = new IEventListener<ChildRemovedEvent>() {
		public <K extends ChildRemovedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceTreeNode() == TreeNode.this) {
				event.getRemovedChild().unregisterListener(
						mBubbleEventListener, DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};
	protected IEventListener<PropertyAddedEvent> mPropertyAddedEventListener = new IEventListener<PropertyAddedEvent>() {
		public <K extends PropertyAddedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceTreeNode() == TreeNode.this) {
				event.getAddedProperty().registerListener(mBubbleEventListener,
						DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};
	protected IEventListener<PropertyRemovedEvent> mPropertyRemovedEventListener = new IEventListener<PropertyRemovedEvent>() {
		public <K extends PropertyRemovedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceTreeNode() == TreeNode.this) {
				event.getRemovedProperty().unregisterListener(
						mBubbleEventListener, DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};

	/**
	 * 
	 */
	public TreeNode() {
		mProperties = new LinkedList<IProperty>();
		mChildren = new LinkedList<ITreeNode>();
		try {
			registerListener(mChildAddedEventListener, ChildAddedEvent.class);
			registerListener(mChildRemovedEventListener,
					ChildRemovedEvent.class);
			registerListener(mPropertyAddedEventListener,
					PropertyAddedEvent.class);
			registerListener(mPropertyRemovedEventListener,
					PropertyRemovedEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void copyChildren(ITreeNode destinationNode)
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

	public boolean hasProperty(IProperty prop)
			throws MethodParameterIsNullException {
		if (prop == null)
			throw new MethodParameterIsNullException();
		return mProperties.contains(prop);
	}

	public <T extends IProperty> boolean hasProperties(Class<T> klass)
			throws MethodParameterIsNullException {
		if (klass == null)
			throw new MethodParameterIsNullException();
		for (IProperty p : getListOfProperties()) {
			if (p.getClass() == klass)
				return true;
		}
		return false;
	}

	public void removeProperty(IProperty prop)
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
		for (IProperty p : getListOfProperties()) {
			try {
				removeProperty(p);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public <T extends IProperty> List<T> removeProperties(Class<T> propType)
			throws MethodParameterIsNullException {
		if (propType == null) {
			throw new MethodParameterIsNullException();
		}
		List<T> remProps;
		remProps = getListOfProperties(propType);
		for (IProperty p : remProps) {
			removeProperty(p);
		}
		return remProps;
	}

	public <T extends IProperty> T getProperty(Class<T> klass)
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

	public <T extends IProperty> void addProperties(List<T> props)
			throws MethodParameterIsNullException,
			PropertyCannotBeAddedToTreeNodeException,
			PropertyAlreadyHasOwnerException {
		if (props == null)
			throw new MethodParameterIsNullException();
		for (T p : props) {
			addProperty(p);
		}
	}

	public <T extends IProperty> void addProperty(T prop)
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
	public <T extends IProperty> List<Class<T>> getListOfUsedPropertyTypes() {
		List<Class<T>> res = new LinkedList<Class<T>>();
		for (IProperty p : (List<T>) getListOfProperties()) {
			if (!res.contains(p.getClass()))
				res.add((Class<T>) p.getClass());
		}
		return res;
	}

	@SuppressWarnings("unchecked")
	public <T extends IProperty> List<T> getListOfProperties() {
		List<T> list = new LinkedList<T>();
		list.addAll((List<T>) mProperties);
		return list;
	}

	@SuppressWarnings("unchecked")
	public <T extends IProperty> List<T> getListOfProperties(Class<T> klass)
			throws MethodParameterIsNullException {
		if (klass == null) {
			throw new MethodParameterIsNullException();
		}
		List<T> res = new LinkedList<T>();
		for (IProperty p : getListOfProperties()) {
			if (p.getClass() == klass)
				res.add((T) p);
		}
		return res;
	}

	public void acceptDepthFirst(ITreeNodeVisitor visitor)
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

	public void acceptBreadthFirst(ITreeNodeVisitor visitor)
			throws MethodParameterIsNullException {
		if (visitor == null)
			throw new MethodParameterIsNullException();
		Queue<ITreeNode> nodeQueue = new LinkedList<ITreeNode>();
		nodeQueue.offer(this);
		while (nodeQueue.size() > 0) {
			ITreeNode next = nodeQueue.poll();
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
		for (ITreeNode child : getListOfChildren()) {
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
		for (IProperty prop : getListOfProperties()) {
			try {
				removeProperty(prop);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		// super.clear();
	}

	private void xukInProperties(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null)
			throw new MethodParameterIsNullException();
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					IProperty newProp;
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
						newProp.xukIn(source, ph);
					} else {
						super.xukInChild(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	private void xukInChildren(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null)
			throw new MethodParameterIsNullException();
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					ITreeNode newChild;
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
						newChild.xukIn(source, ph);
					} else {
						super.xukInChild(source, ph);
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	public void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null)
			throw new MethodParameterIsNullException();
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			readItem = true;
			String str = source.getLocalName();
			if (str == "mProperties") {
				xukInProperties(source, ph);
			} else if (str == "mChildren") {
				xukInChildren(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			super.xukInChild(source, ph);
		}
	}

	@Override
	public void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null)
			throw new MethodParameterIsNullException();
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeStartElement("mProperties", IXukAble.XUK_NS);
		for (IProperty prop : getListOfProperties()) {
			prop.xukOut(destination, baseUri, ph);
		}
		destination.writeEndElement();
		destination.writeStartElement("mChildren", IXukAble.XUK_NS);
		for (int i = 0; i < this.getChildCount(); i++) {
			try {
				getChild(i).xukOut(destination, baseUri, ph);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		destination.writeEndElement();
		// super.xukOutChildren(destination, baseUri, ph);
	}

	public int indexOf(ITreeNode node) throws MethodParameterIsNullException,
			TreeNodeDoesNotExistException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mChildren.contains(node)) {
			throw new TreeNodeDoesNotExistException();
		}
		return mChildren.indexOf(node);
	}

	public ITreeNode getChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || mChildren.size() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		return mChildren.get(index);
	}

	public ITreeNode getParent() {
		return mParent;
	}

	public int getChildCount() {
		return mChildren.size();
	}

	public List<ITreeNode> getListOfChildren() {
		return new LinkedList<ITreeNode>(mChildren);
	}

	protected void copyProperties(ITreeNode destinationNode)
			throws MethodParameterIsNullException {
		if (destinationNode == null) {
			throw new MethodParameterIsNullException();
		}
		for (IProperty prop : getListOfProperties()) {
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

	protected ITreeNode copyProtected(boolean deep, boolean inclProperties) {
		ITreeNode theCopy;
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

	public ITreeNode copy(boolean deep, boolean inclProperties) {
		return copyProtected(deep, inclProperties);
	}

	public ITreeNode copy(boolean deep) {
		return copy(deep, true);
	}

	public ITreeNode copy() {
		return copy(true, true);
	}

	public ITreeNode export(IPresentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return exportProtected(destPres);
	}

	protected ITreeNode exportProtected(IPresentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ITreeNode exportedNode;
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
		for (IProperty prop : getListOfProperties()) {
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
		for (ITreeNode child : getListOfChildren()) {
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

	public ITreeNode getNextSibling() {
		ITreeNode p = getParent();
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

	public ITreeNode getPreviousSibling() {
		ITreeNode p = getParent();
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

	public boolean isSiblingOf(ITreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		ITreeNode p = getParent();
		return (p != null && p == node.getParent());
	}

	public boolean isAncestorOf(ITreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		ITreeNode p = getParent();
		if (p == null) {
			return false;
		} else if (p == node) {
			return true;
		} else {
			return p.isAncestorOf(node);
		}
	}

	public boolean isDescendantOf(ITreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		return node.isAncestorOf(this);
	}

	public void insert(ITreeNode node, int insertIndex)
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

	public ITreeNode detach() {
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

	public ITreeNode removeChild(int index)
			throws MethodParameterIsOutOfBoundsException {
		ITreeNode removedChild = getChild(index);
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

	public ITreeNode removeChild(ITreeNode node)
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

	public void insertBefore(ITreeNode node, ITreeNode anchorNode)
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

	public void insertAfter(ITreeNode node, ITreeNode anchorNode)
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

	public ITreeNode replaceChild(ITreeNode node, int index)
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
		ITreeNode replacedChild = getChild(index);
		insert(node, index);
		replacedChild.detach();
		return replacedChild;
	}

	public ITreeNode replaceChild(ITreeNode node, ITreeNode oldNode)
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

	public void appendChild(ITreeNode node)
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

	public void appendChildrenOf(ITreeNode node)
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

	public void swapWith(ITreeNode node) throws MethodParameterIsNullException,
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
		ITreeNode thisParent = getParent();
		int thisIndex;
		try {
			thisIndex = thisParent.indexOf(this);
		} catch (TreeNodeDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		detach();
		ITreeNode nodeParent = node.getParent();
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

	public ITreeNode splitChildren(int index, boolean copyProperties)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || getChildCount() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		ITreeNode res = copy(false, copyProperties);
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
		ITreeNode nextSibling = getNextSibling();
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
		ITreeNode prevSibling = getPreviousSibling();
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

	public boolean ValueEquals(ITreeNode other)
			throws MethodParameterIsNullException {
		if (other == null)
			throw new MethodParameterIsNullException();
		if (other.getClass() != this.getClass())
			return false;
		List<Class<IProperty>> thisProps = getListOfUsedPropertyTypes();
		List<Class<IProperty>> otherProps = other.getListOfUsedPropertyTypes();
		if (thisProps.size() != otherProps.size())
			return false;
		for (Class<IProperty> pt : thisProps) {
			List<IProperty> thisPs = getListOfProperties(pt);
			List<IProperty> otherPs = other.getListOfProperties(pt);
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

	public ITreeNode getRoot() {
		ITreeNode parent = getParent();
		if (parent == null) {
			return this;
		}
		return parent.getParent();
	}

	public void setParent(ITreeNode node) {
		mParent = node;
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}
}
