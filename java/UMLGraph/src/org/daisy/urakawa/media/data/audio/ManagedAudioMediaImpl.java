package org.daisy.urakawa.media.data.audio;

import java.io.IOException;
import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.ChangeListener;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.ChangeNotifierImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.media.data.MediaDataChangedEvent;
import org.daisy.urakawa.event.media.data.audio.AudioMediaDataEvent;
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
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(klass)) {
			mMediaDataChangedEventNotifier.registerListener(listener, klass);
		}
		super.registerListener(listener, klass);
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (MediaDataChangedEvent.class.isAssignableFrom(klass)) {
			mMediaDataChangedEventNotifier.unregisterListener(listener, klass);
		}
		super.unregisterListener(listener, klass);
	}

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_AudioMediaDataEventListener(AudioMediaDataEvent event)
			throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<AudioMediaDataEvent> mAudioMediaDataEventListener = new ChangeListener<AudioMediaDataEvent>() {
		@Override
		public <K extends AudioMediaDataEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_AudioMediaDataEventListener(event);
		}
	};

	/**
	 * @param event
	 * @throws MethodParameterIsNullException
	 */
	protected void this_MediaDataChangedEventListener(
			MediaDataChangedEvent event) throws MethodParameterIsNullException {
		notifyListeners(event);
	}

	protected ChangeListener<MediaDataChangedEvent> mMediaDataChangedEventListener = new ChangeListener<MediaDataChangedEvent>() {
		@Override
		public <K extends MediaDataChangedEvent> void changeHappened(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			this_MediaDataChangedEventListener(event);
		}
	};
	protected ChangeNotifier<DataModelChangedEvent> mMediaDataChangedEventNotifier = new ChangeNotifierImpl();

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
			data.appendAudioData(pcm, clipEnd.getTimeDelta(clipBegin));
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
	protected void xukInAttributes(XmlDataReader source)
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
		super.xukInAttributes(source);
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		destination.writeAttributeString("audioMediaDataUid", getMediaData()
				.getUID());
		super.xukOutAttributes(destination, baseUri);
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
		if (mAudioMediaData != null)
			mAudioMediaData.unregisterListener(mAudioMediaDataEventListener,
					AudioMediaDataEvent.class);
		AudioMediaData prevData = mAudioMediaData;
		mAudioMediaData = (AudioMediaData) data;
		mAudioMediaData.registerListener(mAudioMediaDataEventListener,
				AudioMediaDataEvent.class);
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
