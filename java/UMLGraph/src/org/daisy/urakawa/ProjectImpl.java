package org.daisy.urakawa;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

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
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ProjectImpl extends XukAbleImpl implements Project {
	private DataModelFactory mDataModelFactory;
	private List<Presentation> mPresentations;

	/**
	 * Default constructor (empty list of Presentations)
	 */
	public ProjectImpl() {
		mPresentations = new LinkedList<Presentation>();
		// TODO: add events
		// this.presentationAdded += new
		// EventHandler<PresentationAddedEventArgs>(this_presentationAdded);
		// this.presentationRemoved += new
		// EventHandler<PresentationRemovedEventArgs>(this_presentationRemoved);
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
		if (!source.readToFollowing("Xuk", XukAbleImpl.XUK_NS)) {
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
		writer.writeStartElement("Xuk", XukAbleImpl.XUK_NS);
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
		// TODO: Add event notification
		// notifyPresentationRemoved(this, pres);
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
		// TODO: Add event notification
		// notifyPresentationAdded(this, presentation);
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
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
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
		destination.writeStartElement("mPresentations", XukAbleImpl.XUK_NS);
		for (Presentation pres : getListOfPresentations()) {
			pres.xukOut(destination, baseUri);
		}
		destination.writeEndElement();
	}
}
