using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.exception;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media.data.audio.codec
{
    [XukNameUglyPrettyAttribute("wvAu", "WavAudioMediaData")]
    public class WavAudioMediaData : AudioMediaData
    {
        public static readonly UglyPrettyName WavClip_NAME = new UglyPrettyName("wvCl", "WavClip");

        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_WAV_MIME_TYPE; }
        }

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

            //Here we do not compare the WavClip/FileDataProvider equality. Instead, we let the super class compare the resulting streams of PCM data.
            //See AudioMediaData.ValueEquals();

            return true;
        }


        // TODO: WavAudioMediaData.mWavClips should not be exposed publicly (used in Cleaner.cs)
        /// <summary>
        /// Stores the <see cref="WavClip"/>s of <c>this</c>
        /// </summary>
        public List<WavClip> mWavClips = new List<WavClip>();

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
                if (!PCMFormat.Data.IsCompatibleWith(newFormat.Data))
                {
                    failReason =
                        "Cannot change the PCMFormat of the WavAudioMediaData after audio data has been added to it";
                    return false;
                }
            }
            return true;
        }

        protected DataProvider CreateDataProviderFromRawPCMStream(Stream pcmData, Time duration)
        {
            return CreateDataProviderFromRawPCMStream(pcmData, duration, null);
        }

        /// <summary>
        /// Gets a <see cref="WavClip"/> from a RAW PCM audio <see cref="Stream"/> of a given duration
        /// </summary>
        /// <param name="pcmData">The raw PCM data stream</param>
        /// <param name="duration">The duration</param>
        /// <returns>The <see cref="WavClip"/></returns>
        protected DataProvider CreateDataProviderFromRawPCMStream(Stream pcmData, Time duration, string fileNamePrefix)
        {
            DataProvider newSingleDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            ((FileDataProvider)newSingleDataProvider).SetNamePrefix(fileNamePrefix);

            uint dataLength;

            if (duration == null)
            {
                dataLength = (uint)(pcmData.Length - pcmData.Position);
                dataLength -= dataLength % PCMFormat.Data.BlockAlign;
            }
            else
            {
                dataLength = (uint)PCMFormat.Data.ConvertTimeToBytes(duration.AsLocalUnits);
            }

            Stream nsdps = newSingleDataProvider.OpenOutputStream();
            try
            {
                ulong pos = PCMFormat.Data.RiffHeaderWrite(nsdps, dataLength);
            }
            finally
            {
                nsdps.Close();
                nsdps = null;
            }

            newSingleDataProvider.AppendData(pcmData, dataLength);

            return newSingleDataProvider;

            //WavClip newSingleWavClip = new WavClip(newSingleDataProvider);
            //return newSingleWavClip;
        }


        public DataProvider ForceSingleDataProvider()
        {
            return ForceSingleDataProvider(true);
        }

        public DataProvider ForceSingleDataProvider(bool reuseSingleWavClip)
        {
            return ForceSingleDataProvider(reuseSingleWavClip, null);
        }

        public DataProvider GetSingleWavClipDataProvider()
        {
            if (mWavClips.Count == 1)
            {
                WavClip theChosenOne = mWavClips[0];
                return theChosenOne.DataProvider;
            }

            return null;
        }

        /// <summary>
        /// Forces the PCM data to be stored in a single <see cref="DataProvider"/>.
        /// </summary>
        /// <remarks>
        /// This effectively copies the data, 
        /// since the <see cref="DataProvider"/>(s) previously used to store the PCM data are left untouched
        /// </remarks>
        public DataProvider ForceSingleDataProvider(bool reuseSingleWavClip, string fileNamePrefix)
        {
            if (mWavClips.Count == 0) return null;

            if (reuseSingleWavClip && mWavClips.Count == 1)
            {
                WavClip theChosenOne = mWavClips[0];
                if (theChosenOne.ClipBegin.IsEqualTo(Time.Zero)
                    && theChosenOne.ClipEnd.IsEqualTo(theChosenOne.MediaDuration))
                {
                    if (!String.IsNullOrEmpty(fileNamePrefix))
                    {
                        ((FileDataProvider)theChosenOne.DataProvider).Rename(fileNamePrefix);
                    }
                    return theChosenOne.DataProvider;
                }
            }

            WavClip newSingleClip;

            DataProvider dataProvider = null;

            Stream audioData = OpenPcmInputStream();
            try
            {
                dataProvider = CreateDataProviderFromRawPCMStream(audioData, null, fileNamePrefix);
                newSingleClip = new WavClip(dataProvider);
            }
            finally
            {
                audioData.Close();
            }
            mWavClips.Clear();
            mWavClips.Add(newSingleClip);

            return dataProvider;
        }

        #region AudioMediaData


        public WavAudioMediaData Copy(Time timeBegin, Time timeEnd)
        {
            if (timeBegin == null || timeEnd == null)
            {
                throw new exception.MethodParameterIsNullException("Clip begin or clip end can not be null");
            }


            if (
                timeBegin.IsLessThan(Time.Zero)
                || timeEnd.IsLessThan(Time.Zero)
                || (timeEnd.IsGreaterThan(Time.Zero) && timeBegin.IsGreaterThan(timeEnd))
                || timeEnd.IsGreaterThan(AudioDuration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format("The given clip times are not valid, must be between 00:00:00.000 and {0} ([{1} --> {2})",
                                  AudioDuration, timeBegin, timeEnd));
            }

            WavAudioMediaData copy = Copy();

            if (timeEnd.IsGreaterThan(Time.Zero) && timeEnd.IsLessThan(AudioDuration))
            {
                copy.RemovePcmData(timeEnd, AudioDuration);
            }

            if (timeBegin.IsGreaterThan(Time.Zero))
            {
                copy.RemovePcmData(Time.Zero, timeBegin);
            }

            return copy;
        }

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
            copy.PCMFormat = PCMFormat.Copy();
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
            expWAMD.PCMFormat = PCMFormat.Copy();

            // it is better to export at the level of asset stream and leave the clip composition behind
            // exporting clips also exports the shared DataProvider multiple times, which increases the size exponentially
            Stream stream = this.OpenPcmInputStream();
            try
            {
                expWAMD.AppendPcmData(stream, null);
            }
            finally
            {
                stream.Close();
            }
            
            //foreach (WavClip clip in mWavClips)
            //{
                //expWAMD.mWavClips.Add(clip.Export(destPres));
            //}
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
            if (clipBegin.IsNegative)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The clip begin value can not be a negative time");
            }
            if (clipEnd.IsLessThan(clipBegin))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The clip end can not be before clip begin");
            }
            if (clipEnd.IsGreaterThan(AudioDuration))
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

