package org.daisy.urakawa.media.data.audio;

import java.io.IOException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.NameChangedEvent;
import org.daisy.urakawa.event.media.data.audio.AudioDataInsertedEvent;
import org.daisy.urakawa.event.media.data.audio.AudioDataRemovedEvent;
import org.daisy.urakawa.event.media.data.audio.PCMFormatChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataAbstractImpl;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeDeltaImpl;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.Stream;

/**
 * Partial reference implementation of the interfaces. This abstract class
 * should be extended to support specific audio codecs.
 * 
 * @stereotype Abstract
 */
public abstract class AudioMediaDataAbstractImpl extends MediaDataAbstractImpl
		implements AudioMediaData {
	private PCMFormatInfo mPCMFormat;

	/**
	 * 
	 */
	public AudioMediaDataAbstractImpl() {
		super();
	}

	protected EventHandler<DataModelChangedEvent> mPCMFormatChangedEventNotifier = new EventHandlerImpl();
	protected EventHandler<DataModelChangedEvent> mAudioDataInsertedEventNotifier = new EventHandlerImpl();
	protected EventHandler<DataModelChangedEvent> mAudioDataRemovedEventNotifier = new EventHandlerImpl();

	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (NameChangedEvent.class.isAssignableFrom(event.getClass())) {
			mNameChangedEventNotifier.notifyListeners(event);
		}
		if (PCMFormatChangedEvent.class.isAssignableFrom(event.getClass())) {
			mPCMFormatChangedEventNotifier.notifyListeners(event);
		}
		if (AudioDataInsertedEvent.class.isAssignableFrom(event.getClass())) {
			mAudioDataInsertedEventNotifier.notifyListeners(event);
		}
		if (AudioDataRemovedEvent.class.isAssignableFrom(event.getClass())) {
			mAudioDataRemovedEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (NameChangedEvent.class.isAssignableFrom(klass)) {
			mNameChangedEventNotifier.registerListener(listener, klass);
		}
		if (PCMFormatChangedEvent.class.isAssignableFrom(klass)) {
			mPCMFormatChangedEventNotifier.registerListener(listener, klass);
		} else if (AudioDataInsertedEvent.class.isAssignableFrom(klass)) {
			mAudioDataInsertedEventNotifier.registerListener(listener, klass);
		} else if (AudioDataRemovedEvent.class.isAssignableFrom(klass)) {
			mAudioDataRemovedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (NameChangedEvent.class.isAssignableFrom(klass)) {
			mNameChangedEventNotifier.unregisterListener(listener, klass);
		}
		if (PCMFormatChangedEvent.class.isAssignableFrom(klass)) {
			mPCMFormatChangedEventNotifier.unregisterListener(listener, klass);
		} else if (AudioDataInsertedEvent.class.isAssignableFrom(klass)) {
			mAudioDataInsertedEventNotifier.unregisterListener(listener, klass);
		} else if (AudioDataRemovedEvent.class.isAssignableFrom(klass)) {
			mAudioDataRemovedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	public String isPCMFormatChangeOk(PCMFormatInfo newFormat)
			throws MethodParameterIsNullException {
		if (newFormat == null) {
			throw new MethodParameterIsNullException();
		}
		String failReason = "";
		try {
			if (getMediaDataManager().getEnforceSinglePCMFormat()) {
				if (!getMediaDataManager().getDefaultPCMFormat().ValueEquals(
						newFormat)) {
					failReason = "When the MediaDataManager enforces a single PCM Format, "
							+ "the PCM Format of the AudioMediaData must match the default defined by the manager";
					return failReason; // NOT OK
				}
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return null; // OK
	}

	public MediaDataFactory getMediaDataFactory()
			throws IsNotInitializedException {
		return getMediaDataManager().getMediaDataFactory();
	}

	public PCMFormatInfo getPCMFormat() {
		if (mPCMFormat == null) {
			try {
				mPCMFormat = new PCMFormatInfoImpl(getMediaDataManager()
						.getDefaultPCMFormat());
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
		return mPCMFormat.copy();
	}

	public void setPCMFormat(PCMFormatInfo newFormat)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (newFormat == null) {
			throw new MethodParameterIsNullException();
		}
		String failReason = isPCMFormatChangeOk(newFormat);
		if (failReason != null) {
			throw new InvalidDataFormatException();
		}
		if (!newFormat.ValueEquals(mPCMFormat)) {
			PCMFormatInfo prevFormat = mPCMFormat;
			mPCMFormat = newFormat.copy();
			notifyListeners(new PCMFormatChangedEvent(this, mPCMFormat.copy(),
					prevFormat));
		}
	}

	/**
	 * @param numberOfChannels
	 * @throws MethodParameterIsOutOfBoundsException
	 */
	public void setNumberOfChannels(short numberOfChannels)
			throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newFormat = getPCMFormat();
		newFormat.setNumberOfChannels(numberOfChannels);
		try {
			setPCMFormat(newFormat);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public void setSampleRate(int sampleRate)
			throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newFormat = getPCMFormat();
		newFormat.setSampleRate(sampleRate);
		try {
			setPCMFormat(newFormat);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public void setBitDepth(short bitDepth)
			throws MethodParameterIsOutOfBoundsException {
		PCMFormatInfo newFormat = getPCMFormat();
		newFormat.setBitDepth(bitDepth);
		try {
			setPCMFormat(newFormat);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public int getPCMLength(TimeDelta duration)
			throws TimeOffsetIsOutOfBoundsException,
			MethodParameterIsNullException {
		if (duration == null) {
			throw new MethodParameterIsNullException();
		}
		return (int) getPCMFormat().getDataLength(duration);
	}

	public int getPCMLength() {
		try {
			return getPCMLength(getAudioDuration());
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public abstract TimeDelta getAudioDuration();

	public Stream getAudioData() {
		try {
			try {
				return getAudioData(new TimeImpl().getZero());
			} catch (TimeOffsetIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public Stream getAudioData(Time clipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			return getAudioData(clipBegin, new TimeImpl().getZero()
					.addTimeDelta(getAudioDuration()));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public abstract Stream getAudioData(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	public void appendAudioData(Stream pcmData, TimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException {
		if (pcmData == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		insertAudioData(pcmData, new TimeImpl(getAudioDuration()
				.getTimeDeltaAsMilliseconds()), duration);
	}

	private TimeDelta parseRiffWaveStream(Stream riffWaveStream)
			throws InvalidDataFormatException, MethodParameterIsNullException,
			IOException {
		if (riffWaveStream == null) {
			throw new MethodParameterIsNullException();
		}
		PCMDataInfo pcmInfo = new PCMDataInfoImpl()
				.parseRiffWaveHeader(riffWaveStream);
		if (!pcmInfo.isCompatibleWith(getPCMFormat())) {
			throw new InvalidDataFormatException();
		}
		TimeDelta duration;
		try {
			duration = new TimeDeltaImpl(pcmInfo.getDuration());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return duration;
	}

	public void appendAudioDataFromRiffWave(Stream riffWaveStream)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			IOException {
		TimeDelta duration = parseRiffWaveStream(riffWaveStream);
		try {
			appendAudioData(riffWaveStream, duration);
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	private Stream openFileStream(String path) {
		return new FileStream(path);
	}

	public void appendAudioDataFromRiffWave(String path)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException, InvalidDataFormatException {
		if (path == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		Stream rwFS = openFileStream(path);
		try {
			appendAudioDataFromRiffWave(rwFS);
		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			try {
				rwFS.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
	}

	public abstract void insertAudioData(Stream pcmData, Time insertPoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

	public void insertAudioDataFromRiffWave(Stream riffWaveStream,
			Time insertPoint, TimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException, IOException {
		if (riffWaveStream == null || insertPoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		TimeDelta fileDuration = parseRiffWaveStream(riffWaveStream);
		if (fileDuration.isLessThan(duration)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		insertAudioData(riffWaveStream, insertPoint, duration);
	}

	public void insertAudioDataFromRiffWave(String path, Time insertPoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, MethodParameterIsEmptyStringException,
			TimeOffsetIsOutOfBoundsException {
		if (path == null || insertPoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		Stream rwFS = openFileStream(path);
		try {
			insertAudioDataFromRiffWave(rwFS, insertPoint, duration);
		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			try {
				rwFS.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
	}

	public void replaceAudioData(Stream pcmData, Time replacePoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, TimeOffsetIsOutOfBoundsException {
		if (pcmData == null || replacePoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		removeAudioData(replacePoint, replacePoint.addTimeDelta(duration));
		insertAudioData(pcmData, replacePoint, duration);
	}

	public void replaceAudioDataFromRiffWave(Stream riffWaveStream,
			Time replacePoint, TimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException, IOException {
		if (riffWaveStream == null || replacePoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		TimeDelta fileDuration = parseRiffWaveStream(riffWaveStream);
		if (fileDuration.isLessThan(duration)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		replaceAudioData(riffWaveStream, replacePoint, duration);
	}

	public void replaceAudioDataFromRiffWave(String path, Time replacePoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, MethodParameterIsEmptyStringException,
			TimeOffsetIsOutOfBoundsException {
		if (path == null || replacePoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		Stream rwFS = openFileStream(path);
		try {
			replaceAudioDataFromRiffWave(rwFS, replacePoint, duration);
		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			try {
				rwFS.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
	}

	public void removeAudioData(Time clipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		removeAudioData(clipBegin, new TimeImpl().getZero().addTimeDelta(
				getAudioDuration()));
	}

	public abstract void removeAudioData(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * @return data
	 */
	public abstract AudioMediaData audioMediaDataCopy();

	@Override
	protected MediaData copyProtected() {
		return audioMediaDataCopy();
	}

	public AudioMediaData split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException, FactoryCannotCreateTypeException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (splitPoint.isNegativeTimeOffset()) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (splitPoint.isGreaterThan(new TimeImpl().getZero().addTimeDelta(
				getAudioDuration()))) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		MediaData md;
		try {
			md = getMediaDataFactory().createMediaData(getXukLocalName(),
					getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e1) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e1);
		} catch (IsNotInitializedException e1) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e1);
		}
		if (!(md instanceof AudioMediaData)) {
			throw new FactoryCannotCreateTypeException();
		}
		AudioMediaData secondPartAMD = (AudioMediaData) md;
		TimeDelta spDur = new TimeImpl().getZero().addTimeDelta(
				getAudioDuration()).getTimeDelta(splitPoint);
		Stream secondPartAudioStream = getAudioData(splitPoint);
		try {
			secondPartAMD.appendAudioData(secondPartAudioStream, spDur);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} finally {
			try {
				secondPartAudioStream.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
		removeAudioData(splitPoint);
		return secondPartAMD;
	}

	public void mergeWith(AudioMediaData other)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!getPCMFormat().isCompatibleWith(other.getPCMFormat())) {
			throw new InvalidDataFormatException();
		}
		Stream otherData = other.getAudioData();
		try {
			appendAudioData(otherData, other.getAudioDuration());
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		} finally {
			try {
				otherData.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
		try {
			other.removeAudioData(new TimeImpl().getZero());
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	@Override
	public boolean ValueEquals(MediaData other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		AudioMediaData amdOther = (AudioMediaData) other;
		if (!getPCMFormat().ValueEquals(amdOther.getPCMFormat()))
			return false;
		if (getPCMLength() != amdOther.getPCMLength())
			return false;
		Stream thisData = getAudioData();
		try {
			Stream otherdata = amdOther.getAudioData();
			try {
				if (!new PCMDataInfoImpl().compareStreamData(thisData,
						otherdata, (int) thisData.getLength()))
					return false;
			} catch (IOException e) {
				e.printStackTrace();
			} finally {
				try {
					otherdata.close();
				} catch (IOException e) {
					// Should never happen
					throw new RuntimeException("WTF ?!", e);
				}
			}
		} finally {
			try {
				thisData.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		}
		return true;
	}
}
