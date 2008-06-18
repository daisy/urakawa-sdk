package org.daisy.urakawa.media.data.audio.codec;

import java.io.IOException;
import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.media.data.audio.AudioDataInsertedEvent;
import org.daisy.urakawa.event.media.data.audio.AudioDataRemovedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.DataIsMissingException;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.FileDataProviderFactoryImpl;
import org.daisy.urakawa.media.data.FileDataProviderManagerImpl;
import org.daisy.urakawa.media.data.InputStreamIsOpenException;
import org.daisy.urakawa.media.data.InputStreamIsTooShortException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.OutputStreamIsOpenException;
import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.AudioMediaDataAbstractImpl;
import org.daisy.urakawa.media.data.audio.PCMDataInfo;
import org.daisy.urakawa.media.data.audio.PCMDataInfoImpl;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;
import org.daisy.urakawa.media.data.audio.PCMFormatInfoImpl;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeDeltaImpl;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsNegativeException;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.SequenceStream;
import org.daisy.urakawa.nativeapi.Stream;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Concrete implementation for RIFF-based audio.
 * 
 * @depend - Composition 0..n WavClip
 */
public class WavAudioMediaData extends AudioMediaDataAbstractImpl {
	private List<WavClip> mWavClips = new LinkedList<WavClip>();

	@Override
	public String isPCMFormatChangeOk(PCMFormatInfo newFormat)
			throws MethodParameterIsNullException {
		if (newFormat == null) {
			throw new MethodParameterIsNullException();
		}
		String msg = super.isPCMFormatChangeOk(newFormat);
		if (msg != null)
			return msg;
		if (mWavClips.size() > 0) {
			if (!getPCMFormat().ValueEquals(newFormat)) {
				return "Cannot change the PCMFormat of the WavAudioMediaData after audio dat has been added to it";
			}
		}
		return null;
	}

	/**
	 * @param pcmData
	 * @return clip
	 * @throws MethodParameterIsNullException
	 */
	public WavClip createWavClipFromRawPCMStream(Stream pcmData)
			throws MethodParameterIsNullException {
		if (pcmData == null) {
			throw new MethodParameterIsNullException();
		}
		return createWavClipFromRawPCMStream(pcmData, null);
	}