#if USE_NORMAL_LIST
            List
#else
            LightLinkedList
#endif //USE_NORMAL_LIST
<Stream> resStreams = new
#if USE_NORMAL_LIST
            List
#else
 LightLinkedList
#endif //USE_NORMAL_LIST
<Stream>();

            while (i < mWavClips.Count)
            {
                WavClip curClip = mWavClips[i];
                Time currentClipDuration = curClip.Duration;
                Time newElapsedTime = new Time(elapsedTime.AsTimeSpanTicks + currentClipDuration.AsTimeSpanTicks, true);
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
                                           clipBegin.GetDifference(elapsedTime)));
                    }
                    else
                    {
                        //Add part of current clip between clipBegin and clipEnd
                        resStreams.Add(curClip.OpenPcmInputStream(
                                           clipBegin.GetDifference(elapsedTime),
                                           clipEnd.GetDifference(elapsedTime)));
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
                                           clipEnd.GetDifference(elapsedTime)));
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
            if (resStreams.IsEmpty)
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
        public override void AppendPcmData(Stream pcmData, Time duration)
        {
            AppendPcmData(CreateDataProviderFromRawPCMStream(pcmData, duration));

            //Time insertPoint = Time.Zero.Add(AudioDuration);
            //WavClip newAppClip = new WavClip(CreateDataProviderFromRawPCMStream(pcmData, duration));
            //mWavClips.Add(newAppClip);
            //if (duration == null) duration = newAppClip.MediaDuration;
            //NotifyAudioDataInserted(this, insertPoint, duration);
        }

        private void checkPcmFormat(WavClip clip)
        {
            if (!Presentation.MediaDataManager.EnforceSinglePCMFormat)
            {
                return;
            }

            if (!clip.PcmFormat.IsCompatibleWith(PCMFormat.Data))
            {
                throw new exception.InvalidDataFormatException(
                    String.Format("RIFF WAV file has incompatible PCM format"));
            }

            ////Stream stream = newSingleWavClip.OpenPcmInputStream(); // Skips the RIFF header !
            //Stream stream = clip.DataProvider.OpenInputStream();
            //try
            //{
            //    checkPcmFormat(stream);
            //}
            //finally
            //{
            //    stream.Close();
            //}
        }

        //private void checkPcmFormat(Stream stream)
        //{
        //    if (!Presentation.MediaDataManager.EnforceSinglePCMFormat)
        //    {
        //        return;
        //    }

        //    uint dataLength;
        //    AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(stream, out dataLength);

        //    //if (dataLength <= 0)
        //    //{
        //    //    dataLength = (uint)(stream.Length - stream.Position);
        //    //}

        //    if (!format.IsCompatibleWith(PCMFormat.Data))
        //    {
        //        throw new exception.InvalidDataFormatException(
        //            String.Format("RIFF WAV file has incompatible PCM format"));
        //    }
        //}

        public override void AppendPcmData(DataProvider fileDataProvider)
        {
            if (fileDataProvider.MimeType != DataProviderFactory.AUDIO_WAV_MIME_TYPE)
            {
                throw new exception.OperationNotValidException(
                    "The mime type of the given DataProvider is not WAV !");
            }

            WavClip newSingleWavClip = new WavClip(fileDataProvider);
            mWavClips.Add(newSingleWavClip);

            checkPcmFormat(newSingleWavClip);

            NotifyAudioDataInserted(this, AudioDuration, newSingleWavClip.MediaDuration);
        }

        public override void AppendPcmData(DataProvider fileDataProvider, Time clipBegin, Time clipEnd)
        {
            if (fileDataProvider.MimeType != DataProviderFactory.AUDIO_WAV_MIME_TYPE)
            {
                throw new exception.OperationNotValidException(
                    "The mime type of the given DataProvider is not WAV !");
            }

            WavClip newSingleWavClip = new WavClip(fileDataProvider);
            if (clipBegin != null)
            {
                newSingleWavClip.ClipBegin = clipBegin.Copy();
            }
            if (clipEnd != null)
            {
                newSingleWavClip.ClipEnd = clipEnd.Copy();
            }
            mWavClips.Add(newSingleWavClip);

            checkPcmFormat(newSingleWavClip);

            NotifyAudioDataInserted(this, AudioDuration, newSingleWavClip.Duration); //ClipEnd.GetDifference(newSingleWavClip.ClipBegin));
        }

        protected void InsertPcmData(WavClip newInsClip, Time insertPoint, Time duration)
        {
            if (insertPoint.IsLessThan(Time.Zero))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given insert point is negative");
            }

            Time endTime = AudioDuration;
            if (insertPoint.IsGreaterThan(endTime))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The given insert point is beyond the end of the WavAudioMediaData");
            }


            checkPcmFormat(newInsClip);

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
                if (insertPoint.IsEqualTo(elapsedTime))
                {
                    //If the insert point at the beginning of the current clip, insert the new clip and break
                    mWavClips.Insert(clipIndex, newInsClip);
                    break;
                }
                if (insertPoint.IsLessThan(new Time(elapsedTime.AsTimeSpanTicks + curClip.Duration.AsTimeSpanTicks, true)))
                {
                    //If the insert point is between the beginning and end of the current clip, 
                    //Replace the current clip with three clips containing 
                    //the audio in the current clip before the insert point,
                    //the audio to be inserted and the audio in the current clip after the insert point respectively
                    Time insPtInCurClip = insertPoint.GetDifference(elapsedTime);

                    WavClip curClipBeforeIns = curClip.Copy();
                    curClipBeforeIns.ClipEnd = curClipBeforeIns.ClipBegin.Copy();
                    curClipBeforeIns.ClipEnd.Add(insPtInCurClip);

                    //Stream audioDataStream = curClip.OpenPcmInputStream(Time.Zero, insPtInCurClip);
                    //try
                    //{
                    //    curClipBeforeIns = new WavClip(CreateDataProviderFromRawPCMStream(audioDataStream, null));
                    //}
                    //finally
                    //{
                    //    audioDataStream.Close();
                    //}

                    WavClip curClipAfterIns = curClip.Copy();
                    curClipAfterIns.ClipBegin.Add(insPtInCurClip);

                    //audioDataStream = curClip.OpenPcmInputStream(insPtInCurClip);
                    //try
                    //{
                    //    curClipAfterIns = new WavClip(CreateDataProviderFromRawPCMStream(audioDataStream, null));
                    //}
                    //finally
                    //{
                    //    audioDataStream.Close();
                    //}

                    mWavClips.RemoveAt(clipIndex);
                    mWavClips.InsertRange(clipIndex, new WavClip[] { curClipBeforeIns, newInsClip, curClipAfterIns });
                    break;
                }
                elapsedTime.Add(curClip.Duration);
                clipIndex++;
            }
            NotifyAudioDataInserted(this, insertPoint, duration);
        }

        public void InsertPcmData(WavAudioMediaData mediaData, Time insertPoint, Time duration)
        {
            Time movingInsertPoint = insertPoint.Copy();
            Time remainingAvailableDuration = duration.Copy();

            foreach (WavClip wavClip in mediaData.mWavClips)
            {
                Time wavClipDur = wavClip.Duration;

                if (wavClipDur.IsLessThan(remainingAvailableDuration)
                    || wavClipDur.IsEqualTo(remainingAvailableDuration))
                {
                    InsertPcmData(wavClip.Copy(), movingInsertPoint, wavClipDur);
                    movingInsertPoint.Add(wavClipDur);
                    remainingAvailableDuration.Substract(wavClipDur);

                    if (remainingAvailableDuration.IsLessThan(Time.Zero)
                    || remainingAvailableDuration.IsEqualTo(Time.Zero)) break;
                }
                else
                {
                    WavClip smallerClip = wavClip.Copy();
                    smallerClip.ClipEnd = smallerClip.ClipBegin.Copy();
                    smallerClip.ClipEnd.Add(remainingAvailableDuration);

                    InsertPcmData(smallerClip, movingInsertPoint, remainingAvailableDuration);

                    //Stream stream = wavClip.OpenPcmInputStream(Time.Zero,
                    //    new Time(wavClip.ClipBegin.AsTimeSpan + remainingAvailableDuration.AsTimeSpan));
                    //try
                    //{
                    //    InsertPcmData(stream, movingInsertPoint, remainingAvailableDuration);
                    //}
                    //finally
                    //{
                    //    stream.Close();
                    //}
                    break;
                }
            }
        }

        /// <summary>
        /// Inserts audio of a given duration from a given source PCM data <see cref="Stream"/> to the wav audio media data
        /// at a given point
        /// </summary>
        /// <param name="pcmData">The source PCM data stream</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration of the aduio to append</param>
        public override void InsertPcmData(Stream pcmData, Time insertPoint, Time duration)
        {
            WavClip newInsClip = new WavClip(CreateDataProviderFromRawPCMStream(pcmData, duration));
            InsertPcmData(newInsClip, insertPoint, duration);
        }

        /// <summary>
        /// Gets the intrinsic duration of the audio data
        /// </summary>
        /// <returns>The duration as an <see cref="Time"/></returns>
        public override Time AudioDuration
        {
            get
            {
                Time dur = new Time();
                foreach (WavClip clip in mWavClips)
                {
                    dur.Add(clip.Duration);
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
            if (clipBegin.IsEqualTo(Time.Zero))
            {
                Time prevDur = AudioDuration;
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

            //long oneMS = PCMFormat.Data.ConvertTimeToBytes(1);
            //double timeBlockAlign = PCMFormat.Data.ConvertBytesToTime(PCMFormat.Data.BlockAlign);

            if (
                clipBegin.IsLessThan(Time.Zero)
                || clipBegin.IsGreaterThan(clipEnd)
                || clipEnd.IsGreaterThan(AudioDuration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format("The given clip times are not valid, must be between 00:00:00.000 and {0} ([{1} --> {2})",
                                  AudioDuration, clipBegin, clipEnd));
            }
            Time curBeginTime = Time.Zero;

            List<WavClip> newClipList = new List<WavClip>();
            foreach (WavClip curClip in mWavClips)
            {
                Time curEndTime = new Time(curBeginTime.AsTimeSpanTicks + curClip.Duration.AsTimeSpanTicks, true);
                if ((!curEndTime.IsGreaterThan(clipBegin)) || (!curBeginTime.IsLessThan(clipEnd)))
                {
                    //The current clip is before or beyond the range to remove - 
                    //so the clip is added unaltered to the new list of clips
                    newClipList.Add(curClip);
                }
                else if (curBeginTime.IsLessThan(clipBegin) && curEndTime.IsGreaterThan(clipEnd))
                {
                    //Some of the current clip is before the range and some is after
                    Time beforePartDur = curBeginTime.GetDifference(clipBegin);
                    Time beyondPartDur = curEndTime.GetDifference(clipEnd);


                    //Time timePointRelativeToClipBeginOfWavClip = new Time(
                    //    curClip.Duration.AsTimeSpan - beyondPartDur.AsTimeSpan);
                    //Stream beyondAS = curClip.OpenPcmInputStream(timePointRelativeToClipBeginOfWavClip);
                    //WavClip beyondPartClip;
                    //try
                    //{
                    //    beyondPartClip = new WavClip(CreateDataProviderFromRawPCMStream(beyondAS, null));
                    //}
                    //finally
                    //{
                    //    beyondAS.Close();
                    //}

                    WavClip beyondPartClip = curClip.Copy();
                    Time copyEnd = beyondPartClip.ClipEnd.Copy();
                    copyEnd.Substract(beyondPartDur);
                    beyondPartClip.ClipBegin = copyEnd;

                    curClip.ClipEnd = new Time(curClip.ClipBegin.AsTimeSpanTicks + beforePartDur.AsTimeSpanTicks, true);

                    newClipList.Add(curClip);
                    newClipList.Add(beyondPartClip);
                }
                else if (curBeginTime.IsLessThan(clipBegin) && curEndTime.IsGreaterThan(clipBegin))
                {
                    //Some of the current clip is before the range to remove, none is beyond
                    Time beforePartDur = curBeginTime.GetDifference(clipBegin);
                    curClip.ClipEnd = new Time(curClip.ClipBegin.AsTimeSpanTicks + beforePartDur.AsTimeSpanTicks, true);
                    newClipList.Add(curClip);
                }
                else if (curBeginTime.IsLessThan(clipEnd) && curEndTime.IsGreaterThan(clipEnd))
                {
                    //Some of the current clip is beyond the range to remove, none is before
                    Time beyondPartDur = curEndTime.GetDifference(clipEnd);
                    curClip.ClipBegin = new Time(curClip.ClipEnd.AsTimeSpanTicks - beyondPartDur.AsTimeSpanTicks, true);
                    newClipList.Add(curClip);
                }

                curBeginTime = curEndTime;
            }
            mWavClips = newClipList;
            NotifyAudioDataRemoved(this, clipBegin, clipEnd.GetDifference(clipBegin));
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

        //#if DEBUG
        //        public override void XukIn(XmlReader source, IProgressHandler handler)
        //        {
        //            base.XukIn(source, handler);

        //            checkWavClips();
        //        }
        //#endif //DEBUG

        /// <summary>
        /// Reads a child of a WavAudioMediaData xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (source.LocalName == XukStrings.WavClips)
                {
                    XukInWavClips(source);
                }
                else if (!Presentation.Project.PrettyFormat
                    && WavClip_NAME.Match(source.LocalName)
                    //XukAble.GetXukName(typeof(WavClip))
                    )
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

        private void XukInPCMFormat(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.NamespaceURI == XukAble.XUK_NS
                            && XukAble.GetXukName(typeof(PCMFormatInfo)).Match(source.LocalName)
                            )
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
                        if (source.NamespaceURI == XukAble.XUK_NS
                            && WavClip_NAME.Match(source.LocalName)
                            //XukAble.GetXukName(typeof(WavClip))
                            )
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
            string dataProviderUid = XukAble.ReadXukAttribute(source, DataProvider.DataProvider_NAME);
            if (String.IsNullOrEmpty(dataProviderUid))
            {
                throw new exception.XukException("dataProvider attribute is missing from WavClip element");
            }

            //if (!Presentation.DataProviderManager.IsManagerOf(dataProviderUid))
            //{
            //    throw new exception.IsNotManagerOfException(
            //            String.Format("DataProvider cannot be found {0}", dataProviderUid));
            //}

            try
            {
                DataProvider prov = Presentation.DataProviderManager.GetManagedObject(dataProviderUid);
                mWavClips.Add(new WavClip(prov, cb, ce));
            }
            catch (DataMissingException ex)
            {
                // TODO: this is a temporary fix !
                // Instead of ignoring the fact that the underlying audio resource is missing,
                // we should have a system to let the consumer of the SDK
                // (i.e. the host application) know about the error and decide about the processing
                // (i.e. abandon parsing or carry-on by ignoring the resource).
                // This relates to the global issue of configurable error recovery,
                // not only for the data attached to the XUK instance,
                // but also for corrupted XUK markup or values. 
                Presentation.Project.notifyDataIsMissing(this, ex);
            }
            catch (Exception ex) //MethodParameterIsOutOfBoundsException IsNotManagerOfException
            {
                Presentation.Project.notifyDataIsMissing(this, new DataMissingException(ex.Message, ex));
            }

            if (!source.IsEmptyElement)
            {
                source.ReadSubtree().Close();
            }
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (!Presentation.Project.PrettyFormat)
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            if (!PCMFormat.ValueEquals(Presentation.MediaDataManager.DefaultPCMFormat))
            {
                destination.WriteStartElement(XukStrings.PCMFormat);
                PCMFormat.XukOut(destination, baseUri, handler);
                destination.WriteEndElement();
            }
            if (Presentation.Project.PrettyFormat)
            {
                destination.WriteStartElement(XukStrings.WavClips, XukAble.XUK_NS);
            }
            foreach (WavClip clip in mWavClips)
            {
                destination.WriteStartElement(WavClip_NAME.z(PrettyFormat), XukAble.XUK_NS);
                destination.WriteAttributeString(DataProvider.DataProvider_NAME.z(PrettyFormat), clip.DataProvider.Uid);
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
            if (Presentation.Project.PrettyFormat)
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
                Time thisInsertPoint = AudioDuration;
                WavAudioMediaData otherWav = (WavAudioMediaData)other;
                mWavClips.AddRange(otherWav.mWavClips);
                Time dur = otherWav.AudioDuration;
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
            if (splitPoint.IsNegative)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The split point can not be negative");
            }
            if (splitPoint.IsGreaterThan(AudioDuration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format(
                        "Split point {0} is beyond the WavAudioMediaData end - audio length is {1}",
                        splitPoint, AudioDuration));
            }
            WavAudioMediaData oWAMD =
                Presentation.MediaDataFactory.Create<WavAudioMediaData>();
            if (oWAMD == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "Thrown if the MediaDataFactory can not create a WacAudioMediaData matching Xuk QName {1}:{0}",
                                                                         GetXukName(), GetXukNamespace()));
            }
            oWAMD.PCMFormat = PCMFormat.Copy();

            Time originalDuration = AudioDuration;
            Time newClipDuration = AudioDuration.GetDifference(splitPoint);

            Time curClip_Begin = Time.Zero;
            List<WavClip> clips = new List<WavClip>(mWavClips);
            mWavClips.Clear();
            oWAMD.mWavClips.Clear();
            for (int i = 0; i < clips.Count; i++)
            {
                WavClip curClip = clips[i];
                Time savedCurClipDuration = curClip.Duration;

                Time curClip_End = new Time(curClip_Begin.AsTimeSpanTicks + curClip.Duration.AsTimeSpanTicks, true);
                if (splitPoint.IsLessThanOrEqualTo(curClip_Begin))
                {
                    oWAMD.mWavClips.Add(curClip);
                }
                else if (splitPoint.IsLessThan(curClip_End))
                {
                    WavClip secondPartClip = new WavClip(
                        curClip.DataProvider,
                        curClip.ClipBegin,
                        curClip.IsClipEndTiedToEOM ? null : curClip.ClipEnd);

                    curClip.ClipEnd = new Time(curClip.ClipBegin.AsTimeSpanTicks + (splitPoint.AsTimeSpanTicks - curClip_Begin.AsTimeSpanTicks), true);

                    secondPartClip.ClipBegin = curClip.ClipEnd.Copy();
                    mWavClips.Add(curClip);
                    oWAMD.mWavClips.Add(secondPartClip);
                }
                else
                {
                    mWavClips.Add(curClip);
                }
                curClip_Begin.Add(savedCurClipDuration);
            }
            NotifyAudioDataRemoved(this, splitPoint, newClipDuration);
            oWAMD.NotifyAudioDataInserted(oWAMD, Time.Zero, newClipDuration);

#if DEBUG
            DebugFix.Assert(AudioLibPCMFormat.TimesAreEqualWithMillisecondsTolerance(
                newClipDuration.AsLocalUnits,
                oWAMD.AudioDuration.AsLocalUnits));

            DebugFix.Assert(newClipDuration.IsEqualTo(oWAMD.AudioDuration));



            DebugFix.Assert(AudioLibPCMFormat.TimesAreEqualWithMillisecondsTolerance(
                this.AudioDuration.AsLocalUnits + oWAMD.AudioDuration.AsLocalUnits,
                originalDuration.AsLocalUnits));

            Time copy = this.AudioDuration.Copy();
            copy.Add(oWAMD.AudioDuration);
            DebugFix.Assert(originalDuration.IsEqualTo(copy));
#endif //DEBUG

            return oWAMD;
        }

        //#if DEBUG
        //        public void checkWavClips()
        //        {
        //            DebugFix.Assert(mWavClips != null && mWavClips.Count > 0);
        //        }
        //#endif //DEBUG
    }
}