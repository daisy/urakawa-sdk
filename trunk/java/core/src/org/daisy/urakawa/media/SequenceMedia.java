package org.daisy.urakawa.media;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class SequenceMedia extends AbstractMedia implements
		ISequenceMedia {
	private List<IMedia> mSequence;
	private boolean mAllowMultipleTypes;

	/**
	 * 
	 */
	public SequenceMedia() {
		mSequence = new LinkedList<IMedia>();
		mAllowMultipleTypes = false;
	}

	public IMedia getItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		if (0 <= index && index < getCount()) {
			return mSequence.get(index);
		} else {
			throw new MethodParameterIsOutOfBoundsException();
		}
	}

	public void insertItem(int index, IMedia newItem)
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
		newItem.registerListener(mBubbleEventListener,
				DataModelChangedEvent.class);
	}

	public void appendItem(IMedia newItem)
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

	public IMedia removeItem(int index)
			throws MethodParameterIsOutOfBoundsException {
		IMedia removedMedia = getItem(index);
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

	public void removeItem(IMedia item) throws MethodParameterIsNullException,
			MediaIsNotInSequenceException {
		if (item == null) {
			throw new MethodParameterIsNullException();
		}
		if (!mSequence.contains(item)) {
			throw new MediaIsNotInSequenceException();
		}
		mSequence.remove(item);
		item.unregisterListener(mBubbleEventListener,
				DataModelChangedEvent.class);
	}

	public int getCount() {
		return mSequence.size();
	}

	public List<IMedia> getListOfItems() {
		return new LinkedList<IMedia>(mSequence);
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
				Class<IMedia> firstItemType;
				try {
					firstItemType = (Class<IMedia>) getItem(0).getClass();
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
	public ISequenceMedia copy() {
		return (ISequenceMedia) copyProtected();
	}

	@Override
	protected IMedia copyProtected() {
		IMedia newMedia;
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
		ISequenceMedia newSeqMedia = (ISequenceMedia) newMedia;
		for (IMedia item : getListOfItems()) {
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
	public ISequenceMedia export(IPresentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ISequenceMedia) exportProtected(destPres);
	}

	@Override
	protected IMedia exportProtected(IPresentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ISequenceMedia exported;
		try {
			exported = (ISequenceMedia) destPres.getMediaFactory().createMedia(
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
		for (IMedia m : getListOfItems()) {
			try {
				exported.appendItem(m.export(destPres));
			} catch (DoesNotAcceptMediaException e) {
				// Should never happen
				throw new RuntimeException("WFT ??!", e);
			}
		}
		return exported;
	}

	public boolean canAcceptMedia(IMedia proposedAddition)
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
		for (IMedia item : getListOfItems()) {
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
		super.xukInAttributes(source, ph);
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
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mSequence") {
				xukInSequence(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			super.xukInChild(source, ph);
		}
	}

	private void xukInSequence(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					IMedia newMedia;
					try {
						newMedia = getMediaFactory()
								.createMedia(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsEmptyStringException e) {
						// Should never happen
						throw new RuntimeException("WFT ??!", e);
					}
					if (newMedia != null) {
						newMedia.xukIn(source, ph);
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
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeAttributeString("allowMultipleMediaTypes",
				getAllowMultipleTypes() ? "true" : "false");
		super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (getCount() > 0) {
			destination.writeStartElement("mSequence", IXukAble.XUK_NS);
			for (int i = 0; i < getCount(); i++) {
				try {
					getItem(i).xukOut(destination, baseUri, ph);
				} catch (MethodParameterIsOutOfBoundsException e) {
					// Should never happen
					throw new RuntimeException("WFT ??!", e);
				}
			}
			destination.writeEndElement();
		}
		//super.xukOutChildren(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(IMedia other)
			throws MethodParameterIsNullException {
		if (!super.ValueEquals(other))
			return false;
		ISequenceMedia otherSeq = (ISequenceMedia) other;
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
