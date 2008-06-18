package org.daisy.urakawa.media.data.audio;

import java.io.IOException;
import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.media.data.MediaDataChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsWrongTypeException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaAbstractImpl;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.Stream;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ManagedAudioMediaImpl extends MediaAbstractImpl implements
		ManagedAudioMedia {
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
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(klass)) {
			mMediaDataChangedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(klass)) {
			mMediaDataChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	protected EventListener<MediaDataChangedEvent> mMediaDataChangedEventListener = new EventListener<MediaDataChangedEvent>() {
		public <K extends MediaDataChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourceManagedMedia() == ManagedAudioMediaImpl.this) {
				MediaData dataPrevious = event.getPreviousMediaData();
				if (dataPrevious != null) {
					dataPrevious.unregisterListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
				MediaData dataNew = event.getNewMediaData();
				if (dataNew != null) {
					dataNew.registerListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};
	protected EventHandler<Event> mMediaDataChangedEventNotifier = new EventHandlerImpl();

	/**
	 * 
	 */
	public ManagedAudioMediaImpl() {
		try {
			registerListener(mMediaDataChangedEventListener,
					MediaDataChangedEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	private AudioMediaData mAudioMediaData = null;

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
	public ManagedAudioMedia copy() {
		return (ManagedAudioMedia) copyProtected();
	}

	@Override
	protected Media copyProtected() {
		ManagedAudioMedia copyMAM = (ManagedAudioMedia) super.copyProtected();
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
	public ManagedAudioMedia export(Presentation destPres)
			throws MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ManagedAudioMedia) export(destPres);
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ManagedAudioMedia exported = (ManagedAudioMedia) super
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

	public ManagedAudioMedia copy(Time clipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		return copy(clipBegin, new TimeImpl().getZero().addTimeDelta(
				getDuration()));
	}

	public ManagedAudioMedia copy(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null || clipEnd == null) {
			throw new MethodParameterIsNullException();
		}
		ManagedAudioMedia copyMAM;
		try {
			copyMAM = (ManagedAudioMedia) getMediaFactory().createMedia(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		Stream pcm = getMediaData().getAudioData(clipBegin, clipEnd);
		try {
			AudioMediaData data = (AudioMediaData) getMediaDataFactory()
					.createMediaData(getMediaData().getXukLocalName(),
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
	protected void xukInAttributes(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		String uid = source.getAttribute("audioMediaDataUid");
		if (uid == null || uid == "") {
			throw new XukDeserializationFailedException();
		}
		try {
			if (!getMediaDataFactory().getMediaDataManager().isManagerOf(uid)) {
				throw new XukDeserializationFailedException();
			}
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		MediaData md;
		try {
			md = getMediaDataFactory().getMediaDataManager().getMediaData(uid);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (!(md instanceof AudioMediaData)) {
			throw new XukDeserializationFailedException();
		}
		setMediaData(md);
		super.xukInAttributes(source, ph);
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException {
		destination.writeAttributeString("audioMediaDataUid", getMediaData()
				.getUID());
		super.xukOutAttributes(destination, baseUri, ph);
	}

	@Override
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		ManagedAudioMedia otherMAM = (ManagedAudioMedia) other;
		if (!getMediaData().ValueEquals(otherMAM.getMediaData()))
			return false;
		return true;
	}

	public TimeDelta getDuration() {
		return getMediaData().getAudioDuration();
	}

	public ManagedAudioMedia split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		AudioMediaData secondPartData;
		try {
			secondPartData = getMediaData().split(splitPoint);
		} catch (FactoryCannotCreateTypeException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		Media oSecondPart;
		try {
			oSecondPart = getMediaFactory().createMedia(getXukLocalName(),
					getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		ManagedAudioMedia secondPartMAM = (ManagedAudioMedia) oSecondPart;
		secondPartMAM.setMediaData(secondPartData);
		return secondPartMAM;
	}

	public AudioMediaData getMediaData() {
		if (mAudioMediaData == null) {
			try {
				setMediaData(getMediaDataFactory().createAudioMediaData());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return mAudioMediaData;
	}

	public void setMediaData(MediaData data)
			throws MethodParameterIsNullException {
		if (data == null) {
			throw new MethodParameterIsNullException();
		}
		if (!(data instanceof AudioMediaData)) {
			throw new MethodParameterIsWrongTypeException();
		}
		AudioMediaData prevData = mAudioMediaData;
		mAudioMediaData = (AudioMediaData) data;
		if (mAudioMediaData != prevData)
			notifyListeners(new MediaDataChangedEvent(this, mAudioMediaData,
					prevData));
	}

	public MediaDataFactory getMediaDataFactory() {
		try {
			return getMediaFactory().getPresentation().getMediaDataFactory();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public void mergeWith(ManagedAudioMedia other)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		getMediaData().mergeWith(other.getMediaData());
	}
}
