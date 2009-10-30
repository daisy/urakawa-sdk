using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
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
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            WavAudioMediaData otherz = other as WavAudioMediaData;
            if (otherz == null)
            {
                return false;
            }

            ///Here we do not compare the WavClip equality. Instead, we let the super class compare the resulting streams of PCM data.
            ///See AudioMediaData.ValueEquals();

            return true;
        }

        public override string GetTypeNameFormatted()
        {
            return XukStrings.WavAudioMediaData;
        }
        /// <summary>
        /// Represents a RIFF WAVE PCM audio data clip
        /// </summary>
        protected class WavClip : Clip, IValueEquatable<WavClip>
        {
            /// <summary>
            /// Constructor setting the <see cref="data.DataProvider"/>, 
            /// clip begin and clip end will in this case be initialized to <c>null</c>,
            /// which means beginning/end if the RIFF WAVE PCM data
            /// </summary>
            /// <param name="clipDataProvider">The <see cref="data.DataProvider"/></param>
            public WavClip(DataProvider clipDataProvider)
                : this(clipDataProvider, new Time(), null)
            {
            }

            /// <summary>
            /// Constructor setting the <see cref="data.DataProvider"/> and clip begin/end values
            /// </summary>
            /// <param name="clipDataProvider">The <see cref="data.DataProvider"/> - can not be <c>null</c></param>
            /// <param name="clipBegin">The clip begin <see cref="Time"/> - can not be <c>null</c></param>
            /// <param name="clipEnd">
            /// The clip end <see cref="Time"/>
            /// - a <c>null</c> value ties clip end to the end of the underlying wave audio</param>
            public WavClip(DataProvider clipDataProvider, Time clipBegin, Time clipEnd)
            {
                if (clipDataProvider == null)
                {
                    throw new exception.MethodParameterIsNullException("The data provider of a WavClip can not be null");
                }
                mDataProvider = clipDataProvider;
                ClipBegin = clipBegin;
                ClipEnd = clipEnd;
            }

            private TimeDelta cachedDuration = null;

            /// <summary>
            /// Gets the duration of the underlying RIFF wav file 
            /// </summary>
            /// <returns>The duration</returns>
            public override TimeDelta MediaDuration
            {
                get
                {
                    if (cachedDuration == null)
                    {
                        Stream raw = DataProvider.OpenInputStream();

                        uint dataLength;
                        AudioLibPCMFormat format;
                        try
                        {
                            format = AudioLibPCMFormat.RiffHeaderParse(raw, out dataLength);
                        }
                        finally
                        {
                            raw.Close();
                        }
                        cachedDuration = new TimeDelta(format.ConvertBytesToTime(dataLength));
                    }
                    return cachedDuration;
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
                //TODO: Check that sharing DataProvider with the copy is not a problem
                // REMARK: FileDataProviders: once created, binary content (including RIFF header) is never changed.
                // therefore, OPEN-only FilsStream access should work concurrently (i.e. FileShare.Read)
                return new WavClip(DataProvider, ClipBegin.Copy(), clipEnd);
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

            private DataProvider mDataProvider;

            /// <summary>
            /// Gets the <see cref="data.DataProvider"/> storing the RIFF WAVE PCM audio data of <c>this</c>
            /// </summary>
            /// <returns>The <see cref="data.DataProvider"/></returns>
            public DataProvider DataProvider
            {
                get { return mDataProvider; }
            }

            /// <summary>
            /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
            /// </summary>
            /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
            public Stream OpenPcmInputStream()
            {
                return OpenPcmInputStream(Time.Zero);
            }

            /// <summary>
            /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
            /// after a given sub-clip begin time
            /// </summary>
            /// <param name="subClipBegin"></param>
            /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
            /// <seealso cref="OpenPcmInputStream(urakawa.media.timing.Time,urakawa.media.timing.Time)"/>
            public Stream OpenPcmInputStream(Time subClipBegin)
            {
                return OpenPcmInputStream(subClipBegin, Time.Zero.AddTimeDelta(Duration));
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
            public Stream OpenPcmInputStream(Time subClipBegin, Time subClipEnd)
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
                Stream raw = DataProvider.OpenInputStream();

                uint dataLength;
                AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(raw, out dataLength);

                Time rawEndTime = Time.Zero.AddTimeDelta(new TimeDelta(format.ConvertBytesToTime(dataLength)));
                if (
                    ClipBegin.IsLessThan(Time.Zero)
                    || ClipBegin.IsGreaterThan(ClipEnd)
                    || ClipEnd.IsGreaterThan(rawEndTime))
                {
                    string msg = String.Format(
                        "WavClip [{0};{1}] is empty or not within the underlying wave data stream ([0;{2}])",
                        ClipBegin, ClipEnd, rawEndTime);
                    throw new exception.InvalidDataFormatException(msg);
                }
                /*
                TimeDelta clipDuration = Duration;
                if (subClipBegin.IsEqualTo(Time.Zero) && subClipEnd.IsEqualTo(Time.Zero.AddTimeDelta(clipDuration)))
                {
                    // Stream.Position is at the end of the RIFF header, we need to bring it back to the begining
                    return new SubStream(
                    raw,
                    raw.Position, raw.Length - raw.Position); 
                }
                */
                Time rawClipBegin = ClipBegin.AddTime(subClipBegin);
                Time rawClipEnd = ClipBegin.AddTime(subClipEnd);

                long beginPos = raw.Position + format.ConvertTimeToBytes(rawClipBegin.TimeAsMillisecondFloat);

                long endPos = raw.Position + format.ConvertTimeToBytes(rawClipEnd.TimeAsMillisecondFloat);

                return new SubStream(
                    raw,
                    beginPos,
                    endPos - beginPos);
            }

            #region IValueEquatable<WavClip> Members

            /// <summary>
            /// Determines of <c>this</c> has the same value as a given other instance
            /// </summary>
            /// <param name="other">The other instance</param>
            /// <returns>A <see cref="bool"/> indicating the result</returns>
            public bool ValueEquals(WavClip other)
            {
                if (other == null)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (other.GetType() != GetType())
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (!ClipBegin.IsEqualTo(other.ClipBegin))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (IsClipEndTiedToEOM != other.IsClipEndTiedToEOM)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (!ClipEnd.IsEqualTo(other.ClipEnd))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                if (!DataProvider.ValueEquals(other.DataProvider))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }
                return true;
            }

            #endregion
        }

        /// <summary>
        /// Stores the <see cref="WavClip"/>s of <c>this</c>
        /// </summary>
        private List<WavClip> mWavClips = new List<WavClip>();

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
        /// Gets a <see cref="WavClip"/> from a RAW PCM audio <see cref="Stream"/> of a given duration
        /// </summary>
        /// <param name="pcmData">The raw PCM data stream</param>
        /// <param name="duration">The duration</param>
        /// <returns>The <see cref="WavClip"/></returns>
        protected DataProvider CreateDataProviderFromRawPCMStream(Stream pcmData, TimeDelta duration)
        {
            DataProvider newSingleDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);

            uint dataLength;

            if (duration == null)
            {
                dataLength = (uint)(pcmData.Length - pcmData.Position);
                dataLength -= dataLength % PCMFormat.Data.BlockAlign;
            }
            else
            {
                dataLength = (uint)PCMFormat.Data.ConvertTimeToBytes(duration.TimeDeltaAsMillisecondDouble);
            }

            Stream nsdps = newSingleDataProvider.OpenOutputStream();
            try
            {
                ulong pos = PCMFormat.Data.RiffHeaderWrite(nsdps, dataLength);
            }
            finally
            {
                nsdps.Close();
            }

            newSingleDataProvider.AppendData(pcmData, dataLength);

            return newSingleDataProvider;

            //WavClip newSingleWavClip = new WavClip(newSingleDataProvider);
            //return newSingleWavClip;
        }


        /// <summary>
        /// Forces the PCM data to be stored in a single <see cref="DataProvider"/>.
        /// </summary>
        /// <remarks>
        /// This effectively copies the data, 
        /// since the <see cref="DataProvider"/>(s) previously used to store the PCM data are left untouched
        /// </remarks>
        public void ForceSingleDataProvider()
        {
            if (mWavClips.Count == 1) return;

            WavClip newSingleClip;

            Stream audioData = OpenPcmInputStream();
            try
            {
                newSingleClip = new WavClip(CreateDataProviderFromRawPCMStream(audioData, null));
            }
            finally
            {
                audioData.Close();
            }
            mWavClips.Clear();
            mWavClips.Add(newSingleClip);
        }

        #region AudioMediaData


        /// <summary>
        /// Creates a copy of <c>this</c>, including copies of all <see cref="DataProvider"/>s used by <c>this</c>
        /// </summary>
        /// <returns>The copy</returns>
        public new WavAudioMediaData Copy()
        {
            return CopyProtected() as WavAudioMediaData;
        }

        /// <summary>
        /// Part of technical solution to make copy method return correct type. 
        /// In implementing classes this method should return a copy of the class instances
        /// </summary>
        /// <returns>The copy</returns>
        protected override MediaData CopyProtected()
        {
            WavAudioMediaData copy = Presentation.MediaDataFactory.Create<WavAudioMediaData>();
            copy.PCMFormat = PCMFormat;
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
        protected override MediaData ExportProtected(Presentation destPres)
        {
            WavAudioMediaData expWAMD = destPres.MediaDataFactory.Create<WavAudioMediaData>();
            expWAMD.PCMFormat = PCMFormat;
            foreach (WavClip clip in mWavClips)
            {
                expWAMD.mWavClips.Add(clip.Export(destPres));
            }
            return expWAMD;
        }

        /// <summary>
        /// Exports <c>this</c> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentation</param>
        /// <returns>The exported wav audio media data</returns>
        public new WavAudioMediaData Export(Presentation destPres)
        {
            return ExportProtected(destPres) as WavAudioMediaData;
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
        /// Gets a <see cref="List{IDataProvider}"/> of the <see cref="DataProvider"/>s
        /// used to store the Wav audio data
        /// </summary>
        /// <returns>The <see cref="List{DataProvider}"/></returns>
        public override IEnumerable<DataProvider> UsedDataProviders
        {
            get
            {
                List<DataProvider> usedDP = new List<DataProvider>(mWavClips.Count);
                foreach (WavClip clip in mWavClips)
                {
                    if (!usedDP.Contains(clip.DataProvider)) usedDP.Add(clip.DataProvider);
                }
                return usedDP;
            }
        }

        #endregion

        public override bool HasActualPcmData
        {
            get { return mWavClips.Count > 0; }
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="Time"/>s
        /// as raw PCM data
        /// </summary>
        /// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
        /// <param name="clipEnd">The given clip end <see cref="Time"/></param>
        /// <returns>The <see cref="Stream"/></returns>
        public override Stream OpenPcmInputStream(Time clipBegin, Time clipEnd)
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
            if (mWavClips.Count == 0)
            {
                Debug.Fail("That's probably going to create problems ?");
                return new MemoryStream(0);
            }
            if (mWavClips.Count == 1)
            {
                return mWavClips[0].OpenPcmInputStream(
                    clipBegin,
                    clipEnd);
            }

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
                        resStreams.Add(curClip.OpenPcmInputStream(
                                           Time.Zero.AddTimeDelta(clipBegin.GetTimeDelta(elapsedTime))));
                    }
                    else
                    {
                        //Add part of current clip between clipBegin and clipEnd
                        resStreams.Add(curClip.OpenPcmInputStream(
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
                        resStreams.Add(curClip.OpenPcmInputStream());
                    }
                    else
                    {
                        //Add part of current clip between elapsedTime and clipEnd
                        //(ie. before clipEnd since elapsedTime is at the beginning of the clip)
                        resStreams.Add(curClip.OpenPcmInputStream(
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
                Debug.Fail("That's probably going to create problems ?");
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
        public override void AppendPcmData(Stream pcmData, TimeDelta duration)
        {
            AppendPcmData(CreateDataProviderFromRawPCMStream(pcmData, duration));

            //Time insertPoint = Time.Zero.AddTimeDelta(AudioDuration);
            //WavClip newAppClip = new WavClip(CreateDataProviderFromRawPCMStream(pcmData, duration));
            //mWavClips.Add(newAppClip);
            //if (duration == null) duration = newAppClip.MediaDuration;
            //NotifyAudioDataInserted(this, insertPoint, duration);
        }

        public override void AppendPcmData(DataProvider fileDataProvider)
        {
            if (fileDataProvider.MimeType != DataProviderFactory.AUDIO_WAV_MIME_TYPE)
            {
                throw new exception.OperationNotValidException(
                    "The mime type of the given DataProvider is not WAV !");
            }

            WavClip newSingleWavClip = new WavClip(fileDataProvider);
            mWavClips.Add(newSingleWavClip);

            NotifyAudioDataInserted(this, Time.Zero.AddTimeDelta(AudioDuration), newSingleWavClip.MediaDuration);
        }

        public override void AppendPcmData(DataProvider fileDataProvider, Time clipBegin, Time clipEnd)
        {
            if (fileDataProvider.MimeType != DataProviderFactory.AUDIO_WAV_MIME_TYPE)
            {
                throw new exception.OperationNotValidException(
                    "The mime type of the given DataProvider is not WAV !");
            }

            WavClip newSingleWavClip = new WavClip(fileDataProvider);
            newSingleWavClip.ClipBegin = clipBegin;
            newSingleWavClip.ClipEnd = clipEnd;
            mWavClips.Add(newSingleWavClip);

            NotifyAudioDataInserted(this, Time.Zero.AddTimeDelta(AudioDuration), newSingleWavClip.ClipEnd.GetTimeDelta(newSingleWavClip.ClipBegin));
        }

        /// <summary>
        /// Inserts audio of a given duration from a given source PCM data <see cref="Stream"/> to the wav audio media data
        /// at a given point
        /// </summary>
        /// <param name="pcmData">The source PCM data stream</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration of the aduio to append</param>
        public override void InsertPcmData(Stream pcmData, Time insertPoint, TimeDelta duration)
        {
            Time insPt = insertPoint.Copy();
            if (insPt.IsLessThan(Time.Zero))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given insert point is negative");
            }
            WavClip newInsClip = new WavClip(CreateDataProviderFromRawPCMStream(pcmData, duration));
            Time endTime = Time.Zero.AddTimeDelta(AudioDuration);
            if (insertPoint.IsGreaterThan(endTime))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given insert point is beyond the end of the WavAudioMediaData");
            }
            if (insertPoint.IsEqualTo(endTime))
            {
                mWavClips.Add(newInsClip);
                NotifyAudioDataInserted(this, insertPoint, duration);
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
                if (insPt.IsLessThan(elapsedTime.AddTimeDelta(curClip.Duration)))
                {
                    //If the insert point is between the beginning and end of the current clip, 
                    //Replace the current clip with three clips containing 
                    //the audio in the current clip before the insert point,
                    //the audio to be inserted and the audio in the current clip after the insert point respectively
                    Time insPtInCurClip = Time.Zero.AddTimeDelta(insPt.GetTimeDelta(elapsedTime));
                    WavClip curClipBeforeIns, curClipAfterIns;
                    Stream audioDataStream = curClip.OpenPcmInputStream(Time.Zero, insPtInCurClip);
                    try
                    {
                        curClipBeforeIns = new WavClip(CreateDataProviderFromRawPCMStream(audioDataStream, null));
                    }
                    finally
                    {
                        audioDataStream.Close();
                    }
                    audioDataStream = curClip.OpenPcmInputStream(insPtInCurClip);
                    try
                    {
                        curClipAfterIns = new WavClip(CreateDataProviderFromRawPCMStream(audioDataStream, null));
                    }
                    finally
                    {
                        audioDataStream.Close();
                    }
                    mWavClips.RemoveAt(clipIndex);
                    mWavClips.InsertRange(clipIndex, new WavClip[] { curClipBeforeIns, newInsClip, curClipAfterIns });
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
        public override void RemovePcmData(Time clipBegin)
        {
            if (clipBegin == Time.Zero)
            {
                TimeDelta prevDur = AudioDuration;
                mWavClips.Clear();
                NotifyAudioDataRemoved(this, clipBegin, prevDur);
            }
            else
            {
                base.RemovePcmData(clipBegin);
            }
        }

        /// <summary>
        /// Removes the audio between given clip begin and end points
        /// </summary>
        /// <param name="clipBegin">The given clip begin point</param>
        /// <param name="clipEnd">The given clip end point</param>
        public override void RemovePcmData(Time clipBegin, Time clipEnd)
        {
            if (clipBegin == null || clipEnd == null)
            {
                throw new exception.MethodParameterIsNullException("Clip begin or clip end can not be null");
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
                    Stream beyondAS = curClip.OpenPcmInputStream(curClip.ClipEnd.SubtractTimeDelta(beyondPartDur));
                    WavClip beyondPartClip;
                    try
                    {
                        beyondPartClip = new WavClip(CreateDataProviderFromRawPCMStream(beyondAS, null));
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
            if (source.NamespaceURI == XukNamespaceUri)
            {
                readItem = true;
                if (source.LocalName == XukStrings.WavClips)
                {
                    XukInWavClips(source);
                }
                else if (!Presentation.Project.IsPrettyFormat() && source.LocalName == XukStrings.WavClip)
                {
                    XukInWavClip(source);
                }
                else if (source.LocalName == XukStrings.PCMFormat)
                {
                    XukInPCMFormat(source, handler);
                }
                else
                {
                    readItem = false;
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
                        if (source.LocalName == XukStrings.PCMFormatInfo && source.NamespaceURI == XukNamespaceUri)
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
                        if (source.LocalName == XukStrings.WavClip && source.NamespaceURI == XukNamespaceUri)
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
            string clipBeginAttr = source.GetAttribute(XukStrings.ClipBegin);
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
            string clipEndAttr = source.GetAttribute(XukStrings.ClipEnd);
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
            string dataProviderUid = source.GetAttribute(XukStrings.DataProvider);
            if (dataProviderUid == null)
            {
                throw new exception.XukException("dataProvider attribute is missing from WavClip element");
            }
            DataProvider prov = Presentation.DataProviderManager.GetManagedObject(dataProviderUid);

            try
            {
                mWavClips.Add(new WavClip(prov, cb, ce));
            }
            catch (exception.DataMissingException ex)
            {
                // TODO: this is a temporary fix ! Instead of ignoring the fact that the underlying audio resource is missing, we should have a system to let the consumer of the SDK (i.e. the host application) know about the error and decide about the processing (i.e. abandon parsing or carry-on by ignoring the resource). This relates to the global issue of configurable error recovery, not only for the data attached to the XUK instance, but also for corrupted XUK markup or values. 
                Presentation.Project.notifyDataIsMissing(this, ex);
            }

            if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (!Presentation.Project.IsPrettyFormat())
            {
                //destination.WriteAttributeString(XukStrings.Uid, Uid);
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

            if (!PCMFormat.ValueEquals(Presentation.MediaDataManager.DefaultPCMFormat))
            {
                destination.WriteStartElement(XukStrings.PCMFormat);
                PCMFormat.XukOut(destination, baseUri, handler);
                destination.WriteEndElement();
            }
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteStartElement(XukStrings.WavClips, XukNamespaceUri);
            }
            foreach (WavClip clip in mWavClips)
            {
                destination.WriteStartElement(XukStrings.WavClip, XukNamespaceUri);
                destination.WriteAttributeString(XukStrings.DataProvider, clip.DataProvider.Uid);
                if (!clip.ClipBegin.IsEqualTo(Time.Zero))
                {
                    destination.WriteAttributeString(XukStrings.ClipBegin, clip.ClipBegin.ToString());
                }
                if (!clip.IsClipEndTiedToEOM)
                {
                    destination.WriteAttributeString(XukStrings.ClipEnd, clip.ClipEnd.ToString());
                }
                destination.WriteEndElement();
            }
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteEndElement();
            }
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
            if (other == this)
            {
                throw new exception.OperationNotValidException("Can not merge a AudioMediaData with itself");
            }
            if (other is WavAudioMediaData)
            {
                if (!PCMFormat.Data.IsCompatibleWith(other.PCMFormat.Data))
                {
                    throw new exception.InvalidDataFormatException(
                        "Can not merge this with a WavAudioMediaData with incompatible audio data");
                }
                Time thisInsertPoint = Time.Zero.AddTimeDelta(AudioDuration);
                WavAudioMediaData otherWav = (WavAudioMediaData)other;
                mWavClips.AddRange(otherWav.mWavClips);
                TimeDelta dur = otherWav.AudioDuration;
                NotifyAudioDataInserted(this, thisInsertPoint, dur);
                otherWav.RemovePcmData(Time.Zero);
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
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format(
                        "Split point {0} is beyond the WavAudioMediaData end - audio length is {1}",
                        splitPoint, AudioDuration));
            }
            WavAudioMediaData oWAMD =
                Presentation.MediaDataFactory.Create(GetType()) as WavAudioMediaData;
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
                        curClip.IsClipEndTiedToEOM ? null : curClip.ClipEnd);
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