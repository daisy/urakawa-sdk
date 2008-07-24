package org.daisy.urakawa.media.data.audio.codec;

import java.io.IOException;
import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.events.media.data.audio.AudioDataInsertedEvent;
import org.daisy.urakawa.events.media.data.audio.AudioDataRemovedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.DataIsMissingException;
import org.daisy.urakawa.media.data.IDataProvider;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.InputStreamIsOpenException;
import org.daisy.urakawa.media.data.InputStreamIsTooShortException;
import org.daisy.urakawa.media.data.InvalidDataFormatException;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.media.data.OutputStreamIsOpenException;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.AbstractAudioMediaData;
import org.daisy.urakawa.media.data.audio.IPCMDataInfo;
import org.daisy.urakawa.media.data.audio.PCMDataInfo;
import org.daisy.urakawa.media.data.audio.IPCMFormatInfo;
import org.daisy.urakawa.media.data.audio.PCMFormatInfo;
import org.daisy.urakawa.media.timing.ITime;
import org.daisy.urakawa.media.timing.ITimeDelta;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeOffsetIsNegativeException;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.nativeapi.SequenceStream;
import org.daisy.urakawa.nativeapi.IStream;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Concrete implementation for RIFF-based audio.
 * 
 * @depend - Composition 0..n WavClip
 */
public class WavAudioMediaData extends AbstractAudioMediaData
{
    private List<WavClip> mWavClips = new LinkedList<WavClip>();