	protected WavClip createWavClipFromRawPCMStream(Stream pcmData,
			TimeDelta duration) throws MethodParameterIsNullException {
		if (pcmData == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		DataProvider newSingleDataProvider;
		try {
			newSingleDataProvider = getMediaDataManager()
					.getDataProviderFactory().createDataProvider(
							FileDataProviderFactoryImpl.AUDIO_WAV_MIME_TYPE);
		} catch (MethodParameterIsEmptyStringException e2) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e2);
		} catch (IsNotInitializedException e2) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e2);
		}
		PCMDataInfo pcmInfo = new PCMDataInfoImpl(getPCMFormat());
		if (duration == null) {
			pcmInfo.setDataLength((int) (pcmData.getLength() - pcmData
					.getPosition()));
		} else {
			try {
				pcmInfo.setDataLength(pcmInfo.getDataLength(duration));
			} catch (TimeOffsetIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		Stream nsdps;
		try {
			nsdps = newSingleDataProvider.getOutputStream();
		} catch (OutputStreamIsOpenException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		} catch (InputStreamIsOpenException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		} catch (DataIsMissingException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		}
		try {
			pcmInfo.writeRiffWaveHeader(nsdps);
		} catch (IOException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} finally {
			try {
				nsdps.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		try {
			new FileDataProviderManagerImpl().appendDataToProvider(pcmData,
					(int) pcmInfo.getDataLength(), newSingleDataProvider);
		} catch (OutputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InputStreamIsOpenException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (DataIsMissingException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IOException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (InputStreamIsTooShortException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		WavClip newSingleWavClip = new WavClip(newSingleDataProvider);
		return newSingleWavClip;
	}

	/**
	 * 
	 */
	public void forceSingleDataProvider() {
		Stream audioData = getAudioData();
		WavClip newSingleClip;
		try {
			newSingleClip = createWavClipFromRawPCMStream(audioData);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} finally {
			try {
				audioData.close();
			} catch (IOException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		mWavClips.clear();
		mWavClips.add(newSingleClip);
	}

	@Override
	public AudioMediaData audioMediaDataCopy() {
		return copy();
	}

	@Override
	public WavAudioMediaData copy() {
		WavAudioMediaData copy;
		try {
			copy = (WavAudioMediaData) getMediaDataFactory().createMediaData(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		for (WavClip clip : mWavClips) {
			copy.mWavClips.add(clip.copy());
		}
		return copy;
	}

	@Override
	protected MediaData protectedExport(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return export(destPres);
	}

	@Override
	public WavAudioMediaData export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		WavAudioMediaData expWAMD;
		try {
			expWAMD = (WavAudioMediaData) destPres.getMediaDataFactory()
					.createMediaData(getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (expWAMD == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			expWAMD.setPCMFormat(getPCMFormat());
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		for (WavClip clip : mWavClips) {
			expWAMD.mWavClips.add(clip.export(destPres));
		}
		return expWAMD;
	}

	@Override
	public void delete() {
		mWavClips.clear();
		super.delete();
	}

	@Override
	public List<DataProvider> getListOfUsedDataProviders() {
		List<DataProvider> usedDP = new LinkedList<DataProvider>();
		for (WavClip clip : mWavClips) {
			if (!usedDP.contains(clip.getDataProvider()))
				usedDP.add(clip.getDataProvider());
		}
		return usedDP;
	}

	@Override
	public Stream getAudioData(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null || clipEnd == null) {
			throw new MethodParameterIsNullException();
		}
		if (clipBegin.isNegativeTimeOffset()) {
			throw new TimeOffsetIsNegativeException();
		}
		if (clipEnd.isLessThan(clipBegin)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (clipEnd.isGreaterThan(new TimeImpl().getZero().addTimeDelta(
				getAudioDuration()))) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		// Time timeBeforeStartIndexClip = new TimeImpl();
		// Time timeBeforeEndIndexClip = new TimeImpl();
		Time elapsedTime = new TimeImpl();
		int i = 0;
		List<Stream> resStreams = new LinkedList<Stream>();
		while (i < mWavClips.size()) {
			WavClip curClip = mWavClips.get(i);
			TimeDelta currentClipDuration = curClip.getDuration();
			Time newElapsedTime = elapsedTime.addTimeDelta(currentClipDuration);
			if (newElapsedTime.isLessThan(clipBegin)) {
				// Do nothing - the current clip and the [clipBegin;clipEnd] are
				// disjunkt
			} else if (elapsedTime.isLessThan(clipBegin)) {
				if (newElapsedTime.isLessThan(clipEnd)) {
					// Add part of current clip between clipBegin and
					// newElapsedTime
					// (ie. after clipBegin, since newElapsedTime is at the end
					// of the clip)
					resStreams.add(curClip.getAudioData(new TimeImpl()
							.getZero().addTimeDelta(
									clipBegin.getTimeDelta(elapsedTime))));
				} else {
					// Add part of current clip between clipBegin and clipEnd
					try {
						resStreams.add(curClip.getAudioData(new TimeImpl()
								.getZero().addTimeDelta(
										clipBegin.getTimeDelta(elapsedTime)),
								new TimeImpl().getZero().addTimeDelta(
										clipEnd.getTimeDelta(elapsedTime))));
					} catch (InvalidDataFormatException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
			} else if (elapsedTime.isLessThan(clipEnd)) {
				if (newElapsedTime.isLessThan(clipEnd)) {
					// Add part of current clip between elapsedTime and
					// newElapsedTime
					// (ie. entire clip since elapsedTime and newElapsedTime is
					// at
					// the beginning and end of the clip respectively)
					resStreams.add(curClip.getAudioData());
				} else {
					// Add part of current clip between elapsedTime and clipEnd
					// (ie. before clipEnd since elapsedTime is at the beginning
					// of the clip)
					try {
						resStreams.add(curClip.getAudioData(new TimeImpl()
								.getZero(),
								new TimeImpl().getZero().addTimeDelta(
										clipEnd.getTimeDelta(elapsedTime))));
					} catch (InvalidDataFormatException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
			} else {
				// The current clip and all remaining clips are beyond clipEnd
				break;
			}
			elapsedTime = newElapsedTime;
			i++;
		}
		if (resStreams.size() == 0) {
			return null; // TODO: new MemoryStream(0);
		}
		return new SequenceStream(resStreams);
	}

	@Override
	public void appendAudioData(Stream pcmData, TimeDelta duration)
			throws MethodParameterIsNullException {
		if (pcmData == null || duration == null) {
			throw new MethodParameterIsNullException();
		}
		Time insertPoint = new TimeImpl().getZero().addTimeDelta(
				getAudioDuration());
		WavClip newAppClip = createWavClipFromRawPCMStream(pcmData, duration);
		mWavClips.add(newAppClip);
		notifyListeners(new AudioDataInsertedEvent(this, insertPoint,
				(duration == null ? newAppClip.getMediaDuration() : duration)));
	}

	@Override
	public void insertAudioData(Stream pcmData, Time insertPoint,
			TimeDelta duration) throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		Time insPt = insertPoint.copy();
		if (insPt.isLessThan(new TimeImpl().getZero())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		WavClip newInsClip = createWavClipFromRawPCMStream(pcmData, duration);
		Time endTime = new TimeImpl().getZero()
				.addTimeDelta(getAudioDuration());
		if (insertPoint.isGreaterThan(endTime)) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (insertPoint.isEqualTo(endTime)) {
			mWavClips.add(newInsClip);
			return;
		}
		Time elapsedTime = new TimeImpl().getZero();
		int clipIndex = 0;
		while (clipIndex < mWavClips.size()) {
			WavClip curClip = mWavClips.get(clipIndex);
			if (insPt.isEqualTo(elapsedTime)) {
				// If the insert point at the beginning of the current clip,
				// insert the new clip and break
				mWavClips.add(clipIndex, newInsClip);
				break;
			} else if (insPt.isLessThan(elapsedTime.addTimeDelta(curClip
					.getDuration()))) {
				// If the insert point is between the beginning and end of the
				// current clip,
				// Replace the current clip with three clips containing
				// the audio in the current clip before the insert point,
				// the audio to be inserted and the audio in the current clip
				// after the insert point respectively
				Time insPtInCurClip = new TimeImpl().getZero().addTimeDelta(
						insPt.getTimeDelta(elapsedTime));
				Stream audioDataStream;
				try {
					audioDataStream = curClip.getAudioData(new TimeImpl()
							.getZero(), insPtInCurClip);
				} catch (InvalidDataFormatException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
				WavClip curClipBeforeIns, curClipAfterIns;
				try {
					curClipBeforeIns = createWavClipFromRawPCMStream(audioDataStream);
				} finally {
					try {
						audioDataStream.close();
					} catch (IOException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
				audioDataStream = curClip.getAudioData(insPtInCurClip);
				try {
					curClipAfterIns = createWavClipFromRawPCMStream(audioDataStream);
				} finally {
					try {
						audioDataStream.close();
					} catch (IOException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
				mWavClips.remove(clipIndex);
				List<WavClip> list = new LinkedList<WavClip>();
				list.add(curClipBeforeIns);
				list.add(newInsClip);
				list.add(curClipAfterIns);
				mWavClips.addAll(clipIndex, list);
				break;
			}
			elapsedTime = elapsedTime.addTimeDelta(curClip.getDuration());
			clipIndex++;
		}
		notifyListeners(new AudioDataInsertedEvent(this, insertPoint, duration));
	}

	@Override
	public TimeDelta getAudioDuration() {
		TimeDelta dur = new TimeDeltaImpl();
		for (WavClip clip : mWavClips) {
			try {
				dur.addTimeDelta(clip.getDuration());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return dur;
	}

	@Override
	public void removeAudioData(Time clipBegin)
			throws TimeOffsetIsOutOfBoundsException,
			MethodParameterIsNullException {
		if (clipBegin == null) {
			throw new MethodParameterIsNullException();
		}
		if (clipBegin.isGreaterThan(new TimeImpl()
				.addTimeDelta(getAudioDuration()))
				|| clipBegin.isLessThan(new TimeImpl().getZero())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (clipBegin == new TimeImpl().getZero()) {
			TimeDelta prevDur = getAudioDuration();
			mWavClips.clear();
			notifyListeners(new AudioDataRemovedEvent(this, clipBegin, prevDur));
		} else {
			super.removeAudioData(clipBegin);
		}
	}

	@Override
	public void removeAudioData(Time clipBegin, Time clipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (clipBegin == null || clipEnd == null) {
			throw new MethodParameterIsNullException();
		}
		if (clipBegin.isLessThan(new TimeImpl().getZero())
				|| clipBegin.isGreaterThan(clipEnd)
				|| clipEnd.isGreaterThan(new TimeImpl().getZero().addTimeDelta(
						getAudioDuration()))) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (clipBegin.isEqualTo(new TimeImpl().getZero())
				&& clipEnd.isEqualTo(new TimeImpl(getAudioDuration()
						.getTimeDeltaAsMilliseconds()))) {
			mWavClips.clear();
			notifyListeners(new AudioDataRemovedEvent(this, new TimeImpl()
					.getZero(), getAudioDuration()));
			return;
		}
		Time curBeginTime = new TimeImpl().getZero();
		List<WavClip> newClipList = new LinkedList<WavClip>();
		for (WavClip curClip : mWavClips) {
			Time curEndTime = curBeginTime.addTimeDelta(curClip.getDuration());
			if ((!curEndTime.isGreaterThan(clipBegin))
					|| (!curBeginTime.isLessThan(clipEnd))) {
				// The current clip is before or beyond the range to remove -
				// so the clip is added unaltered to the new list of clips
				newClipList.add(curClip);
			} else if (curBeginTime.isLessThan(clipBegin)
					&& curEndTime.isGreaterThan(clipEnd)) {
				// Some of the current clip is before the range and some is
				// after
				TimeDelta beforePartDur = curBeginTime.getTimeDelta(clipBegin);
				TimeDelta beyondPartDur = curEndTime.getTimeDelta(clipEnd);
				Stream beyondAS = curClip.getAudioData(curClip.getClipEnd()
						.subtractTimeDelta(beyondPartDur));
				WavClip beyondPartClip;
				try {
					beyondPartClip = createWavClipFromRawPCMStream(beyondAS);
				} finally {
					try {
						beyondAS.close();
					} catch (IOException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
				curClip.setClipEnd(curClip.getClipBegin().addTimeDelta(
						beforePartDur));
				newClipList.add(curClip);
				newClipList.add(beyondPartClip);
			} else if (curBeginTime.isLessThan(clipBegin)
					&& curEndTime.isGreaterThan(clipBegin)) {
				// Some of the current clip is before the range to remove, none
				// is beyond
				TimeDelta beforePartDur = curBeginTime.getTimeDelta(clipBegin);
				curClip.setClipEnd(curClip.getClipBegin().addTimeDelta(
						beforePartDur));
				newClipList.add(curClip);
			} else if (curBeginTime.isLessThan(clipEnd)
					&& curEndTime.isGreaterThan(clipEnd)) {
				// Some of the current clip is beyond the range to remove, none
				// is before
				TimeDelta beyondPartDur = curEndTime.getTimeDelta(clipEnd);
				curClip.setClipBegin(curClip.getClipEnd().subtractTimeDelta(
						beyondPartDur));
				newClipList.add(curClip);
			} else {
				// All of the current clip is within the range to remove,
				// so this clip is not added to the new list of WavClips
			}
			curBeginTime = curEndTime;
		}
		mWavClips = newClipList;
		notifyListeners(new AudioDataRemovedEvent(this, clipBegin, clipEnd
				.getTimeDelta(clipBegin)));
		/*
		 * TimeDelta dur = getAudioDuration(); new
		 * TimeDeltaImpl(dur.getTimeDeltaAsMilliseconds() -
		 * clipEnd.getTimeAsMilliseconds() - clipBegin.getTimeAsMilliseconds()))
		 */
	}

	@Override
	protected void clear() {
		mWavClips.clear();
		super.clear();
	}

	@Override
	protected void xukInChild(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAble.XUK_NS) {
			readItem = true;
			if (source.getLocalName() == "mWavClips") {
				xukInWavClips(source);
			} else if (source.getLocalName() == "mPCMFormat") {
				xukInPCMFormat(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem)
			super.xukInChild(source, ph);
	}

	private void xukInPCMFormat(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == "PCMFormatInfo"
							&& source.getNamespaceURI() == XukAble.XUK_NS) {
						PCMFormatInfo newInfo = new PCMFormatInfoImpl();
						newInfo.xukIn(source, ph);
						try {
							setPCMFormat(newInfo);
						} catch (InvalidDataFormatException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
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

	private void xukInWavClips(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == "WavClip"
							&& source.getNamespaceURI() == XukAble.XUK_NS) {
						xukInWavClip(source);
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

	private void xukInWavClip(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String clipBeginAttr = source.getAttribute("clipBegin");
		Time cb = new TimeImpl().getZero();
		if (clipBeginAttr != null) {
			try {
				cb = new TimeImpl(clipBeginAttr);
			} catch (Exception e) {
				throw new XukDeserializationFailedException();
			}
		}
		String clipEndAttr = source.getAttribute("clipEnd");
		Time ce = null;
		if (clipEndAttr != null) {
			try {
				ce = new TimeImpl(clipEndAttr);
			} catch (Exception e) {
				throw new XukDeserializationFailedException();
			}
		}
		String dataProviderUid = source.getAttribute("dataProvider");
		if (dataProviderUid == null) {
			throw new XukDeserializationFailedException();
		}
		DataProvider prov;
		try {
			prov = getMediaDataManager().getPresentation()
					.getDataProviderManager().getDataProvider(dataProviderUid);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			mWavClips.add(new WavClip(prov, cb, ce));
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (!source.isEmptyElement()) {
			source.readSubtree().close();
		}
	}

	@Override
	protected void xukOutChildren(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		super.xukOutChildren(destination, baseUri, ph);
		destination.writeStartElement("mPCMFormat", XukAble.XUK_NS);
		getPCMFormat().xukOut(destination, baseUri, ph);
		destination.writeEndElement();
		destination.writeStartElement("mWavClips", XukAble.XUK_NS);
		for (WavClip clip : mWavClips) {
			destination.writeStartElement("WavClip", XukAble.XUK_NS);
			destination.writeAttributeString("dataProvider", clip
					.getDataProvider().getUid());
			destination.writeAttributeString("clipBegin", clip.getClipBegin()
					.toString());
			if (!clip.isClipEndTiedToEOM())
				destination.writeAttributeString("clipEnd", clip.getClipEnd()
						.toString());
			destination.writeEndElement();
		}
		destination.writeEndElement();
	}

	@Override
	public void mergeWith(AudioMediaData other)
			throws MethodParameterIsNullException, InvalidDataFormatException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (other instanceof WavAudioMediaData) {
			if (!getPCMFormat().isCompatibleWith(other.getPCMFormat())) {
				throw new InvalidDataFormatException();
			}
			Time thisInsertPoint = new TimeImpl().getZero().addTimeDelta(
					getAudioDuration());
			WavAudioMediaData otherWav = (WavAudioMediaData) other;
			mWavClips.addAll(otherWav.mWavClips);
			TimeDelta dur = otherWav.getAudioDuration();
			notifyListeners(new AudioDataInsertedEvent(this, thisInsertPoint,
					dur));
			try {
				otherWav.removeAudioData(new TimeImpl().getZero());
			} catch (TimeOffsetIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			super.mergeWith(other);
		}
	}

	@Override
	public AudioMediaData split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException, FactoryCannotCreateTypeException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (splitPoint.isNegativeTimeOffset()) {
			throw new TimeOffsetIsNegativeException();
		}
		if (splitPoint.isGreaterThan(new TimeImpl().getZero().addTimeDelta(
				getAudioDuration()))) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		WavAudioMediaData oWAMD;
		try {
			oWAMD = (WavAudioMediaData) getMediaDataFactory().createMediaData(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (oWAMD == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			oWAMD.setPCMFormat(getPCMFormat());
		} catch (InvalidDataFormatException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		TimeDelta dur = new TimeImpl().getZero().addTimeDelta(
				getAudioDuration()).getTimeDelta(splitPoint);
		Time elapsed = new TimeImpl().getZero();
		List<WavClip> clips = new LinkedList<WavClip>(mWavClips);
		mWavClips.clear();
		oWAMD.mWavClips.clear();
		for (int i = 0; i < clips.size(); i++) {
			WavClip curClip = clips.get(i);
			Time endCurClip = elapsed.addTimeDelta(curClip.getDuration());
			if (splitPoint.isLessThanOrEqualTo(elapsed)) {
				oWAMD.mWavClips.add(curClip);
			} else if (splitPoint.isLessThan(endCurClip)) {
				WavClip secondPartClip = new WavClip(curClip.getDataProvider(),
						curClip.getClipBegin(),
						curClip.isClipEndTiedToEOM() ? null : curClip
								.getClipEnd());
				curClip.setClipEnd(curClip.getClipBegin().addTime(
						splitPoint.subtractTime(elapsed)));
				secondPartClip.setClipBegin(curClip.getClipEnd());
				mWavClips.add(curClip);
				oWAMD.mWavClips.add(secondPartClip);
			} else {
				mWavClips.add(curClip);
			}
			elapsed = elapsed.addTimeDelta(curClip.getDuration());
		}
		notifyListeners(new AudioDataRemovedEvent(this, splitPoint, dur));
		oWAMD.notifyListeners(new AudioDataInsertedEvent(oWAMD, new TimeImpl()
				.getZero(), dur));
		return oWAMD;
	}
}
