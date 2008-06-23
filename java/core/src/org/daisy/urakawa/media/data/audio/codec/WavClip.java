package org.daisy.urakawa.media.data.audio.codec;

import java.io.IOException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.data.DataIsMissingException;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.OutputStreamIsOpenException;
import org.daisy.urakawa.media.data.audio.PCMDataInfo;
import org.daisy.urakawa.media.data.audio.PCMDataInfoImpl;
import org.daisy.urakawa.media.data.utilities.Clip;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeDeltaImpl;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.Stream;
import org.daisy.urakawa.nativeapi.SubStream;

/**
 * 
 * @depend - Aggregation 1 DataProvider
 */
public class WavClip extends Clip implements ValueEquatable<WavClip> {
	/**
	 * @param clipDataProvider
	 * @throws MethodParameterIsNullException
	 */
	public WavClip(DataProvider clipDataProvider)
			throws MethodParameterIsNullException {
		try {
			init(clipDataProvider, new TimeImpl().getZero(), null);
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @param clipDataProvider
	 * @param clipBegin
	 * @param clipEnd
	 * @throws MethodParameterIsNullException
	 * @throws TimeOffsetIsOutOfBoundsException
	 */
	public void init(DataProvider clipDataProvider, Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipDataProvider == null || clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		mDataProvider = clipDataProvider;
		setClipBegin(clipBegin);
		setClipEnd(clipEnd);
	}

	/**
	 * @param clipDataProvider
	 * @param clipBegin
	 * @param clipEnd
	 * @throws MethodParameterIsNullException
	 * @throws TimeOffsetIsOutOfBoundsException
	 */
	public WavClip(DataProvider clipDataProvider, Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipDataProvider == null || clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		init(clipDataProvider, clipBegin, clipEnd);
	}

	@Override
	public TimeDelta getMediaDuration() {
		Stream raw;
		try {
			raw = getDataProvider().getInputStream();
		} catch (DataIsMissingException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (OutputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		PCMDataInfo pcmInfo;
		try {
			pcmInfo = new PCMDataInfoImpl().parseRiffWaveHeader(raw);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IOException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} finally {
			try {
				raw.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		try {
			return new TimeDeltaImpl(pcmInfo.getDuration());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @return a copy
	 */
	public WavClip copy() {
		Time clipEnd = null;
		if (!isClipEndTiedToEOM())
			clipEnd = getClipEnd().copy();
		try {
			return new WavClip(getDataProvider().copy(), getClipBegin().copy(),
					clipEnd);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @param destPres
	 * @return an exported copy
	 * @throws MethodParameterIsNullException
	 * @throws FactoryCannotCreateTypeException
	 */
	public WavClip export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		Time clipEnd = null;
		if (!isClipEndTiedToEOM())
			clipEnd = getClipEnd().copy();
		try {
			return new WavClip(getDataProvider().export(destPres),
					getClipBegin().copy(), clipEnd);
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	private DataProvider mDataProvider;

	/**
	 * @return the data provider
	 */
	public DataProvider getDataProvider() {
		return mDataProvider;
	}

	/**
	 * @return a stream
	 */
	public Stream getAudioData() {
		try {
			return getAudioData(new TimeImpl().getZero());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @param subClipBegin
	 * @return a stream
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws MethodParameterIsNullException
	 */
	public Stream getAudioData(Time subClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		try {
			return getAudioData(subClipBegin, new TimeImpl().getZero()
					.addTimeDelta(getDuration()));
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	/**
	 * @param subClipBegin
	 * @param subClipEnd
	 * @return a stream
	 * @throws MethodParameterIsNullException
	 * @throws TimeOffsetIsOutOfBoundsException
	 * @throws InvalidDataFormatException
	 */
	public Stream getAudioData(Time subClipBegin, Time subClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException, InvalidDataFormatException {
		if (subClipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		if (subClipEnd == null) {
			throw new MethodParameterIsNullException();
		}
		if (subClipBegin.isLessThan(new TimeImpl().getZero())
				|| subClipEnd.isLessThan(subClipBegin)
				|| subClipBegin.addTimeDelta(getDuration()).isLessThan(
						subClipEnd)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		Stream raw;
		try {
			raw = getDataProvider().getInputStream();
		} catch (DataIsMissingException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (OutputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		PCMDataInfo pcmInfo;
		try {
			pcmInfo = new PCMDataInfoImpl().parseRiffWaveHeader(raw);
		} catch (IOException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		Time rawEndTime = new TimeImpl().getZero().addTimeDelta(
				pcmInfo.getDuration());
		if (getClipBegin().isLessThan(new TimeImpl().getZero())
				|| getClipBegin().isGreaterThan(getClipEnd())
				|| getClipEnd().isGreaterThan(rawEndTime)) {
			throw new InvalidDataFormatException();
		}
		Time rawClipBegin = getClipBegin().addTime(subClipBegin);
		Time rawClipEnd = getClipBegin().addTime(subClipEnd);
		long offset;
		int beginPos = raw.getPosition()
				+ (int) ((rawClipBegin.getTimeAsMilliseconds() * pcmInfo
						.getByteRate()) / 1000);
		offset = (beginPos - raw.getPosition()) % pcmInfo.getBlockAlign();
		beginPos -= offset;
		int endPos = raw.getPosition()
				+ (int) ((rawClipEnd.getTimeAsMilliseconds() * pcmInfo
						.getByteRate()) / 1000);
		offset = (endPos - raw.getPosition()) % pcmInfo.getBlockAlign();
		endPos -= offset;
		SubStream res;
		try {
			res = new SubStream(raw, beginPos, endPos - beginPos);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return res;
	}

	public boolean ValueEquals(WavClip other)
			throws MethodParameterIsNullException {
		if (other == null)
			throw new MethodParameterIsNullException();
		if (!getClipBegin().isEqualTo(other.getClipBegin()))
			return false;
		if (isClipEndTiedToEOM() != other.isClipEndTiedToEOM())
			return false;
		if (!getClipEnd().isEqualTo(other.getClipEnd()))
			return false;
		if (!getDataProvider().ValueEquals(other.getDataProvider()))
			return false;
		return true;
	}
}