package org.daisy.urakawa;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.project.PresentationAddedEvent;
import org.daisy.urakawa.event.project.PresentationRemovedEvent;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsAlreadyManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.OpenXukAction;
import org.daisy.urakawa.xuk.SaveXukAction;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukAbleAbstractImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ProjectImpl extends XukAbleAbstractImpl implements IProject {
	private IDataModelFactory mDataModelFactory;
	private List<IPresentation> mPresentations;
	// The 2 event bus below handle events related to adding and removing
	// presentations to and from this project.
	// Please note that this class automatically adds a listener for each bus,
	// in order to handle
	// the (de)registration of a special listener (mBubbleEventListener) which
	// forwards the bubbling events from the Data Model. See comment for
	// mBubbleEventListener.
	protected IEventHandler<Event> mPresentationAddedEventNotifier = new EventHandlerImpl();
	protected IEventHandler<Event> mPresentationRemovedEventNotifier = new EventHandlerImpl();
	// This event bus receives all the events that are raised from within the
	// Data Model of the underlying Presentations of this IProject, including the
	// above built-in events (PresentationRemoved and
	// PresentationRemoved).
	protected IEventHandler<Event> mDataModelEventNotifier = new EventHandlerImpl();

	// This "hub" method automatically dispatches the notify() call to the
	// appropriate IEventHandler (either mPresentationAddedEventNotifier,
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
	// appropriate IEventHandler (either mPresentationAddedEventNotifier,
	// mPresentationRemovedEventNotifier or mDataModelEventNotifier), based on
	// the class type given. Please note that the PresentationAdded and
	// PresentationRemoved listeners are not registered with the generic
	// mDataModelEventNotifier event bus (only to their corresponding
	// mPresentationAddedEventNotifier and mPresentationRemovedEventNotifier
	// bus).
	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
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
			IEventListener<K> listener, Class<K> klass)
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
	// Presentations of this IProject.
	// It simply forwards the received event to the main event bus for this
	// IProject (which by default has no registered listeners: application
	// programmers should manually register their listeners by calling
	// IProject.registerListener(IEventListener<DataModelChangedEvent>,
	// DataModelChangedEvent.class)), or
	// IProject.registerListener(IEventListener<ChildAddedEvent>,
	// ChildAddedEvent.class)), or
	// IProject.registerListener(IEventListener<PresentationRemovedEvent>,
	// PresentationRemovedEvent.class)), etc.
	protected IEventListener<DataModelChangedEvent> mBubbleEventListener = new IEventListener<DataModelChangedEvent>() {
		public <K extends DataModelChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			notifyListeners(event);
		}
	};
	// This built-in listener takes care of registering the
	// mBubbleEventListener for a IPresentation when that IPresentation is added
	// to the IProject.
	protected IEventListener<PresentationAddedEvent> mPresentationAddedEventListener = new IEventListener<PresentationAddedEvent>() {
		public <K extends PresentationAddedEvent> void eventCallback(K event)
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
	// mBubbleEventListener for a IPresentation when that IPresentation is removed
	// from the IProject.
	protected IEventListener<PresentationRemovedEvent> mPresentationRemovedEventListener = new IEventListener<PresentationRemovedEvent>() {
		public <K extends PresentationRemovedEvent> void eventCallback(K event)
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
		mPresentations = new LinkedList<IPresentation>();
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

	public void setDataModelFactory(IDataModelFactory fact)
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

	public IDataModelFactory getDataModelFactory() {
		if (mDataModelFactory == null) {
			// TODO: add a concrete constructor
			mDataModelFactory = new DataModelFactoryImpl();
		}
		return mDataModelFactory;
	}

	public void openXUK(URI uri) throws MethodParameterIsNullException {
		if (uri == null) {
			throw new MethodParameterIsNullException();
		}
		OpenXukAction action = new OpenXukAction(uri, this);
		try {
			action.execute();
		} catch (CommandCannotExecuteException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		}
	}

	public void saveXUK(URI uri) throws MethodParameterIsNullException {
		if (uri == null) {
			throw new MethodParameterIsNullException();
		}
		SaveXukAction action = new SaveXukAction(uri, this);
		try {
			action.execute();
		} catch (CommandCannotExecuteException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		}
	}

	public boolean ValueEquals(IProject other)
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

	public IPresentation addNewPresentation() {
		IPresentation newPres = getDataModelFactory().createPresentation();
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

	public void addPresentation(IPresentation iPresentation)
			throws MethodParameterIsNullException, IsAlreadyManagerOfException {
		try {
			setPresentation(iPresentation, getNumberOfPresentations());
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public List<IPresentation> getListOfPresentations() {
		return new LinkedList<IPresentation>(mPresentations);
	}

	public int getNumberOfPresentations() {
		return mPresentations.size();
	}

	public IPresentation getPresentation(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || getNumberOfPresentations() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		return mPresentations.get(index);
	}

	public void removeAllPresentations() {
		mPresentations.clear();
	}

	public IPresentation removePresentation(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (index < 0 || getNumberOfPresentations() <= index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		IPresentation pres = getPresentation(index);
		mPresentations.remove(index);
		try {
			notifyListeners(new PresentationRemovedEvent(this, pres));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return pres;
	}

	public void setPresentation(IPresentation iPresentation, int index)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, IsAlreadyManagerOfException {
		if (iPresentation == null) {
			throw new MethodParameterIsNullException();
		}
		if (index < 0 || getNumberOfPresentations() < index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (mPresentations.contains(iPresentation)) {
			if (mPresentations.indexOf(iPresentation) != index) {
				throw new IsAlreadyManagerOfException();
			}
		}
		if (index < getNumberOfPresentations()) {
			removePresentation(index);
			mPresentations.add(index, iPresentation);
		} else {
			mPresentations.add(iPresentation);
		}
		try {
			iPresentation.setProject(this);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		notifyListeners(new PresentationAddedEvent(this, iPresentation));
	}

	private void xukInPresentations(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					IPresentation pres = null;
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
							pres.xukIn(source, ph);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
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
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
	}

	@Override
	protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		@SuppressWarnings("unused")
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			if (source.getLocalName() == "mPresentations") {
				try {
					xukInPresentations(source, ph);
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
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
	}

	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		// super.xukOutChildren(destination, baseUri);
		destination.writeStartElement("mPresentations", IXukAble.XUK_NS);
		for (IPresentation pres : getListOfPresentations()) {
			pres.xukOut(destination, baseUri, ph);
		}
		destination.writeEndElement();
	}
}
