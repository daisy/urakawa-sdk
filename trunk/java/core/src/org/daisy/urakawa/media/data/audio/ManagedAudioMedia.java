package org.daisy.urakawa.media.data.audio;

import java.io.IOException;
import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.Event;
import org.daisy.urakawa.events.EventHandler;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.events.IEventListener;
import org.daisy.urakawa.events.media.data.MediaDataChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsWrongTypeException;
import org.daisy.urakawa.media.AbstractMedia;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ManagedAudioMedia extends AbstractMedia implements
		IManagedAudioMedia {
	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(event.getClass())) {
			mMediaDataChangedEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(klass)) {
			mMediaDataChangedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(klass)) {
			mMediaDataChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	protected IEventListener<MediaDataChangedEvent> mMediaDataChangedEventListener = new IEventListener<MediaDataChangedEvent>() {
		@SuppressWarnings("synthetic-access")
		public <K extends MediaDataChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceManagedMedia() == ManagedAudioMedia.this) {
				IMediaData dataPrevious = event.getPreviousMediaData();
				if (dataPrevious != null) {
					dataPrevious.unregisterListener(
							ManagedAudioMedia.this.mBubbleEventListener,
							DataModelChangedEvent.class);
				}
				IMediaData dataNew = event.getNewMediaData();
				if (dataNew != null) {
					dataNew.registerListener(
							ManagedAudioMedia.this.mBubbleEventListener,
							DataModelChangedEvent.class);
				}
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};
	protected IEventHandler<Event> mMediaDataChangedEventNotifier = new EventHandler();

	/**
	 * 
	 */
	public ManagedAudioMedia() {
		try {
			registerListener(mMediaDataChangedEventListener,
					MediaDataChangedEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	private IAudioMediaData mAudioMediaData = null;

	@Override
	public boolean isContinuous() {
		return true;
	}

	@Override
	public boolean isDiscrete() {
		return false;
	}

	@Override
	public boolean isSequence() {
		return false;
	}

	@Override
	public IManagedAudioMedia copy() {
		return (IManagedAudioMedia) copyProtected();
	}

	@Override
	protected IMedia copyProtected() {
		IManagedAudioMedia copyMAM = (IManagedAudioMedia) super.copyProtected();
		try {
			copyMAM.setLanguage(getLanguage());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			copyMAM.setMediaData(getMediaData().copy());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return copyMAM;
	}

	@Override
	public IManagedAudioMedia export(IPresentation destPres)
			throws MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return export(destPres);
	}

	@Override
	protected IMedia exportProtected(IPresentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		IManagedAudioMedia exported = (IManagedAudioMedia) super
				.exportProtected(destPres);
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			exported.setLanguage(getLanguage());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		exported.setMediaData(getMediaData().export(destPres));
		return exported;
	}

	public IManagedAudioMedia copy(ITime clipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		return copy(clipBegin, new Time().getZero().addTimeDelta(getDuration()));
	}

	public IManagedAudioMedia copy(ITime clipBegin, ITime clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null || clipEnd == null) {
			throw new MethodParameterIsNullException();
		}
		IManagedAudioMedia copyMAM;
		try {
			copyMAM = (IManagedAudioMedia) getPresentation().getMediaFactory().create(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		IStream pcm = getMediaData().getAudioData(clipBegin, clipEnd);
		try {
			IAudioMediaData data = getPresentation().getMediaDataFactory()
					.create(getMediaData().getXukLocalName(),
							getMediaData().getXukNamespaceURI());
			data.setPCMFormat(getMediaData().getPCMFormat());
			data.appendAudioData(pcm, null);
			copyMAM.setMediaData(data);
			return copyMAM;
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} finally {
			try {
				pcm.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	@Override
	protected void clear() {
		mAudioMediaData = null;
		super.clear();
	}

	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		String uid = source.getAttribute("audioMediaDataUid");
		if (uid == null || uid.length() == 0) {
			throw new XukDeserializationFailedException();
		}
		try {
			if (!getPresentation().getMediaDataManager().isManagerOf(uid)) {
				throw new XukDeserializationFailedException();
			}
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		IMediaData md;
		try {
			md = getPresentation().getMediaDataManager().getMediaData(uid);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (!(md instanceof IAudioMediaData)) {
			throw new XukDeserializationFailedException();
		}
		setMediaData(md);
		super.xukInAttributes(source, ph);
	}

	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeAttributeString("audioMediaDataUid", getMediaData()
				.getUID());
		super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(IMedia other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		IManagedAudioMedia otherMAM = (IManagedAudioMedia) other;
		if (!getMediaData().ValueEquals(otherMAM.getMediaData()))
			return false;
		return true;
	}

	public ITimeDelta getDuration() {
		return getMediaData().getAudioDuration();
	}

	public IManagedAudioMedia split(ITime splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		IAudioMediaData secondPartData;
		try {
			secondPartData = getMediaData().split(splitPoint);
		} catch (FactoryCannotCreateTypeException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		IMedia oSecondPart;
		try {
			oSecondPart = getPresentation().getMediaFactory().create(getXukLocalName(),
					getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		IManagedAudioMedia secondPartMAM = (IManagedAudioMedia) oSecondPart;
		secondPartMAM.setMediaData(secondPartData);
		return secondPartMAM;
	}

	public IAudioMediaData getMediaData() {
		if (mAudioMediaData == null) {
			try {
				setMediaData(getPresentation().getMediaDataFactory().create());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return mAudioMediaData;
	}

	public void setMediaData(IMediaData data)
			throws MethodParameterIsNullException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		if (!(data instanceof IAudioMediaData)) {
			throw new MethodParameterIsWrongTypeException();
		}
		IAudioMediaData prevData = mAudioMediaData;
		mAudioMediaData = (IAudioMediaData) data;
		if (mAudioMediaData != prevData)
			notifyListeners(new MediaDataChangedEvent(this, mAudioMediaData,
					prevData));
	}

	public void mergeWith(IManagedAudioMedia other)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		getMediaData().mergeWith(other.getMediaData());
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
		/**
		 * Does nothing.
		 */
	}
}