    @Override
    public String isPCMFormatChangeOk(IPCMFormatInfo newFormat)
            throws MethodParameterIsNullException
    {
        if (newFormat == null)
        {
            throw new MethodParameterIsNullException();
        }
        String msg = super.isPCMFormatChangeOk(newFormat);
        if (msg != null)
            return msg;
        if (mWavClips.size() > 0)
        {
            if (!getPCMFormat().ValueEquals(newFormat))
            {
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
    public WavClip createWavClipFromRawPCMStream(IStream pcmData)
            throws MethodParameterIsNullException
    {
        if (pcmData == null)
        {
            throw new MethodParameterIsNullException();
        }
        return createWavClipFromRawPCMStream(pcmData, null);
    }

    /**
     * @param pcmData
     * @param duration can be null, in which case the current pcmData position
     *        will be used
     * @return
     * @throws MethodParameterIsNullException
     */
    protected WavClip createWavClipFromRawPCMStream(IStream pcmData,
            ITimeDelta duration) throws MethodParameterIsNullException
    {
        if (pcmData == null)
        {
            throw new MethodParameterIsNullException();
        }
        IDataProvider newSingleDataProvider;
        try
        {
            newSingleDataProvider = getPresentation().getDataProviderFactory()
                    .create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
        }
        catch (MethodParameterIsEmptyStringException e2)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e2);
        }
        catch (IsNotInitializedException e2)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e2);
        }
        IPCMDataInfo pcmInfo = new PCMDataInfo(getPCMFormat());
        if (duration == null)
        {
            pcmInfo.setDataLength(pcmData.getLength() - pcmData.getPosition());
        }
        else
        {
            try
            {
                pcmInfo.setDataLength(pcmInfo.getDataLength(duration));
            }
            catch (TimeOffsetIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        IStream nsdps;
        try
        {
            nsdps = newSingleDataProvider.getOutputStream();
        }
        catch (OutputStreamIsOpenException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        catch (InputStreamIsOpenException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        catch (DataIsMissingException e1)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e1);
        }
        try
        {
            pcmInfo.writeRiffWaveHeader(nsdps);
        }
        catch (IOException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        finally
        {
            try
            {
                nsdps.close();
            }
            catch (IOException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        try
        {
            new DataProviderManager().appendDataToProvider(pcmData, pcmInfo
                    .getDataLength(), newSingleDataProvider);
        }
        catch (OutputStreamIsOpenException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InputStreamIsOpenException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (DataIsMissingException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IOException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (InputStreamIsTooShortException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        WavClip newSingleWavClip = new WavClip(newSingleDataProvider);
        return newSingleWavClip;
    }

    /**
	 * 
	 */
    public void forceSingleDataProvider()
    {
        IStream audioData = getAudioData();
        WavClip newSingleClip;
        try
        {
            newSingleClip = createWavClipFromRawPCMStream(audioData);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        finally
        {
            try
            {
                audioData.close();
            }
            catch (IOException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        mWavClips.clear();
        mWavClips.add(newSingleClip);
    }

    @Override
    public IAudioMediaData audioMediaDataCopy()
    {
        return copy();
    }

    @Override
    public WavAudioMediaData copy()
    {
        WavAudioMediaData copy;
        try
        {
            copy = getPresentation().getMediaDataFactory().create(
                    getXukLocalName(), getXukNamespaceURI());
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        for (WavClip clip : mWavClips)
        {
            copy.mWavClips.add(clip.copy());
        }
        return copy;
    }

    @Override
    protected IMediaData protectedExport(Presentation destPres)
            throws MethodParameterIsNullException,
            FactoryCannotCreateTypeException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        return export(destPres);
    }

    @Override
    public WavAudioMediaData export(Presentation destPres)
            throws MethodParameterIsNullException,
            FactoryCannotCreateTypeException
    {
        if (destPres == null)
        {
            throw new MethodParameterIsNullException();
        }
        WavAudioMediaData expWAMD;
        try
        {
            expWAMD = destPres.getMediaDataFactory().create(getXukLocalName(),
                    getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (expWAMD == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            expWAMD.setPCMFormat(getPCMFormat());
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        for (WavClip clip : mWavClips)
        {
            expWAMD.mWavClips.add(clip.export(destPres));
        }
        return expWAMD;
    }

    @Override
    public void delete()
    {
        mWavClips.clear();
        super.delete();
    }

    @Override
    public List<IDataProvider> getListOfUsedDataProviders()
    {
        List<IDataProvider> usedDP = new LinkedList<IDataProvider>();
        for (WavClip clip : mWavClips)
        {
            if (!usedDP.contains(clip.getDataProvider()))
                usedDP.add(clip.getDataProvider());
        }
        return usedDP;
    }

    @Override
    public IStream getAudioData(ITime clipBegin, ITime clipEnd)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (clipBegin == null || clipEnd == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (clipBegin.isNegativeTimeOffset())
        {
            throw new TimeOffsetIsNegativeException();
        }
        if (clipEnd.isLessThan(clipBegin))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (clipEnd.isGreaterThan(new Time().getZero().addTimeDelta(
                getAudioDuration())))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        // ITime timeBeforeStartIndexClip = new Time();
        // ITime timeBeforeEndIndexClip = new Time();
        ITime elapsedTime = new Time();
        int i = 0;
        List<IStream> resStreams = new LinkedList<IStream>();
        while (i < mWavClips.size())
        {
            WavClip curClip = mWavClips.get(i);
            ITimeDelta currentClipDuration = curClip.getDuration();
            ITime newElapsedTime = elapsedTime
                    .addTimeDelta(currentClipDuration);
            if (newElapsedTime.isLessThan(clipBegin))
            {
                // Do nothing - the current clip and the [clipBegin;clipEnd] are
                // disjunkt
            }
            else
                if (elapsedTime.isLessThan(clipBegin))
                {
                    if (newElapsedTime.isLessThan(clipEnd))
                    {
                        // Add part of current clip between clipBegin and
                        // newElapsedTime
                        // (ie. after clipBegin, since newElapsedTime is at the
                        // end
                        // of the clip)
                        resStreams.add(curClip.getAudioData(new Time()
                                .getZero().addTimeDelta(
                                        clipBegin.getTimeDelta(elapsedTime))));
                    }
                    else
                    {
                        // Add part of current clip between clipBegin and
                        // clipEnd
                        try
                        {
                            resStreams
                                    .add(curClip
                                            .getAudioData(
                                                    new Time()
                                                            .getZero()
                                                            .addTimeDelta(
                                                                    clipBegin
                                                                            .getTimeDelta(elapsedTime)),
                                                    new Time()
                                                            .getZero()
                                                            .addTimeDelta(
                                                                    clipEnd
                                                                            .getTimeDelta(elapsedTime))));
                        }
                        catch (InvalidDataFormatException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                    }
                }
                else
                    if (elapsedTime.isLessThan(clipEnd))
                    {
                        if (newElapsedTime.isLessThan(clipEnd))
                        {
                            // Add part of current clip between elapsedTime and
                            // newElapsedTime
                            // (ie. entire clip since elapsedTime and
                            // newElapsedTime is
                            // at
                            // the beginning and end of the clip respectively)
                            resStreams.add(curClip.getAudioData());
                        }
                        else
                        {
                            // Add part of current clip between elapsedTime and
                            // clipEnd
                            // (ie. before clipEnd since elapsedTime is at the
                            // beginning
                            // of the clip)
                            try
                            {
                                resStreams
                                        .add(curClip
                                                .getAudioData(
                                                        new Time().getZero(),
                                                        new Time()
                                                                .getZero()
                                                                .addTimeDelta(
                                                                        clipEnd
                                                                                .getTimeDelta(elapsedTime))));
                            }
                            catch (InvalidDataFormatException e)
                            {
                                // Should never happen
                                throw new RuntimeException("WTF ??!", e);
                            }
                        }
                    }
                    else
                    {
                        // The current clip and all remaining clips are beyond
                        // clipEnd
                        break;
                    }
            elapsedTime = newElapsedTime;
            i++;
        }
        if (resStreams.size() == 0)
        {
            return null; // TODO: new MemoryStream(0);
        }
        return new SequenceStream(resStreams);
    }

    @Override
    /*
     * @param duration can be null, in which case the newly created clip
     * duration will be used instead
     */
    public void appendAudioData(IStream pcmData, ITimeDelta duration)
            throws MethodParameterIsNullException
    {
        if (pcmData == null)
        {
            throw new MethodParameterIsNullException();
        }
        ITime insertPoint = new Time().getZero().addTimeDelta(
                getAudioDuration());
        WavClip newAppClip = createWavClipFromRawPCMStream(pcmData, duration);
        mWavClips.add(newAppClip);
        notifyListeners(new AudioDataInsertedEvent(this, insertPoint,
                (duration == null ? newAppClip.getMediaDuration() : duration)));
    }

    @Override
    public void insertAudioData(IStream pcmData, ITime insertPoint,
            ITimeDelta duration) throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        ITime insPt = insertPoint.copy();
        if (insPt.isLessThan(new Time().getZero()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        WavClip newInsClip = createWavClipFromRawPCMStream(pcmData, duration);
        ITime endTime = new Time().getZero().addTimeDelta(getAudioDuration());
        if (insertPoint.isGreaterThan(endTime))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (insertPoint.isEqualTo(endTime))
        {
            mWavClips.add(newInsClip);
            return;
        }
        ITime elapsedTime = new Time().getZero();
        int clipIndex = 0;
        while (clipIndex < mWavClips.size())
        {
            WavClip curClip = mWavClips.get(clipIndex);
            if (insPt.isEqualTo(elapsedTime))
            {
                // If the insert point at the beginning of the current clip,
                // insert the new clip and break
                mWavClips.add(clipIndex, newInsClip);
                break;
            }
            else
                if (insPt.isLessThan(elapsedTime.addTimeDelta(curClip
                        .getDuration())))
                {
                    // If the insert point is between the beginning and end of
                    // the
                    // current clip,
                    // Replace the current clip with three clips containing
                    // the audio in the current clip before the insert point,
                    // the audio to be inserted and the audio in the current
                    // clip
                    // after the insert point respectively
                    ITime insPtInCurClip = new Time().getZero().addTimeDelta(
                            insPt.getTimeDelta(elapsedTime));
                    IStream audioDataStream;
                    try
                    {
                        audioDataStream = curClip.getAudioData(new Time()
                                .getZero(), insPtInCurClip);
                    }
                    catch (InvalidDataFormatException e)
                    {
                        // Should never happen
                        throw new RuntimeException("WTF ??!", e);
                    }
                    WavClip curClipBeforeIns, curClipAfterIns;
                    try
                    {
                        curClipBeforeIns = createWavClipFromRawPCMStream(audioDataStream);
                    }
                    finally
                    {
                        try
                        {
                            audioDataStream.close();
                        }
                        catch (IOException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                    }
                    audioDataStream = curClip.getAudioData(insPtInCurClip);
                    try
                    {
                        curClipAfterIns = createWavClipFromRawPCMStream(audioDataStream);
                    }
                    finally
                    {
                        try
                        {
                            audioDataStream.close();
                        }
                        catch (IOException e)
                        {
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
    public ITimeDelta getAudioDuration()
    {
        ITimeDelta dur = new TimeDelta();
        for (WavClip clip : mWavClips)
        {
            try
            {
                dur.addTimeDelta(clip.getDuration());
            }
            catch (MethodParameterIsNullException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        return dur;
    }

    @Override
    public void removeAudioData(ITime clipBegin)
            throws TimeOffsetIsOutOfBoundsException,
            MethodParameterIsNullException
    {
        if (clipBegin == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (clipBegin
                .isGreaterThan(new Time().addTimeDelta(getAudioDuration()))
                || clipBegin.isLessThan(new Time().getZero()))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (clipBegin == new Time().getZero())
        {
            ITimeDelta prevDur = getAudioDuration();
            mWavClips.clear();
            notifyListeners(new AudioDataRemovedEvent(this, clipBegin, prevDur));
        }
        else
        {
            super.removeAudioData(clipBegin);
        }
    }

    @Override
    public void removeAudioData(ITime clipBegin, ITime clipEnd)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException
    {
        if (clipBegin == null || clipEnd == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (clipBegin.isLessThan(new Time().getZero())
                || clipBegin.isGreaterThan(clipEnd)
                || clipEnd.isGreaterThan(new Time().getZero().addTimeDelta(
                        getAudioDuration())))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        if (clipBegin.isEqualTo(new Time().getZero())
                && clipEnd.isEqualTo(new Time(getAudioDuration()
                        .getTimeDeltaAsMilliseconds())))
        {
            mWavClips.clear();
            notifyListeners(new AudioDataRemovedEvent(this, new Time()
                    .getZero(), getAudioDuration()));
            return;
        }
        ITime curBeginTime = new Time().getZero();
        List<WavClip> newClipList = new LinkedList<WavClip>();
        for (WavClip curClip : mWavClips)
        {
            ITime curEndTime = curBeginTime.addTimeDelta(curClip.getDuration());
            if ((!curEndTime.isGreaterThan(clipBegin))
                    || (!curBeginTime.isLessThan(clipEnd)))
            {
                // The current clip is before or beyond the range to remove -
                // so the clip is added unaltered to the new list of clips
                newClipList.add(curClip);
            }
            else
                if (curBeginTime.isLessThan(clipBegin)
                        && curEndTime.isGreaterThan(clipEnd))
                {
                    // Some of the current clip is before the range and some is
                    // after
                    ITimeDelta beforePartDur = curBeginTime
                            .getTimeDelta(clipBegin);
                    ITimeDelta beyondPartDur = curEndTime.getTimeDelta(clipEnd);
                    IStream beyondAS = curClip.getAudioData(curClip
                            .getClipEnd().subtractTimeDelta(beyondPartDur));
                    WavClip beyondPartClip;
                    try
                    {
                        beyondPartClip = createWavClipFromRawPCMStream(beyondAS);
                    }
                    finally
                    {
                        try
                        {
                            beyondAS.close();
                        }
                        catch (IOException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                    }
                    curClip.setClipEnd(curClip.getClipBegin().addTimeDelta(
                            beforePartDur));
                    newClipList.add(curClip);
                    newClipList.add(beyondPartClip);
                }
                else
                    if (curBeginTime.isLessThan(clipBegin)
                            && curEndTime.isGreaterThan(clipBegin))
                    {
                        // Some of the current clip is before the range to
                        // remove, none
                        // is beyond
                        ITimeDelta beforePartDur = curBeginTime
                                .getTimeDelta(clipBegin);
                        curClip.setClipEnd(curClip.getClipBegin().addTimeDelta(
                                beforePartDur));
                        newClipList.add(curClip);
                    }
                    else
                        if (curBeginTime.isLessThan(clipEnd)
                                && curEndTime.isGreaterThan(clipEnd))
                        {
                            // Some of the current clip is beyond the range to
                            // remove, none
                            // is before
                            ITimeDelta beyondPartDur = curEndTime
                                    .getTimeDelta(clipEnd);
                            curClip.setClipBegin(curClip.getClipEnd()
                                    .subtractTimeDelta(beyondPartDur));
                            newClipList.add(curClip);
                        }
                        else
                        {
                            // All of the current clip is within the range to
                            // remove,
                            // so this clip is not added to the new list of
                            // WavClips
                        }
            curBeginTime = curEndTime;
        }
        mWavClips = newClipList;
        notifyListeners(new AudioDataRemovedEvent(this, clipBegin, clipEnd
                .getTimeDelta(clipBegin)));
        /*
         * ITimeDelta dur = getAudioDuration(); new
         * TimeDelta(dur.getTimeDeltaAsMilliseconds() -
         * clipEnd.getTimeAsMilliseconds() - clipBegin.getTimeAsMilliseconds()))
         */
    }

    @Override
    protected void clear()
    {
        mWavClips.clear();
        // super.clear();
    }

    @Override
    protected void xukInChild(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        // To avoid event notification overhead, we bypass this:
        if (false && ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        boolean readItem = false;
        if (source.getNamespaceURI() == IXukAble.XUK_NS)
        {
            readItem = true;
            if (source.getLocalName() == "mWavClips")
            {
                xukInWavClips(source, ph);
            }
            else
                if (source.getLocalName() == "mPCMFormat")
                {
                    xukInPCMFormat(source, ph);
                }
                else
                {
                    readItem = false;
                }
        }
        if (!readItem)
        {
            super.xukInChild(source, ph);
        }
    }

    private void xukInPCMFormat(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    if (source.getLocalName() == "PCMFormatInfo"
                            && source.getNamespaceURI() == IXukAble.XUK_NS)
                    {
                        IPCMFormatInfo newInfo = new PCMFormatInfo();
                        newInfo.xukIn(source, ph);
                        try
                        {
                            setPCMFormat(newInfo);
                        }
                        catch (InvalidDataFormatException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                    }
                    else
                    {
                        super.xukInChild(source, ph);
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    private void xukInWavClips(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (!source.isEmptyElement())
        {
            while (source.read())
            {
                if (source.getNodeType() == IXmlDataReader.ELEMENT)
                {
                    if (source.getLocalName() == "WavClip"
                            && source.getNamespaceURI() == IXukAble.XUK_NS)
                    {
                        xukInWavClip(source, ph);
                    }
                    else
                    {
                        try
                        {
                            super.xukInChild(source, ph);
                        }
                        catch (ProgressCancelledException e)
                        {
                            // Should never happen
                            throw new RuntimeException("WTF ??!", e);
                        }
                    }
                }
                else
                    if (source.getNodeType() == IXmlDataReader.END_ELEMENT)
                    {
                        break;
                    }
                if (source.isEOF())
                    throw new XukDeserializationFailedException();
            }
        }
    }

    private void xukInWavClip(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException
    {
        if (source == null)
        {
            throw new MethodParameterIsNullException();
        }
        String clipBeginAttr = source.getAttribute("clipBegin");
        ITime cb = new Time().getZero();
        if (clipBeginAttr != null)
        {
            try
            {
                cb = new Time(clipBeginAttr);
            }
            catch (Exception e)
            {
                throw new XukDeserializationFailedException();
            }
        }
        String clipEndAttr = source.getAttribute("clipEnd");
        ITime ce = null;
        if (clipEndAttr != null)
        {
            try
            {
                ce = new Time(clipEndAttr);
            }
            catch (Exception e)
            {
                throw new XukDeserializationFailedException();
            }
        }
        String dataProviderUid = source.getAttribute("dataProvider");
        if (dataProviderUid == null)
        {
            throw new XukDeserializationFailedException();
        }
        IDataProvider prov;
        try
        {
            prov = getPresentation().getDataProviderManager().getDataProvider(
                    dataProviderUid);
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotManagerOfException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            mWavClips.add(new WavClip(prov, cb, ce));
        }
        catch (TimeOffsetIsOutOfBoundsException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        try
        {
            super.xukInChild(source, ph);
        }
        catch (ProgressCancelledException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    @Override
    protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws MethodParameterIsNullException,
            XukSerializationFailedException, ProgressCancelledException
    {
        if (destination == null || baseUri == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (ph != null && ph.notifyProgress())
        {
            throw new ProgressCancelledException();
        }
        // super.xukOutChildren(destination, baseUri, ph);
        destination.writeStartElement("mPCMFormat", IXukAble.XUK_NS);
        getPCMFormat().xukOut(destination, baseUri, ph);
        destination.writeEndElement();
        destination.writeStartElement("mWavClips", IXukAble.XUK_NS);
        for (WavClip clip : mWavClips)
        {
            destination.writeStartElement("WavClip", IXukAble.XUK_NS);
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
    public void mergeWith(IAudioMediaData other)
            throws MethodParameterIsNullException, InvalidDataFormatException
    {
        if (other == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (other instanceof WavAudioMediaData)
        {
            if (!getPCMFormat().isCompatibleWith(other.getPCMFormat()))
            {
                throw new InvalidDataFormatException();
            }
            ITime thisInsertPoint = new Time().getZero().addTimeDelta(
                    getAudioDuration());
            WavAudioMediaData otherWav = (WavAudioMediaData) other;
            mWavClips.addAll(otherWav.mWavClips);
            ITimeDelta dur = otherWav.getAudioDuration();
            notifyListeners(new AudioDataInsertedEvent(this, thisInsertPoint,
                    dur));
            try
            {
                otherWav.removeAudioData(new Time().getZero());
            }
            catch (TimeOffsetIsOutOfBoundsException e)
            {
                // Should never happen
                throw new RuntimeException("WTF ??!", e);
            }
        }
        else
        {
            super.mergeWith(other);
        }
    }

    @Override
    public IAudioMediaData split(ITime splitPoint)
            throws MethodParameterIsNullException,
            TimeOffsetIsOutOfBoundsException, FactoryCannotCreateTypeException
    {
        if (splitPoint == null)
        {
            throw new MethodParameterIsNullException();
        }
        if (splitPoint.isNegativeTimeOffset())
        {
            throw new TimeOffsetIsNegativeException();
        }
        if (splitPoint.isGreaterThan(new Time().getZero().addTimeDelta(
                getAudioDuration())))
        {
            throw new TimeOffsetIsOutOfBoundsException();
        }
        WavAudioMediaData oWAMD;
        try
        {
            oWAMD = getPresentation().getMediaDataFactory().create(
                    getXukLocalName(), getXukNamespaceURI());
        }
        catch (MethodParameterIsEmptyStringException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        if (oWAMD == null)
        {
            throw new FactoryCannotCreateTypeException();
        }
        try
        {
            oWAMD.setPCMFormat(getPCMFormat());
        }
        catch (InvalidDataFormatException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        ITimeDelta dur = new Time().getZero().addTimeDelta(getAudioDuration())
                .getTimeDelta(splitPoint);
        ITime elapsed = new Time().getZero();
        List<WavClip> clips = new LinkedList<WavClip>(mWavClips);
        mWavClips.clear();
        oWAMD.mWavClips.clear();
        for (int i = 0; i < clips.size(); i++)
        {
            WavClip curClip = clips.get(i);
            ITime endCurClip = elapsed.addTimeDelta(curClip.getDuration());
            if (splitPoint.isLessThanOrEqualTo(elapsed))
            {
                oWAMD.mWavClips.add(curClip);
            }
            else
                if (splitPoint.isLessThan(endCurClip))
                {
                    WavClip secondPartClip = new WavClip(curClip
                            .getDataProvider(), curClip.getClipBegin(), curClip
                            .isClipEndTiedToEOM() ? null : curClip.getClipEnd());
                    curClip.setClipEnd(curClip.getClipBegin().addTime(
                            splitPoint.subtractTime(elapsed)));
                    secondPartClip.setClipBegin(curClip.getClipEnd());
                    mWavClips.add(curClip);
                    oWAMD.mWavClips.add(secondPartClip);
                }
                else
                {
                    mWavClips.add(curClip);
                }
            elapsed = elapsed.addTimeDelta(curClip.getDuration());
        }
        notifyListeners(new AudioDataRemovedEvent(this, splitPoint, dur));
        oWAMD.notifyListeners(new AudioDataInsertedEvent(oWAMD, new Time()
                .getZero(), dur));
        return oWAMD;
    }

    @SuppressWarnings("unused")
    @Override
    protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
            throws MethodParameterIsNullException,
            XukDeserializationFailedException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }

    @SuppressWarnings("unused")
    @Override
    protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
            IProgressHandler ph) throws XukSerializationFailedException,
            MethodParameterIsNullException, ProgressCancelledException
    {
        /**
         * Does nothing.
         */
    }
}
