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
import org.daisy.urakawa.event.project.ProjectEvent;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataReaderImpl;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XmlDataWriterImpl;
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
	protected ChangeNotifier<DataModelChangedEvent> mPresentationAddedEventNotifier = new ChangeNotifierImpl();
	protected ChangeNotifier<DataModelChangedEvent> mPresentationRemovedEventNotifier = new ChangeNotifierImpl();

	public <K extends ProjectEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (PresentationAddedEvent.class.isAssignableFrom(event.getClass())) {
			mPresentationAddedEventNotifier.notifyListeners(event);
		}
		if (PresentationRemovedEvent.class.isAssignableFrom(event.getClass())) {
			mPresentationRemovedEventNotifier.notifyListeners(event);
		}
	}

	public <K extends ProjectEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (PresentationAddedEvent.class.isAssignableFrom(klass)) {
			mPresentationAddedEventNotifier.registerListener(listener, klass);
		}
		if (PresentationRemovedEvent.class.isAssignableFrom(klass)) {
			mPresentationRemovedEventNotifier.registerListener(listener, klass);
		}
	}

	public <K extends ProjectEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (PresentationAddedEvent.class.isAssignableFrom(klass)) {
			mPresentationAddedEventNotifier.unregisterListener(listener, klass);
		}
		if (PresentationRemovedEvent.class.isAssignableFrom(klass)) {
			mPresentationRemovedEventNotifier.unregisterListener(listener,
					klass);
		}
	}

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_PresentationAddedEventListener(
			PresentationAddedEvent event) throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<PresentationAddedEvent> mPresentationAddedEventListener = new ChangeListener<PresentationAddedEvent>() {
		@Override
		public <K extends PresentationAddedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_PresentationAddedEventListener(event);
		}
	};

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_PresentationRemovedEventListener(
			PresentationRemovedEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<PresentationRemovedEvent> mPresentationRemovedEventListener = new ChangeListener<PresentationRemovedEvent>() {
		@Override
		public <K extends PresentationRemovedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_PresentationRemovedEventListener(event);
		}
	};

	/**
	 * Default constructor (empty list of Presentations)
	 */
	public ProjectImpl() {
		mPresentations = new LinkedList<Presentation>();
		try {
			mPresentationAddedEventNotifier.registerListener(
					mPresentationAddedEventListener,
					PresentationAddedEvent.class);
			mPresentationRemovedEventNotifier.registerListener(
					mPresentationRemovedEventListener,
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
			// FIXME: add a concrete constructor
			mDataModelFactory = new DataModelFactoryImpl();
		}
		return mDataModelFactory;
	}

	public void openXUK(URI uri) throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		// FIXME: add a concrete constructor
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
		// FIXME: add a concrete constructor
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
		// TODO: Is returning a new List wrapper really necessary ? (to avoid
		// external code to modify the list)
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
