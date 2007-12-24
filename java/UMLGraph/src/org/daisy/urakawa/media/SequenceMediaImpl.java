package org.daisy.urakawa.media;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class SequenceMediaImpl extends MediaAbstractImpl implements
		SequenceMedia {
	private List<Media> mSequence;
	private boolean mAllowMultipleTypes;

	/**
	 * 
	 */
	public SequenceMediaImpl() {
		mSequence = new LinkedList<Media>();
		mAllowMultipleTypes = false;
	}

	public Media getItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (0 <= index && index < getCount()) {
			return (Media) mSequence.get(index);
		} else {
			throw new MethodParameterIsOutOfBoundsException();
		}
	}

	public void insertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, DoesNotAcceptMediaException {
		if (newItem == null) {
			throw new MethodParameterIsNullException();
		}
		if (index < 0 || getCount() < index) {
			throw new MethodParameterIsOutOfBoundsException();
		}
		if (!canAcceptMedia(newItem)) {
			throw new DoesNotAcceptMediaException();
		}
		mSequence.add(index, newItem);
	}

	public void appendItem(Media newItem)
			throws MethodParameterIsNullException, DoesNotAcceptMediaException {
		if (newItem == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			insertItem(getCount(), newItem);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		}
	}

	public Media removeItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		Media removedMedia = getItem(index);
		try {
			removeItem(removedMedia);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		} catch (MediaIsNotInSequenceException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		}
		return removedMedia;
	}

	public void removeItem(Media item) throws MethodParameterIsNullException,
			MediaIsNotInSequenceException {
		if (item == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mSequence.contains(item)) {
			throw new MediaIsNotInSequenceException();
		}
		mSequence.remove(item);
	}

	public int getCount() {
		return mSequence.size();
	}

	public List<Media> getListOfItems() {
		return new LinkedList<Media>(mSequence);
	}

	public boolean getAllowMultipleTypes() {
		return mAllowMultipleTypes;
	}

	@SuppressWarnings("unchecked")
	public void setAllowMultipleTypes(boolean newValue)
			throws SequenceHasMultipleTypesException {
		if (!newValue) {
			int count = getCount();
			if (count > 0) {
				Class<Media> firstItemType;
				try {
					firstItemType = (Class<Media>) getItem(0).getClass();
				} catch (MethodParameterIsOutOfBoundsException e) {
					// Should never happen
					throw new RuntimeException("WFT ??!", e);
				}
				int i = 1;
				while (i < count) {
					try {
						if (getItem(i).getClass() != firstItemType) {
							throw new SequenceHasMultipleTypesException();
						}
					} catch (MethodParameterIsOutOfBoundsException e) {
						// Should never happen
						throw new RuntimeException("WFT ??!", e);
					}
				}
			}
		}
		mAllowMultipleTypes = newValue;
	}

	@Override
	public boolean isContinuous() {
		if (getCount() > 0) {
			try {
				return getItem(0).isContinuous();
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		} else {
			return false;
		}
	}

	@Override
	public boolean isDiscrete() {
		// use the first item in the collection to determine the value
		if (getCount() > 0) {
			try {
				return getItem(0).isDiscrete();
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		} else {
			return false;
		}
	}

	@Override
	public boolean isSequence() {
		return true;
	}

	@Override
	public SequenceMedia copy() {
		return (SequenceMedia) copyProtected();
	}

	@Override
	protected Media copyProtected() {
		Media newMedia;
		try {
			newMedia = getMediaFactory().createMedia(getXukLocalName(),
					getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		}
		SequenceMedia newSeqMedia = (SequenceMedia) newMedia;
		for (Media item : getListOfItems()) {
			try {
				newSeqMedia.appendItem(item.copy());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			} catch (DoesNotAcceptMediaException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		}
		return newSeqMedia;
	}

	@Override
	public SequenceMedia export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (SequenceMedia) exportProtected(destPres);
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		SequenceMedia exported;
		try {
			exported = (SequenceMedia) destPres.getMediaFactory().createMedia(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WFT ??!", e);
		}
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		for (Media m : getListOfItems()) {
			try {
				exported.appendItem(m.export(destPres));
			} catch (DoesNotAcceptMediaException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		}
		return exported;
	}

	public boolean canAcceptMedia(Media proposedAddition)
			throws MethodParameterIsNullException {
		if (proposedAddition == null) {
			throw new MethodParameterIsNullException();
		}
		if (getCount() > 0 && !getAllowMultipleTypes()) {
			try {
				if (getItem(0).getClass() != proposedAddition.getClass())
					return false;
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		}
		return true;
	}

	@Override
	protected void clear() {
		mAllowMultipleTypes = false;
		for (Media item : getListOfItems()) {
			try {
				removeItem(item);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			} catch (MediaIsNotInSequenceException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		}
		super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String val = source.getAttribute("allowMultipleMediaTypes");
		if (val == "true" || val == "1") {
			try {
				setAllowMultipleTypes(true);
			} catch (SequenceHasMultipleTypesException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		} else {
			try {
				setAllowMultipleTypes(false);
			} catch (SequenceHasMultipleTypesException e) {
				throw new XukDeserializationFailedException();
			}
		}
		super.xukInAttributes(source);
	}

	@Override
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAble.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mSequence") {
				xukInSequence(source);
			} else {
				readItem = false;
			}
		}
		if (!readItem)
			super.xukIn(source);
	}

	private void xukInSequence(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					Media newMedia;
					try {
						newMedia = getMediaFactory()
								.createMedia(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WFT ??!", e);
					}
					if (newMedia != null) {
						newMedia.xukIn(source);
						if (!canAcceptMedia(newMedia)) {
							throw new XukDeserializationFailedException();
						}
						try {
							insertItem(getCount(), newMedia);
						} catch (MethodParameterIsOutOfBoundsException e) {
							// Should never happen
							throw new RuntimeException("WFT ??!", e);
						} catch (DoesNotAcceptMediaException e) {
							// Should never happen
							throw new RuntimeException("WFT ??!", e);
						}
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

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		destination.writeAttributeString("allowMultipleMediaTypes",
				getAllowMultipleTypes() ? "true" : "false");
		super.xukOutAttributes(destination, baseUri);
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (getCount() > 0) {
			destination.writeStartElement("mSequence", XukAble.XUK_NS);
			for (int i = 0; i < getCount(); i++) {
				try {
					getItem(i).xukOut(destination, baseUri);
				} catch (MethodParameterIsOutOfBoundsException e) {
					// Should never happen
					throw new RuntimeException("WFT ??!", e);
				}
			}
			destination.writeEndElement();
		}
		super.xukOutChildren(destination, baseUri);
	}

	@Override
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (!super.ValueEquals(other))
			return false;
		SequenceMedia otherSeq = (SequenceMedia) other;
		if (getCount() != otherSeq.getCount())
			return false;
		for (int i = 0; i < getCount(); i++) {
			try {
				if (!getItem(i).ValueEquals(otherSeq.getItem(i)))
					return false;
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		}
		return true;
	}
}
