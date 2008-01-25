package org.daisy.urakawa;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.event.ChangeListener;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.ChangeNotifierImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.project.PresentationAddedEvent;
import org.daisy.urakawa.event.project.PresentationRemovedEvent;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataReaderImpl;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.nativeapi.XmlDataWriterImpl;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukAbleAbstractImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ProjectImpl extends XukAbleAbstractImpl implements Project {
	private DataModelFactory mDataModelFactory;
	private List<Presentation> mPresentations;
	// The 2 event bus below handle events related to adding and removing
	// presentations to and from this project.
	// Please note that this class automatically adds a listener for each bus,
	// in order to handle
	// the (de)registration of a special listener (mBubbleEventListener) which
	// forwards the bubbling events from the Data Model. See comment for
	// mBubbleEventListener.
	protected ChangeNotifier<DataModelChangedEvent> mPresentationAddedEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mPresentationRemovedEventNotifier = new ChangeNotifierImpl();
	// This event bus receives all the events that are raised from within the
	// Data Model of the underlying Presentations of this Project, including the
	// above built-in events (PresentationRemoved and
	// PresentationRemoved).
	protected ChangeNotifier<DataModelChangedEvent> mDataModelEventNotifier = new ChangeNotifierImpl();

	// This "hub" method automatically dispatches the notify() call to the
	// appropriate ChangeNotifier (either mPresentationAddedEventNotifier,
	// mPresentationRemovedEventNotifier or mDataModelEventNotifier), based on
	// the type of the given event. Please note that the PresentationAdded and
	// PresentationRemoved events are passed to the generic
	// mDataModelEventNotifier event bus as well as to their corresponding
	// mPresentationAddedEventNotifier and mPresentationRemovedEventNotifier
	// bus.
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (PresentationAddedEvent.class.isAssignableFrom(event.getClass())) {
			mPresentationAddedEventNotifier.notifyListeners(event);
		} else if (PresentationRemovedEvent.class.isAssignableFrom(event
				.getClass())) {
			mPresentationRemovedEventNotifier.notifyListeners(event);
		}
		mDataModelEventNotifier.notifyListeners(event);
	}

	// This "hub" method automatically dispatches the registerListener() call to
	// the
	// appropriate ChangeNotifier (either mPresentationAddedEventNotifier,
	// mPresentationRemovedEventNotifier or mDataModelEventNotifier), based on
	// the class type given. Please note that the PresentationAdded and
	// PresentationRemoved listeners are not registered with the generic
	// mDataModelEventNotifier event bus (only to their corresponding
	// mPresentationAddedEventNotifier and mPresentationRemovedEventNotifier
	// bus).
	public <K extends DataModelChangedEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (PresentationAddedEvent.class.isAssignableFrom(klass)) {
			mPresentationAddedEventNotifier.registerListener(listener, klass);
		} else if (PresentationRemovedEvent.class.isAssignableFrom(klass)) {
			mPresentationRemovedEventNotifier.registerListener(listener, klass);
		} else {
			mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	// Same as above, for de-registration.
	public <K extends DataModelChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (PresentationAddedEvent.class.isAssignableFrom(klass)) {
			mPresentationAddedEventNotifier.unregisterListener(listener, klass);
		} else if (PresentationRemovedEvent.class.isAssignableFrom(klass)) {
			mPresentationRemovedEventNotifier.unregisterListener(listener,
					klass);
		} else {
			mDataModelEventNotifier.unregisterListener(listener, klass);
		}
	}

	// This listener receives events that are raised from within the
	// Presentations of this Project.
	// It simply forwards the received event to the main event bus for this
	// Project (which by default has no registered listeners: application
	// programmers should manually register their listeners by calling
	// Project.registerListener(ChangeListener<DataModelChangedEvent>,
	// DataModelChangedEvent.class)), or
	// Project.registerListener(ChangeListener<ChildAddedEvent>,
	// ChildAddedEvent.class)), or
	// Project.registerListener(ChangeListener<PresentationRemovedEvent>,
	// PresentationRemovedEvent.class)), etc.
	protected ChangeListener<DataModelChangedEvent> mBubbleEventListener = new ChangeListener<DataModelChangedEvent>() {
		
		public <K extends DataModelChangedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			notifyListeners(event);
		}
	};
	// This built-in listener takes care of registering the
	// mBubbleEventListener for a Presentation when that Presentation is added
	// to the Project.
	protected ChangeListener<PresentationAddedEvent> mPresentationAddedEventListener = new ChangeListener<PresentationAddedEvent>() {
		
		public <K extends PresentationAddedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceProject() == ProjectImpl.this) {
				event.getAddedPresentation().registerListener(
						mBubbleEventListener, DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};
	// This built-in listener takes care of unregistering the
	// mBubbleEventListener for a Presentation when that Presentation is removed
	// from the Project.
	protected ChangeListener<PresentationRemovedEvent> mPresentationRemovedEventListener = new ChangeListener<PresentationRemovedEvent>() {
		
		public <K extends PresentationRemovedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceProject() == ProjectImpl.this) {
				event.getRemovedPresentation().unregisterListener(
						mBubbleEventListener, DataModelChangedEvent.class);
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};

	/**
	 * Default constructor (empty list of Presentations)
	 */
	public ProjectImpl() {
		mPresentations = new LinkedList<Presentation>();
		try {
			registerListener(mPresentationAddedEventListener,
					PresentationAddedEvent.class);
			registerListener(mPresentationRemovedEventListener,
					PresentationRemovedEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void setDataModelFactory(DataModelFactory fact)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (fact == null) {
			throw new MethodParameterIsNullException();
		}
		if (mDataModelFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mDataModelFactory = fact;
	}

	public DataModelFactory getDataModelFactory() {
		if (mDataModelFactory == null) {
			// TODO: add a concrete constructor
			mDataModelFactory = new DataModelFactoryImpl();
		}
		return mDataModelFactory;
	}

	public void openXUK(URI uri) throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		// TODO: add a concrete constructor
		XmlDataReader source = new XmlDataReaderImpl(uri);
		try {
			openXUK(source);
		} finally {
			source.close();
		}
	}

	public void openXUK(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.readToFollowing("Xuk", XukAble.XUK_NS)) {
			throw new XukDeserializationFailedException();
		}
		boolean foundProject = false;
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == getXukLocalName()
							&& source.getNamespaceURI() == getXukNamespaceURI()) {
						foundProject = true;
						xukIn(source);
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
		if (!foundProject) {
			throw new XukDeserializationFailedException();
		}
	}

	public void saveXUK(URI uri) throws MethodParameterIsNullException,
			XukSerializationFailedException {
		// TODO: add a concrete constructor
		XmlDataWriter writer = new XmlDataWriterImpl(uri);
		try {
			saveXUK(writer, uri);
		} finally {
			writer.close();
		}
	}

	public void saveXUK(XmlDataWriter writer, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		writer.writeStartDocument();
		writer.writeStartElement("Xuk", XukAble.XUK_NS);
		// TODO: add schema declaration in XML header
		xukOut(writer, baseUri);
		writer.writeEndElement();
		writer.writeEndDocument();
	}

	public boolean ValueEquals(Project other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		if (getNumberOfPresentations() != other.getNumberOfPresentations())
			return false;
		for (int index = 0; index < getNumberOfPresentations(); index++) {
			try {
				if (!getPresentation(index).ValueEquals(
						other.getPresentation(index)))
					return false;
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
		return true;
	}

	public void cleanup() {
	}

	public Presentation addNewPresentation() {
		Presentation newPres = getDataModelFactory().createPresentation();
		try {
			addPresentation(newPres);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} catch (IsAlreadyManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return newPres;
	}

	public void addPresentation(Presentation presentation)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException {
		try {
			setPresentation(presentation, getNumberOfPresentations());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public List<Presentation> getListOfPresentations() {
		return new LinkedList<Presentation>(mPresentations);
	}

	public int getNumberOfPresentations() {
		return mPresentations.size();
	}

	public Presentation getPresentation(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || getNumberOfPresentations() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		return mPresentations.get(index);
	}

	public void removeAllPresentations() {
		mPresentations.clear();
	}

	public Presentation removePresentation(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || getNumberOfPresentations() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		Presentation pres = getPresentation(index);
		mPresentations.remove(index);
		try {
			notifyListeners(new PresentationRemovedEvent(this, pres));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return pres;
	}

	public void setPresentation(Presentation presentation, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, IsAlreadyManagerOfException {
		if (presentation == null) {
			throw new MethodParameterIsNullException();
		}
		if (index < 0 || getNumberOfPresentations() < index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (mPresentations.contains(presentation)) {
			if (mPresentations.indexOf(presentation) != index) {
				throw new IsAlreadyManagerOfException();
			}
		}
		if (index < getNumberOfPresentations()) {
			removePresentation(index);
			mPresentations.add(index, presentation);
		} else {
			mPresentations.add(presentation);
		}
		try {
			presentation.setProject(this);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		notifyListeners(new PresentationAddedEvent(this, presentation));
	}

	private void xukInPresentations(XmlDataReader source)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					Presentation pres = null;
					try {
						pres = getDataModelFactory()
								.createPresentation(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
					if (pres != null) {
						try {
							addPresentation(pres);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyManagerOfException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						try {
							pres.xukIn(source);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF()) {
					throw new XukDeserializationFailedException();
				}
			}
		}
	}

	@Override
	protected void clear() {
		removeAllPresentations();
		// super.clear();
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		@SuppressWarnings("unused")
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAble.XUK_NS) {
			if (source.getLocalName() == "mPresentations") {
				try {
					xukInPresentations(source);
				} catch (MethodParameterIsNullException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
				readItem = true;
			}
		}
		// if (!readItem) super.xukInChild(source);
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException,
			MethodParameterIsNullException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException,
			MethodParameterIsNullException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		// super.xukOutChildren(destination, baseUri);
		destination.writeStartElement("mPresentations", XukAble.XUK_NS);
		for (Presentation pres : getListOfPresentations()) {
			pres.xukOut(destination, baseUri);
		}
		destination.writeEndElement();
	}
}
