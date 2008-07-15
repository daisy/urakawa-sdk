using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.media.data;
using urakawa.media.timing;
using urakawa.media.data.utilities;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media.data.audio.codec
{
    /// <summary>
    /// Implementation of <see cref="AudioMediaData"/> that supports sequences of RIFF WAVE PCM audio data clips
    /// </summary>
    public class WavAudioMediaData : AudioMediaData
    {
        /// <summary>
        /// Represents a RIFF WAVE PCM audio data clip
        /// </summary>
        protected class WavClip : Clip, IValueEquatable<WavClip>
        {
            /// <summary>
            /// Constructor setting the <see cref="IDataProvider"/>, 
            /// clip begin and clip end will in this case be initialized to <c>null</c>,
            /// which means beginning/end if the RIFF WAVE PCM data
            /// </summary>
            /// <param name="clipDataProvider">The <see cref="IDataProvider"/></param>
            public WavClip(IDataProvider clipDataProvider) : this(clipDataProvider, new Time(), null)
            {
            }

            /// <summary>
            /// Constructor setting the <see cref="IDataProvider"/> and clip begin/end values
            /// </summary>
            /// <param name="clipDataProvider">The <see cref="IDataProvider"/> - can not be <c>null</c></param>
            /// <param name="clipBegin">The clip begin <see cref="Time"/> - can not be <c>null</c></param>
            /// <param name="clipEnd">
            /// The clip end <see cref="Time"/>
            /// - a <c>null</c> value ties clip end to the end of the underlying wave audio</param>
            public WavClip(IDataProvider clipDataProvider, Time clipBegin, Time clipEnd)
            {
                if (clipDataProvider == null)
                {
                    throw new exception.MethodParameterIsNullException("The data provider of a WavClip can not be null");
                }
                mDataProvider = clipDataProvider;
                ClipBegin = clipBegin;
                ClipEnd = clipEnd;
            }

            /// <summary>
            /// Gets the duration of the underlying RIFF wav file 
            /// </summary>
            /// <returns>The duration</returns>
            public override TimeDelta MediaDuration
            {
                get
                {
                    Stream raw = DataProvider.GetInputStream();
                    PCMDataInfo pcmInfo;
                    try
                    {
                        pcmInfo = PCMDataInfo.ParseRiffWaveHeader(raw);
                    }
                    finally
                    {
                        raw.Close();
                    }
                    return new TimeDelta(pcmInfo.Duration);
                }
            }


            /// <summary>
            /// Creates a copy of the wav clip
            /// </summary>
            /// <returns>The copy</returns>
            public WavClip Copy()
            {
                Time clipEnd = null;
                if (!IsClipEndTiedToEOM) clipEnd = ClipEnd.Copy();
                return new WavClip(DataProvider.Copy(), ClipBegin.Copy(), clipEnd);
            }

            /// <summary>
            /// Exports the clip to a destination <see cref="Presentation"/>
            /// </summary>
            /// <param name="destPres">The destination <see cref="Presentation"/></param>
            /// <returns>The exported clip</returns>
            public WavClip Export(Presentation destPres)
            {
                Time clipEnd = null;
                if (!IsClipEndTiedToEOM) clipEnd = ClipEnd.Copy();
                return new WavClip(DataProvider.Export(destPres), ClipBegin.Copy(), clipEnd);
            }

            private IDataProvider mDataProvider;

            /// <summary>
            /// Gets the <see cref="IDataProvider"/> storing the RIFF WAVE PCM audio data of <c>this</c>
            /// </summary>
            /// <returns>The <see cref="IDataProvider"/></returns>
            public IDataProvider DataProvider
            {
                get { return mDataProvider; }
            }

            /// <summary>
            /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
            /// </summary>
            /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
            public Stream GetAudioData()
            {
                return GetAudioData(Time.Zero);
            }

            /// <summary>
            /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
            /// after a given sub-clip begin time
            /// </summary>
            /// <param name="subClipBegin"></param>
            /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
            /// <seealso cref="GetAudioData(Time,Time)"/>
            public Stream GetAudioData(Time subClipBegin)
            {
                return GetAudioData(subClipBegin, Time.Zero.AddTimeDelta(Duration));
            }

            /// <summary>
            /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
            /// between given sub-clip begin and end times
            /// </summary>
            /// <param name="subClipBegin">The beginning of the sub-clip</param>
            /// <param name="subClipEnd">The end of the sub-clip</param>
            /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
            /// <remarks>
            /// <para>Sub-clip times must be in the interval <c>[0;this.getAudioDuration()]</c>.</para>
            /// <para>
            /// The sub-clip is
            /// relative to clip begin of the WavClip, that if <c>this.getClipBegin()</c>
            /// returns <c>00:00:10</c>, <c>this.getClipEnd()</c> returns <c>00:00:50</c>, 
            /// <c>x</c> and <c>y</c> is <c>00:00:05</c> and <c>00:00:30</c> respectively, 
            /// then <c>this.GetAudioData(x, y)</c> will get the audio in the underlying wave audio between
            /// <c>00:00:15</c> and <c>00:00:40</c>
            /// </para>
            /// </remarks>
            public Stream GetAudioData(Time subClipBegin, Time subClipEnd)
            {
                if (subClipBegin == null)
                {
                    throw new exception.MethodParameterIsNullException("subClipBegin must not be null");
                }
                if (subClipEnd == null)
                {
                    throw new exception.MethodParameterIsNullException("subClipEnd must not be null");
                }
                if (
                    subClipBegin.IsLessThan(Time.Zero)
                    || subClipEnd.IsLessThan(subClipBegin)
                    || subClipBegin.AddTimeDelta(Duration).IsLessThan(subClipEnd))
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "The interval [subClipBegin;subClipEnd] must be non-empty and contained in [0;GetDuration()]");
                }
                Stream raw = DataProvider.GetInputStream();
                PCMDataInfo pcmInfo = PCMDataInfo.ParseRiffWaveHeader(raw);
                Time rawEndTime = Time.Zero.AddTimeDelta(pcmInfo.Duration);
                if (
                    ClipBegin.IsLessThan(Time.Zero)
                    || ClipBegin.IsGreaterThan(ClipEnd)
                    || ClipEnd.IsGreaterThan(rawEndTime))
                {
                    throw new exception.InvalidDataFormatException(String.Format(
                                                                       "WavClip [{0};{1}] is empty or not within the underlying wave data stream ([0;{2}])",
                                                                       ClipBegin.ToString(), ClipEnd.ToString(),
                                                                       rawEndTime.ToString()));
                }
                Time rawClipBegin = ClipBegin.AddTime(subClipBegin);
                Time rawClipEnd = ClipBegin.AddTime(subClipEnd);
                long offset;
                long beginPos = raw.Position + (long) ((rawClipBegin.TimeAsMillisecondFloat*pcmInfo.ByteRate)/1000);
                offset = (beginPos - raw.Position)%pcmInfo.BlockAlign;
                beginPos -= offset;
                long endPos = raw.Position + (long) ((rawClipEnd.TimeAsMillisecondFloat*pcmInfo.ByteRate)/1000);
                offset = (endPos - raw.Position)%pcmInfo.BlockAlign;
                endPos -= offset;
                utilities.SubStream res = new utilities.SubStream(
                    raw,
                    beginPos,
                    endPos - beginPos);
                return res;
            }

            #region IValueEquatable<WavClip> Members

            /// <summary>
            /// Determines of <c>this</c> has the same value as a given other instance
            /// </summary>
            /// <param name="other">The other instance</param>
            /// <returns>A <see cref="bool"/> indicating the result</returns>
            public bool ValueEquals(WavClip other)
            {
                if (other == null) return false;
                if (!ClipBegin.IsEqualTo(other.ClipBegin)) return false;
                if (IsClipEndTiedToEOM != other.IsClipEndTiedToEOM) return false;
                if (!ClipEnd.IsEqualTo(other.ClipEnd)) return false;
                if (!DataProvider.ValueEquals(other.DataProvider)) return false;
                return true;
            }

            #endregion
        }

        /// <summary>
        /// Stores the <see cref="WavClip"/>s of <c>this</c>
        /// </summary>
        private List<WavClip> mWavClips = new List<WavClip>();

        /// <summary>
        /// Constructor associating the newly constructed <see cref="WavAudioMediaData"/> 
        /// with a given <see cref="MediaDataManager"/> 
        /// </summary>
        protected internal WavAudioMediaData()
        {
        }

        /// <summary>
        /// Determines if a PCM Format change is ok
        /// </summary>
        /// <param name="newFormat">The new PCM Format value - assumed not to be <c>null</c></param>
        /// <param name="failReason">The <see cref="string"/> to which a failure reason must be written in case the change is not ok</param>
        /// <returns>A <see cref="bool"/> indicating if the change is ok</returns>
        protected override bool IsPCMFormatChangeOk(PCMFormatInfo newFormat, out string failReason)
        {
            if (!base.IsPCMFormatChangeOk(newFormat, out failReason)) return false;
            if (mWavClips.Count > 0)
            {
                if (!PCMFormat.ValueEquals(newFormat))
                {
                    failReason =
                        "Cannot change the PCMFormat of the WavAudioMediaData after audio dat has been added to it";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a <see cref="WavClip"/> from a RAW PCM audio <see cref="Stream"/>, 
        /// reading all data from the current position in the stream till it's end
        /// </summary>
        /// <param name="pcmData">The raw PCM stream</param>
        /// <returns>The <see cref="WavClip"/></returns>
        protected WavClip CreateWavClipFromRawPCMStream(Stream pcmData)
        {
            return CreateWavClipFromRawPCMStream(pcmData, null);
        }

        /// <summary>
        /// Gets a <see cref="WavClip"/> from a RAW PCM audio <see cref="Stream"/> of a given duration
        /// </summary>
        /// <param name="pcmData">The raw PCM data stream</param>
        /// <param name="duration">The duration</param>
        /// <returns>The <see cref="WavClip"/></returns>
        protected WavClip CreateWavClipFromRawPCMStream(Stream pcmData, TimeDelta duration)
        {
            IDataProvider newSingleDataProvider = MediaDataManager.DataProviderFactory.CreateDataProvider(
                DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            PCMDataInfo pcmInfo = new PCMDataInfo(PCMFormat);
            if (duration == null)
            {
                pcmInfo.DataLength = (uint) (pcmData.Length - pcmData.Position);
            }
            else
            {
                pcmInfo.DataLength = pcmInfo.GetDataLength(duration);
            }
            Stream nsdps = newSingleDataProvider.GetOutputStream();
            try
            {
                pcmInfo.WriteRiffWaveHeader(nsdps);
            }
            finally
            {
                nsdps.Close();
            }
            DataProviderManager.AppendDataToProvider(pcmData, (int) pcmInfo.DataLength, newSingleDataProvider);
            WavClip newSingleWavClip = new WavClip(newSingleDataProvider);
            return newSingleWavClip;
        }

        /// <summary>
        /// Forces the PCM data to be stored in a single <see cref="IDataProvider"/>.
        /// </summary>
        /// <remarks>
        /// This effectively copies the data, 
        /// since the <see cref="IDataProvider"/>(s) previously used to store the PCM data are left untouched
        /// </remarks>
        public void ForceSingleDataProvider()
        {
            Stream audioData = GetAudioData();
            WavClip newSingleClip;
            try
            {
                newSingleClip = CreateWavClipFromRawPCMStream(audioData);
            }
            finally
            {
                audioData.Close();
            }
            mWavClips.Clear();
            mWavClips.Add(newSingleClip);
        }

        #region MediaData

        /// <summary>
        /// Creates a copy of <c>this</c>, including copies of all <see cref="IDataProvider"/>s used by <c>this</c>
        /// </summary>
        /// <returns>The copy</returns>
        protected override AudioMediaData AudioMediaDataCopy()
        {
            return Copy();
        }

        /// <summary>
        /// Creates a copy of <c>this</c>, including copies of all <see cref="IDataProvider"/>s used by <c>this</c>
        /// </summary>
        /// <returns>The copy</returns>
        public new WavAudioMediaData Copy()
        {
            MediaData oCopy = GetMediaDataFactory().CreateMediaData(XukLocalName, XukNamespaceUri);
            if (!(oCopy is WavAudioMediaData))
            {
                throw new exception.FactoryCannotCreateTypeException(
                    "The MediaDataFactory can not create a WavAudioMediaData");
            }
            WavAudioMediaData copy = (WavAudioMediaData) oCopy;
            foreach (WavClip clip in mWavClips)
            {
                copy.mWavClips.Add(clip.Copy());
            }
            return copy;
        }

        /// <summary>
        /// Exports <c>this</c> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentation</param>
        /// <returns>The exported wav audio media data</returns>
        protected override MediaData ProtectedExport(Presentation destPres)
        {
            return Export(destPres);
        }

        /// <summary>
        /// Exports <c>this</c> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentation</param>
        /// <returns>The exported wav audio media data</returns>
        public new WavAudioMediaData Export(Presentation destPres)
        {
            WavAudioMediaData expWAMD = destPres.MediaDataFactory.CreateMediaData(
                                            XukLocalName, XukNamespaceUri) as WavAudioMediaData;
            if (expWAMD == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaDataFactory of the destination Presentation cannot create a WavAudioMediaData matching QName {0}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            expWAMD.PCMFormat = PCMFormat;
            foreach (WavClip clip in mWavClips)
            {
                expWAMD.mWavClips.Add(clip.Export(destPres));
            }
            return expWAMD;
        }

        /// <summary>
        /// Deletes the <see cref="MediaData"/>, detaching it from it's manager 
        /// and clearing the list of clips making up the wave audio media
        /// </summary>
        public override void Delete()
        {
            mWavClips.Clear();
            base.Delete();
        }

        /// <summary>
        /// Gets a <see cref="List{IDataProvider}"/> of the <see cref="IDataProvider"/>s
        /// used to store the Wav audio data
        /// </summary>
        /// <returns>The <see cref="List{IDataProvider}"/></returns>
        public override List<IDataProvider> ListOfUsedDataProviders
        {
            get
            {
                List<IDataProvider> usedDP = new List<IDataProvider>(mWavClips.Count);
                foreach (WavClip clip in mWavClips)
                {
                    if (!usedDP.Contains(clip.DataProvider)) usedDP.Add(clip.DataProvider);
                }
                return usedDP;
            }
        }

        #endregion

        /// <summary>
        /// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="Time"/>s
        /// as raw PCM data
        /// </summary>
        /// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
        /// <param name="clipEnd">The given clip end <see cref="Time"/></param>
        /// <returns>The <see cref="Stream"/></returns>
        public override Stream GetAudioData(Time clipBegin, Time clipEnd)
        {
            if (clipBegin.IsNegativeTimeOffset)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The clip begin value can not be a negative time");
            }
            if (clipEnd.IsLessThan(clipBegin))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The clip end can not be before clip begin");
            }
            if (clipEnd.IsGreaterThan(Time.Zero.AddTimeDelta(AudioDuration)))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The clip end can not beyond the end of the audio content");
            }
            Time timeBeforeStartIndexClip = new Time();
            Time timeBeforeEndIndexClip = new Time();
            Time elapsedTime = new Time();
            int i = 0;
            List<Stream> resStreams = new List<Stream>();
            while (i < mWavClips.Count)
            {
                WavClip curClip = mWavClips[i];
                TimeDelta currentClipDuration = curClip.Duration;
                Time newElapsedTime = elapsedTime.AddTimeDelta(currentClipDuration);
                if (newElapsedTime.IsLessThan(clipBegin))
                {
                    //Do nothing - the current clip and the [clipBegin;clipEnd] are disjunkt
                }
                else if (elapsedTime.IsLessThan(clipBegin))
                {
                    if (newElapsedTime.IsLessThan(clipEnd))
                    {
                        //Add part of current clip between clipBegin and newElapsedTime 
                        //(ie. after clipBegin, since newElapsedTime is at the end of the clip)
                        resStreams.Add(curClip.GetAudioData(
                                           Time.Zero.AddTimeDelta(clipBegin.GetTimeDelta(elapsedTime))));
                    }
                    else
                    {
                        //Add part of current clip between clipBegin and clipEnd
                        resStreams.Add(curClip.GetAudioData(
                                           Time.Zero.AddTimeDelta(clipBegin.GetTimeDelta(elapsedTime)),
                                           Time.Zero.AddTimeDelta(clipEnd.GetTimeDelta(elapsedTime))));
                    }
                }
                else if (elapsedTime.IsLessThan(clipEnd))
                {
                    if (newElapsedTime.IsLessThan(clipEnd))
                    {
                        //Add part of current clip between elapsedTime and newElapsedTime
                        //(ie. entire clip since elapsedTime and newElapsedTime is at
                        //the beginning and end of the clip respectively)
                        resStreams.Add(curClip.GetAudioData());
                    }
                    else
                    {
                        //Add part of current clip between elapsedTime and clipEnd
                        //(ie. before clipEnd since elapsedTime is at the beginning of the clip)
                        resStreams.Add(curClip.GetAudioData(
                                           Time.Zero,
                                           Time.Zero.AddTimeDelta(clipEnd.GetTimeDelta(elapsedTime))));
                    }
                }
                else
                {
                    //The current clip and all remaining clips are beyond clipEnd
                    break;
                }
                elapsedTime = newElapsedTime;
                i++;
            }
            if (resStreams.Count == 0)
            {
                return new MemoryStream(0);
            }
            return new SequenceStream(resStreams);
        }

        /// <summary>
        /// Appends audio of a given duration from a given source PCM data <see cref="Stream"/> to the wav audio media data
        /// </summary>
        /// <param name="pcmData">The source PCM data stream</param>
        /// <param name="duration">The duration of the audio to append 
        /// - if <c>null</c>, all audio data from <paramref name="pcmData"/> is added</param>
        public override void AppendAudioData(Stream pcmData, TimeDelta duration)
        {
            Time insertPoint = Time.Zero.AddTimeDelta(AudioDuration);
            WavClip newAppClip = CreateWavClipFromRawPCMStream(pcmData, duration);
            mWavClips.Add(newAppClip);
            if (duration == null) duration = newAppClip.MediaDuration;
            NotifyAudioDataInserted(this, insertPoint, duration);
        }


        /// <summary>
        /// Inserts audio of a given duration from a given source PCM data <see cref="Stream"/> to the wav audio media data
        /// at a given point
        /// </summary>
        /// <param name="pcmData">The source PCM data stream</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration of the aduio to append</param>
        public override void InsertAudioData(Stream pcmData, Time insertPoint, TimeDelta duration)
        {
            Time insPt = insertPoint.Copy();
            if (insPt.IsLessThan(Time.Zero))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given insert point is negative");
            }
            WavClip newInsClip = CreateWavClipFromRawPCMStream(pcmData, duration);
            Time endTime = Time.Zero.AddTimeDelta(AudioDuration);
            if (insertPoint.IsGreaterThan(endTime))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given insert point is beyond the end of the WavAudioMediaData");
            }
            if (insertPoint.IsEqualTo(endTime))
            {
                mWavClips.Add(newInsClip);
                return;
            }
            Time elapsedTime = Time.Zero;
            int clipIndex = 0;
            while (clipIndex < mWavClips.Count)
            {
                WavClip curClip = mWavClips[clipIndex];
                if (insPt.IsEqualTo(elapsedTime))
                {
                    //If the insert point at the beginning of the current clip, insert the new clip and break
                    mWavClips.Insert(clipIndex, newInsClip);
                    break;
                }
                else if (insPt.IsLessThan(elapsedTime.AddTimeDelta(curClip.Duration)))
                {
                    //If the insert point is between the beginning and end of the current clip, 
                    //Replace the current clip with three clips containing 
                    //the audio in the current clip before the insert point,
                    //the audio to be inserted and the audio in the current clip after the insert point respectively
                    Time insPtInCurClip = Time.Zero.AddTimeDelta(insPt.GetTimeDelta(elapsedTime));
                    Stream audioDataStream;
                    audioDataStream = curClip.GetAudioData(Time.Zero, insPtInCurClip);
                    WavClip curClipBeforeIns, curClipAfterIns;
                    try
                    {
                        curClipBeforeIns = CreateWavClipFromRawPCMStream(audioDataStream);
                    }
                    finally
                    {
                        audioDataStream.Close();
                    }
                    audioDataStream = curClip.GetAudioData(insPtInCurClip);
                    try
                    {
                        curClipAfterIns = CreateWavClipFromRawPCMStream(audioDataStream);
                    }
                    finally
                    {
                        audioDataStream.Close();
                    }
                    mWavClips.RemoveAt(clipIndex);
                    mWavClips.InsertRange(clipIndex, new WavClip[] {curClipBeforeIns, newInsClip, curClipAfterIns});
                    break;
                }
                elapsedTime = elapsedTime.AddTimeDelta(curClip.Duration);
                clipIndex++;
            }
            NotifyAudioDataInserted(this, insertPoint, duration);
        }

        /// <summary>
        /// Gets the intrinsic duration of the audio data
        /// </summary>
        /// <returns>The duration as an <see cref="TimeDelta"/></returns>
        public override TimeDelta AudioDuration
        {
            get
            {
                TimeDelta dur = new TimeDelta();
                foreach (WavClip clip in mWavClips)
                {
                    dur.AddTimeDelta(clip.Duration);
                }
                return dur;
            }
        }

        /// <summary>
        /// Removes all audio after a given clip begin point
        /// </summary>
        /// <param name="clipBegin">The given clip begin point</param>
        public override void RemoveAudioData(Time clipBegin)
        {
            if (clipBegin == Time.Zero)
            {
                TimeDelta prevDur = AudioDuration;
                mWavClips.Clear();
                NotifyAudioDataRemoved(this, clipBegin, prevDur);
            }
            else
            {
                base.RemoveAudioData(clipBegin);
            }
        }

        /// <summary>
        /// Removes the audio between given clip begin and end points
        /// </summary>
        /// <param name="clipBegin">The given clip begin point</param>
        /// <param name="clipEnd">The given clip end point</param>
        public override void RemoveAudioData(Time clipBegin, Time clipEnd)
        {
            if (clipBegin == null || clipEnd == null)
            {
                throw new exception.MethodParameterIsNullException("Clip begin and clip end can not be null");
            }
            if (
                clipBegin.IsLessThan(Time.Zero)
                || clipBegin.IsGreaterThan(clipEnd)
                || clipEnd.IsGreaterThan(Time.Zero.AddTimeDelta(AudioDuration)))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format("The given clip times are not valid, must be between 00:00:00.000 and {0}",
                                  AudioDuration));
            }
            Time curBeginTime = Time.Zero;

            List<WavClip> newClipList = new List<WavClip>();
            foreach (WavClip curClip in mWavClips)
            {
                Time curEndTime = curBeginTime.AddTimeDelta(curClip.Duration);
                if ((!curEndTime.IsGreaterThan(clipBegin)) || (!curBeginTime.IsLessThan(clipEnd)))
                {
                    //The current clip is before or beyond the range to remove - 
                    //so the clip is added unaltered to the new list of clips
                    newClipList.Add(curClip);
                }
                else if (curBeginTime.IsLessThan(clipBegin) && curEndTime.IsGreaterThan(clipEnd))
                {
                    //Some of the current clip is before the range and some is after
                    TimeDelta beforePartDur = curBeginTime.GetTimeDelta(clipBegin);
                    TimeDelta beyondPartDur = curEndTime.GetTimeDelta(clipEnd);
                    Stream beyondAS = curClip.GetAudioData(curClip.ClipEnd.SubtractTimeDelta(beyondPartDur));
                    WavClip beyondPartClip;
                    try
                    {
                        beyondPartClip = CreateWavClipFromRawPCMStream(beyondAS);
                    }
                    finally
                    {
                        beyondAS.Close();
                    }

                    curClip.ClipEnd = curClip.ClipBegin.AddTimeDelta(beforePartDur);
                    newClipList.Add(curClip);
                    newClipList.Add(beyondPartClip);
                }
                else if (curBeginTime.IsLessThan(clipBegin) && curEndTime.IsGreaterThan(clipBegin))
                {
                    //Some of the current clip is before the range to remove, none is beyond
                    TimeDelta beforePartDur = curBeginTime.GetTimeDelta(clipBegin);
                    curClip.ClipEnd = curClip.ClipBegin.AddTimeDelta(beforePartDur);
                    newClipList.Add(curClip);
                }
                else if (curBeginTime.IsLessThan(clipEnd) && curEndTime.IsGreaterThan(clipEnd))
                {
                    //Some of the current clip is beyond the range to remove, none is before
                    TimeDelta beyondPartDur = curEndTime.GetTimeDelta(clipEnd);
                    curClip.ClipBegin = curClip.ClipEnd.SubtractTimeDelta(beyondPartDur);
                    newClipList.Add(curClip);
                }
                else
                {
                    //All of the current clip is within the range to remove, 
                    //so this clip is not added to the new list of WavClips
                }
                curBeginTime = curEndTime;
            }
            mWavClips = newClipList;
            NotifyAudioDataRemoved(this, clipBegin, clipEnd.GetTimeDelta(clipBegin));
        }

        #region IXukAble

        /// <summary>
        /// Clears the <see cref="WavAudioMediaData"/>, removing all <see cref="WavClip"/>s
        /// </summary>
        protected override void Clear()
        {
            mWavClips.Clear();
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a WavAudioMediaData xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                switch (source.LocalName)
                {
                    case "mWavClips":
                        XukInWavClips(source);
                        break;
                    case "mPCMFormat":
                        XukInPCMFormat(source, handler);
                        break;
                    default:
                        readItem = false;
                        break;
                }
            }
            if (!readItem) base.XukInChild(source, handler);
        }

        private void XukInPCMFormat(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == "PCMFormatInfo" && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            PCMFormatInfo newInfo = new PCMFormatInfo();
                            newInfo.XukIn(source, handler);
                            PCMFormat = newInfo;
                        }
                        else if (!source.IsEmptyElement)
                        {
                            source.ReadSubtree().Close();
                        }
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        private void XukInWavClips(XmlReader source)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == "WavClip" && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            XukInWavClip(source);
                        }
                        else if (!source.IsEmptyElement)
                        {
                            source.ReadSubtree().Close();
                        }
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        private void XukInWavClip(XmlReader source)
        {
            string clipBeginAttr = source.GetAttribute("clipBegin");
            Time cb = Time.Zero;
            if (clipBeginAttr != null)
            {
                try
                {
                    cb = new Time(clipBeginAttr);
                }
                catch (Exception e)
                {
                    throw new exception.XukException(
                        String.Format("Invalid clip begin time {0}", clipBeginAttr),
                        e);
                }
            }
            string clipEndAttr = source.GetAttribute("clipEnd");
            Time ce = null;
            if (clipEndAttr != null)
            {
                try
                {
                    ce = new Time(clipEndAttr);
                }
                catch (Exception e)
                {
                    throw new exception.XukException(
                        String.Format("Invalid clip end time {0}", clipEndAttr),
                        e);
                }
            }
            string dataProviderUid = source.GetAttribute("dataProvider");
            if (dataProviderUid == null)
            {
                throw new exception.XukException("dataProvider attribute is missing from WavClip element");
            }
            IDataProvider prov;
            prov = MediaDataManager.Presentation.DataProviderManager.GetDataProvider(dataProviderUid);
            mWavClips.Add(new WavClip(prov, cb, ce));
            if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }

        /// <summary>
        /// Write the child elements of a WavAudioMediaData element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);
            destination.WriteStartElement("mPCMFormat");
            PCMFormat.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();
            destination.WriteStartElement("mWavClips", XukAble.XUK_NS);
            foreach (WavClip clip in mWavClips)
            {
                destination.WriteStartElement("WavClip", XukAble.XUK_NS);
                destination.WriteAttributeString("dataProvider", clip.DataProvider.Uid);
                destination.WriteAttributeString("clipBegin", clip.ClipBegin.ToString());
                if (!clip.IsClipEndTiedToEOM) destination.WriteAttributeString("clipEnd", clip.ClipEnd.ToString());
                destination.WriteEndElement();
            }
            destination.WriteEndElement();
        }

        #endregion

        /// <summary>
        /// Merges <c>this</c> with a given other <see cref="AudioMediaData"/>,
        /// appending the audio data of the other <see cref="AudioMediaData"/> to <c>this</c>,
        /// leaving the other <see cref="AudioMediaData"/> without audio data
        /// </summary>
        /// <param name="other">The given other AudioMediaData</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="other"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the PCM format of <c>this</c> is not compatible with that of <paramref name="other"/>
        /// </exception>
        public override void MergeWith(AudioMediaData other)
        {
            if (other is WavAudioMediaData)
            {
                if (!PCMFormat.IsCompatibleWith(other.PCMFormat))
                {
                    throw new exception.InvalidDataFormatException(
                        "Can not merge this with a WavAudioMediaData with incompatible audio data");
                }
                Time thisInsertPoint = Time.Zero.AddTimeDelta(AudioDuration);
                WavAudioMediaData otherWav = (WavAudioMediaData) other;
                mWavClips.AddRange(otherWav.mWavClips);
                TimeDelta dur = otherWav.AudioDuration;
                NotifyAudioDataInserted(this, thisInsertPoint, dur);
                otherWav.RemoveAudioData(Time.Zero);
            }
            else
            {
                base.MergeWith(other);
            }
        }

        /// <summary>
        /// Splits the audio media data at a given split point in time,
        /// <c>this</c> retaining the audio before the split point,
        /// creating a new <see cref="WavAudioMediaData"/> containing the audio after the split point
        /// </summary>
        /// <param name="splitPoint">The given split point</param>
        /// <returns>A audio media data containing the audio after the split point</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the given split point is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given split point is negative or is beyond the duration of <c>this</c>
        /// </exception>
        public override AudioMediaData Split(Time splitPoint)
        {
            if (splitPoint == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "The split point can not be null");
            }
            if (splitPoint.IsNegativeTimeOffset)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The split point can not be negative");
            }
            if (splitPoint.IsGreaterThan(Time.Zero.AddTimeDelta(AudioDuration)))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Split point {0} is beyond the WavAudioMediaData end - audio length is {1}",
                                                                              splitPoint.ToString(),
                                                                              AudioDuration.ToString()));
            }
            WavAudioMediaData oWAMD =
                GetMediaDataFactory().CreateMediaData(XukLocalName, XukNamespaceUri) as WavAudioMediaData;
            if (oWAMD == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "Thrown if the MediaDataFactory can not create a WacAudioMediaData matching Xuk QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            oWAMD.PCMFormat = PCMFormat;

            TimeDelta dur = Time.Zero.AddTimeDelta(AudioDuration).GetTimeDelta(splitPoint);

            Time elapsed = Time.Zero;
            List<WavClip> clips = new List<WavClip>(mWavClips);
            mWavClips.Clear();
            oWAMD.mWavClips.Clear();
            for (int i = 0; i < clips.Count; i++)
            {
                WavClip curClip = clips[i];
                Time endCurClip = elapsed.AddTimeDelta(curClip.Duration);
                if (splitPoint.IsLessThanOrEqualTo(elapsed))
                {
                    oWAMD.mWavClips.Add(curClip);
                }
                else if (splitPoint.IsLessThan(endCurClip))
                {
                    WavClip secondPartClip = new WavClip(
                        curClip.DataProvider,
                        curClip.ClipBegin,
                        curClip.IsClipEndTiedToEOM ? null as Time : curClip.ClipEnd);
                    curClip.ClipEnd = curClip.ClipBegin.AddTime(splitPoint.SubtractTime(elapsed));
                    secondPartClip.ClipBegin = curClip.ClipEnd;
                    mWavClips.Add(curClip);
                    oWAMD.mWavClips.Add(secondPartClip);
                }
                else
                {
                    mWavClips.Add(curClip);
                }
                elapsed = elapsed.AddTimeDelta(curClip.Duration);
            }
            NotifyAudioDataRemoved(this, splitPoint, dur);
            oWAMD.NotifyAudioDataInserted(oWAMD, Time.Zero, dur);
            return oWAMD;
        }
    }
}