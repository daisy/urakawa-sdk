package org.daisy.urakawa.media.data.audio;

import java.io.IOException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.NameChangedEvent;
import org.daisy.urakawa.event.media.data.audio.AudioDataInsertedEvent;
import org.daisy.urakawa.event.media.data.audio.AudioDataRemovedEvent;
import org.daisy.urakawa.event.media.data.audio.PCMFormatChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.media.data.AbstractMediaData;
import org.daisy.urakawa.media.data.IMediaDataFactory;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.FileStream;
import org.daisy.urakawa.nativeapi.IStream;

/**
 * Partial reference implementation of the interfaces. This abstract class
 * should be extended to support specific audio codecs.
 * 
 * @stereotype Abstract
 */
public abstract class AbstractAudioMediaData extends AbstractMediaData
		implements IAudioMediaData {
	private IPCMFormatInfo mPCMFormat;

	/**
	 * 
	 */
	public AbstractAudioMediaData() {
		super();
	}

	protected IEventHandler<Event> mPCMFormatChangedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mAudioDataInsertedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mAudioDataRemovedEventNotifier = new EventHandler();

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
			IEventListener<K> listener, Class<K> klass)
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
			IEventListener<K> listener, Class<K> klass)
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

	public String isPCMFormatChangeOk(IPCMFormatInfo newFormat)
			throws MethodParameterIsNullException {
		if (newFormat == null) {
			throw new MethodParameterIsNullException();
		}
		String failReason = "";
		try {
			if (getMediaDataManager().getEnforceSinglePCMFormat()) {
				if (!getMediaDataManager().getDefaultPCMFormat().ValueEquals(
						newFormat)) {
					failReason = "When the IMediaDataManager enforces a single PCM Format, "
							+ "the PCM Format of the IAudioMediaData must match the default defined by the manager";
					return failReason; // NOT OK
				}
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return null; // OK
	}

	public IMediaDataFactory getMediaDataFactory()
			throws IsNotInitializedException {
		return getMediaDataManager().getMediaDataFactory();
	}

	public IPCMFormatInfo getPCMFormat() {
		if (mPCMFormat == null) {
			try {
				mPCMFormat = new PCMFormatInfo(getMediaDataManager()
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

	public void setPCMFormat(IPCMFormatInfo newFormat)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (newFormat == null) {
			throw new MethodParameterIsNullException();
		}
		String failReason = isPCMFormatChangeOk(newFormat);
		if (failReason != null) {
			throw new InvalidDataFormatException();
		}
		if (!newFormat.ValueEquals(mPCMFormat)) {
			IPCMFormatInfo prevFormat = mPCMFormat;
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
		IPCMFormatInfo newFormat = getPCMFormat();
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
		IPCMFormatInfo newFormat = getPCMFormat();
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
		IPCMFormatInfo newFormat = getPCMFormat();
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

	public int getPCMLength(ITimeDelta duration)
			throws TimeOffsetIsOutOfBoundsException,
			MethodParameterIsNullException {
		if (duration == null) {
			throw new MethodParameterIsNullException();
		}
		return getPCMFormat().getDataLength(duration);
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

	public abstract ITimeDelta getAudioDuration();

	public IStream getAudioData() {
		try {
			try {
				return getAudioData(new Time().getZero());
			} catch (TimeOffsetIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ?!", e);
			}
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public IStream getAudioData(ITime clipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		try {
			return getAudioData(clipBegin, new Time().getZero().addTimeDelta(
					getAudioDuration()));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	public abstract IStream getAudioData(ITime clipBegin, ITime clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	public void appendAudioData(IStream pcmData, ITimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException {
		if (pcmData == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		insertAudioData(pcmData, new Time(getAudioDuration()
				.getTimeDeltaAsMilliseconds()), duration);
	}

	private ITimeDelta parseRiffWaveStream(IStream riffWaveStream)
			throws InvalidDataFormatException, MethodParameterIsNullException,
			IOException {
		if (riffWaveStream == null) {
			throw new MethodParameterIsNullException();
		}
		IPCMDataInfo pcmInfo = new PCMDataInfo()
				.parseRiffWaveHeader(riffWaveStream);
		if (!pcmInfo.isCompatibleWith(getPCMFormat())) {
			throw new InvalidDataFormatException();
		}
		ITimeDelta duration;
		try {
			duration = new TimeDelta(pcmInfo.getDuration());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
		return duration;
	}

	public void appendAudioDataFromRiffWave(IStream riffWaveStream)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			IOException {
		ITimeDelta duration = parseRiffWaveStream(riffWaveStream);
		try {
			appendAudioData(riffWaveStream, duration);
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	private IStream openFileStream(String path) {
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
		IStream rwFS = openFileStream(path);
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

	public abstract void insertAudioData(IStream pcmData, ITime insertPoint,
			ITimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, TimeOffsetIsOutOfBoundsException;

	public void insertAudioDataFromRiffWave(IStream riffWaveStream,
			ITime insertPoint, ITimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException, IOException {
		if (riffWaveStream == null || insertPoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		ITimeDelta fileDuration = parseRiffWaveStream(riffWaveStream);
		if (fileDuration.isLessThan(duration)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		insertAudioData(riffWaveStream, insertPoint, duration);
	}

	public void insertAudioDataFromRiffWave(String path, ITime insertPoint,
			ITimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, MethodParameterIsEmptyStringException,
			TimeOffsetIsOutOfBoundsException {
		if (path == null || insertPoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		IStream rwFS = openFileStream(path);
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

	public void replaceAudioData(IStream pcmData, ITime replacePoint,
			ITimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, TimeOffsetIsOutOfBoundsException {
		if (pcmData == null || replacePoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		removeAudioData(replacePoint, replacePoint.addTimeDelta(duration));
		insertAudioData(pcmData, replacePoint, duration);
	}

	public void replaceAudioDataFromRiffWave(IStream riffWaveStream,
			ITime replacePoint, ITimeDelta duration)
			throws MethodParameterIsNullException, InvalidDataFormatException,
			TimeOffsetIsOutOfBoundsException, IOException {
		if (riffWaveStream == null || replacePoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		ITimeDelta fileDuration = parseRiffWaveStream(riffWaveStream);
		if (fileDuration.isLessThan(duration)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		replaceAudioData(riffWaveStream, replacePoint, duration);
	}

	public void replaceAudioDataFromRiffWave(String path, ITime replacePoint,
			ITimeDelta duration) throws MethodParameterIsNullException,
			InvalidDataFormatException, MethodParameterIsEmptyStringException,
			TimeOffsetIsOutOfBoundsException {
		if (path == null || replacePoint == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		if (path == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		IStream rwFS = openFileStream(path);
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

	public void removeAudioData(ITime clipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		removeAudioData(clipBegin, new Time().getZero().addTimeDelta(
				getAudioDuration()));
	}

	public abstract void removeAudioData(ITime clipBegin, ITime clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException;

	/**
	 * @return data
	 */
	public abstract IAudioMediaData audioMediaDataCopy();

	@Override
	protected IMediaData copyProtected() {
		return audioMediaDataCopy();
	}

	public IAudioMediaData split(ITime splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException, FactoryCannotCreateTypeException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (splitPoint.isNegativeTimeOffset()) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (splitPoint.isGreaterThan(new Time().getZero().addTimeDelta(
				getAudioDuration()))) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		IMediaData md;
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
		if (!(md instanceof IAudioMediaData)) {
			throw new FactoryCannotCreateTypeException();
		}
		IAudioMediaData secondPartAMD = (IAudioMediaData) md;
		ITimeDelta spDur = new Time().getZero()
				.addTimeDelta(getAudioDuration()).getTimeDelta(splitPoint);
		IStream secondPartAudioStream = getAudioData(splitPoint);
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

	public void mergeWith(IAudioMediaData other)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!getPCMFormat().isCompatibleWith(other.getPCMFormat())) {
			throw new InvalidDataFormatException();
		}
		IStream otherData = other.getAudioData();
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
			other.removeAudioData(new Time().getZero());
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}

	@Override
	public boolean ValueEquals(IMediaData other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (!super.ValueEquals(other))
			return false;
		IAudioMediaData amdOther = (IAudioMediaData) other;
		if (!getPCMFormat().ValueEquals(amdOther.getPCMFormat()))
			return false;
		if (getPCMLength() != amdOther.getPCMLength())
			return false;
		IStream thisData = getAudioData();
		try {
			IStream otherdata = amdOther.getAudioData();
			try {
				if (!new PCMDataInfo().compareStreamData(thisData, otherdata,
						thisData.getLength()))
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
