using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.media.timing;
using urakawa.media.data.utilities;

namespace urakawa.media.data.audio
{
    /// <summary>
    /// Abstract base class for audio <see cref="MediaData"/>.
    /// Implements PCM format accessors (number of channels, bit depth, sample rate) 
    /// and leaves all other methods abstract
    /// </summary>
    public abstract class AudioMediaData : MediaData
    {
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/> has changed
        /// </summary>
        public event EventHandler<events.media.data.audio.PCMFormatChangedEventArgs> PCMFormatChanged;

        /// <summary>
        /// Fires the <see cref="PCMFormatChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="AudioMediaData"/> whoose <see cref="PCMFormatInfo"/> has changed</param>
        /// <param name="newFormat">The new value</param>
        /// <param name="prevFormat">The value prior to the change</param>
        protected void NotifyPCMFormatChanged(AudioMediaData source, PCMFormatInfo newFormat, PCMFormatInfo prevFormat)
        {
            EventHandler<events.media.data.audio.PCMFormatChangedEventArgs> d = PCMFormatChanged;
            if (d != null)
                d(this, new urakawa.events.media.data.audio.PCMFormatChangedEventArgs(source, newFormat, prevFormat));
        }

        private void this_pcmFormatChanged(object sender, urakawa.events.media.data.audio.PCMFormatChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        /// <summary>
        /// Event fired after audio data has been inserted into the <see cref="AudioMediaData"/>
        /// </summary>
        public event EventHandler<events.media.data.audio.AudioDataInsertedEventArgs> AudioDataInserted;

        /// <summary>
        /// Fires the <see cref="AudioDataInserted"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="AudioMediaData"/> into which audio data was inserted</param>
        /// <param name="insertPoint">The insert point at which audio data was inserted</param>
        /// <param name="duration">The duration of the inserted audio data</param>
        protected void NotifyAudioDataInserted(AudioMediaData source, Time insertPoint, TimeDelta duration)
        {
            EventHandler<events.media.data.audio.AudioDataInsertedEventArgs> d = AudioDataInserted;
            if (d != null)
                d(this, new urakawa.events.media.data.audio.AudioDataInsertedEventArgs(source, insertPoint, duration));
        }

        private void this_audioDataInserted(object sender, urakawa.events.media.data.audio.AudioDataInsertedEventArgs e)
        {
            NotifyChanged(e);
        }

        /// <summary>
        /// Event fired after audio data has been removed from the <see cref="AudioMediaData"/>
        /// </summary>
        public event EventHandler<events.media.data.audio.AudioDataRemovedEventArgs> AudioDataRemoved;

        /// <summary>
        /// Fires the <see cref="AudioDataRemoved"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="AudioMediaData"/> from which audio data was removed</param>
        /// <param name="fromPoint">The point at which audio data was removed</param>
        /// <param name="duration">The duration of the removed audio data</param>
        protected void NotifyAudioDataRemoved(AudioMediaData source, Time fromPoint, TimeDelta duration)
        {
            EventHandler<events.media.data.audio.AudioDataRemovedEventArgs> d = AudioDataRemoved;
            if (d != null)
                d(this, new urakawa.events.media.data.audio.AudioDataRemovedEventArgs(source, fromPoint, duration));
        }

        private void this_audioDataRemoved(object sender, urakawa.events.media.data.audio.AudioDataRemovedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        internal AudioMediaData()
        {
            this.PCMFormatChanged +=
                new EventHandler<urakawa.events.media.data.audio.PCMFormatChangedEventArgs>(this_pcmFormatChanged);
            this.AudioDataInserted +=
                new EventHandler<urakawa.events.media.data.audio.AudioDataInsertedEventArgs>(this_audioDataInserted);
            this.AudioDataRemoved +=
                new EventHandler<urakawa.events.media.data.audio.AudioDataRemovedEventArgs>(this_audioDataRemoved);
        }

        private PCMFormatInfo mPCMFormat;

        /// <summary>
        /// Determines if a PCM Format change is ok
        /// </summary>
        /// <param name="newFormat">The new PCM Format value - assumed not to be <c>null</c></param>
        /// <param name="failReason">The <see cref="string"/> to which a failure reason must be written in case the change is not ok</param>
        /// <returns>A <see cref="bool"/> indicating if the change is ok</returns>
        protected virtual bool IsPCMFormatChangeOk(PCMFormatInfo newFormat, out string failReason)
        {
            failReason = "";
            if (MediaDataManager.EnforceSinglePCMFormat)
            {
                if (!MediaDataManager.DefaultPCMFormat.ValueEquals(newFormat))
                {
                    failReason =
                        "When the MediaDataManager enforces a single PCM Format, "
                        + "the PCM Format of the AudioMediaData must match the default defined by the manager";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the <see cref="MediaDataFactory"/>
        /// </summary>
        /// <returns></returns>
        protected MediaDataFactory GetMediaDataFactory()
        {
            return MediaDataManager.MediaDataFactory;
        }

        /// <summary>
        /// Gets (a copy of) the <see cref="PCMFormatInfo"/> of the audio media data 
        /// </summary>
        /// <returns>The PCMFormatInfo</returns>
        public PCMFormatInfo PCMFormat
        {
            get
            {
                if (mPCMFormat == null)
                {
                    mPCMFormat = new PCMFormatInfo(MediaDataManager.DefaultPCMFormat);
                }
                return mPCMFormat.Copy();
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The new PCMFormatInfo can not be null");
                }
                string failReason;
                if (!IsPCMFormatChangeOk(value, out failReason))
                {
                    throw new exception.InvalidDataFormatException(failReason);
                }
                if (!value.ValueEquals(mPCMFormat))
                {
                    PCMFormatInfo prevFormat = mPCMFormat;
                    mPCMFormat = value.Copy();
                    NotifyPCMFormatChanged(this, mPCMFormat.Copy(), prevFormat);
                }
            }
        }

        /// <summary>
        /// Sets the number of channels of the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/>
        /// </summary>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new value is less than <c>1</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format with a different number of channels from the new value
        /// or when audio data with a different number of channels has already been added to the <see cref="AudioMediaData"/>
        /// </exception>
        public ushort NumberOfChannels
        {
            set
            {
                PCMFormatInfo newFormat = PCMFormat;
                newFormat.NumberOfChannels = value;
                PCMFormat = newFormat;
            }
        }

        /// <summary>
        /// Sets the sample rate of the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/>
        /// </summary>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format with a different sample rate from the new value
        /// or when audio data with a different sample rate has already been added to the <see cref="AudioMediaData"/>
        /// </exception>
        public uint SampleRate
        {
            set
            {
                PCMFormatInfo newFormat = PCMFormat;
                newFormat.SampleRate = value;
                PCMFormat = newFormat;
            }
        }

        /// <summary>
        /// Sets the number of channels of the <see cref="PCMFormatInfo"/> of the <see cref="AudioMediaData"/>
        /// </summary>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new value is less than <c>1</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the <see cref="MediaDataManager"/> enforces a single PCM Format with a different bit depth from the new value
        /// or when audio data with a different bit depth has already been added to the <see cref="AudioMediaData"/>
        /// </exception>
        public ushort BitDepth
        {
            set
            {
                PCMFormatInfo newFormat = PCMFormat;
                newFormat.BitDepth = value;
                PCMFormat = newFormat;
            }
        }

        /// <summary>
        /// Gets the count in bytes of PCM data of the audio media data of a given duration
        /// </summary>
        /// <param name="duration">The duration</param>
        /// <returns>The count in bytes</returns>
        public int GetPCMLength(TimeDelta duration)
        {
            return (int) PCMFormat.GetDataLength(duration);
        }

        /// <summary>
        /// Gets the count in bytes of the PCM data of the audio media data
        /// </summary>
        /// <returns>The count in bytes</returns>
        public int GetPCMLength()
        {
            return GetPCMLength(AudioDuration);
        }


        /// <summary>
        /// Gets the intrinsic duration of the audio data
        /// </summary>
        /// <returns>The duration as an <see cref="TimeDelta"/></returns>
        public abstract TimeDelta AudioDuration { get; }


        /// <summary>
        /// Gets an input <see cref="Stream"/> giving access to all audio data as raw PCM
        /// </summary>
        /// <returns>The input <see cref="Stream"/></returns>
        public Stream GetAudioData()
        {
            return GetAudioData(Time.Zero);
        }

        /// <summary>
        /// Gets an input <see cref="Stream"/> giving access to the audio data after a given <see cref="Time"/> 
        /// as raw PCM
        /// </summary>
        /// <returns>The input <see cref="Stream"/></returns>
        /// <remarks>
        /// Make sure to close any audio <see cref="Stream"/> returned by this method when no longer needed.
        /// Failing to do so may cause <see cref="exception.InputStreamsOpenException"/> when trying to added data to or delete
        /// the <see cref="DataProvider"/>s used by the <see cref="AudioMediaData"/> instance
        /// </remarks>
        public Stream GetAudioData(Time clipBegin)
        {
            return GetAudioData(clipBegin, Time.Zero.AddTimeDelta(AudioDuration));
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="Time"/>s
        /// as raw PCM data
        /// </summary>
        /// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
        /// <param name="clipEnd">The given clip end <see cref="Time"/></param>
        /// <returns>The <see cref="Stream"/></returns>
        /// <remarks>
        /// Make sure to close any audio <see cref="Stream"/> returned by this method when no longer needed.
        /// Failing to do so may cause <see cref="exception.InputStreamsOpenException"/> when trying to added data to or delete
        /// the <see cref="DataProvider"/>s used by the <see cref="AudioMediaData"/> instance
        /// </remarks>
        public abstract Stream GetAudioData(Time clipBegin, Time clipEnd);

        /// <summary>
        /// Appends audio of a given duration to <c>this</c>
        /// </summary>
        /// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
        /// <param name="duration">The duration of the audio to add</param>
        public virtual void AppendAudioData(Stream pcmData, TimeDelta duration)
        {
            InsertAudioData(pcmData, new Time(AudioDuration.TimeDeltaAsMillisecondFloat), duration);
        }

        private void ParseRiffWaveStream(Stream riffWaveStream, out TimeDelta duration)
        {
            PCMDataInfo pcmInfo = PCMDataInfo.ParseRiffWaveHeader(riffWaveStream);
            if (!pcmInfo.IsCompatibleWith(PCMFormat))
            {
                throw new exception.InvalidDataFormatException(
                    String.Format("RIFF WAV file has incompatible PCM format"));
            }
            duration = new TimeDelta(pcmInfo.Duration);
        }

        /// <summary>
        /// Appends audio data from a RIFF Wave file
        /// </summary>
        /// <param name="riffWaveStream">The RIFF Wave file</param>
        public void AppendAudioDataFromRiffWave(Stream riffWaveStream)
        {
            TimeDelta duration;
            ParseRiffWaveStream(riffWaveStream, out duration);
            AppendAudioData(riffWaveStream, duration);
        }

        private Stream OpenFileStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Appends audio data from a RIFF Wave file
        /// </summary>
        /// <param name="path">The path of the RIFF Wave file</param>
        public void AppendAudioDataFromRiffWave(string path)
        {
            Stream rwFS = OpenFileStream(path);
            try
            {
                AppendAudioDataFromRiffWave(rwFS);
            }
            finally
            {
                rwFS.Close();
            }
        }


        /// <summary>
        /// Inserts audio data of a given duration at a given insert point
        /// </summary>
        /// <param name="pcmData">A <see cref="Stream"/> providing read access to the audio data as RAW PCM</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration</param>
        public abstract void InsertAudioData(Stream pcmData, Time insertPoint, TimeDelta duration);

        /// <summary>
        /// Inserts audio data from a RIFF Wave file at a given insert point and of a given duration
        /// </summary>
        /// <param name="riffWaveStream">The RIFF Wave file</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration - if <c>null</c> the entire RIFF Wave file is inserted</param>
        public void InsertAudioDataFromRiffWave(Stream riffWaveStream, Time insertPoint, TimeDelta duration)
        {
            TimeDelta fileDuration;
            ParseRiffWaveStream(riffWaveStream, out fileDuration);
            if (duration == null) duration = fileDuration;
            if (fileDuration.IsLessThan(duration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Can not insert {0} of audio from RIFF Wave file since the file's duration is only {1}",
                                                                              duration, fileDuration));
            }
            InsertAudioData(riffWaveStream, insertPoint, duration);
        }

        /// <summary>
        /// Inserts audio data from a RIFF Wave file at a given insert point and of a given duration
        /// </summary>
        /// <param name="path">The path of the RIFF Wave file</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration - if <c>null</c> the entire RIFF Wave file is inserted</param>
        public void InsertAudioDataFromRiffWave(string path, Time insertPoint, TimeDelta duration)
        {
            Stream rwFS = OpenFileStream(path);
            try
            {
                InsertAudioDataFromRiffWave(rwFS, insertPoint, duration);
            }
            finally
            {
                rwFS.Close();
            }
        }

        /// <summary>
        /// Replaces with audio of a given duration at a given replace point
        /// </summary>
        /// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
        /// <param name="replacePoint">The given replace point</param>
        /// <param name="duration">The duration of the audio to replace</param>
        public void ReplaceAudioData(Stream pcmData, Time replacePoint, TimeDelta duration)
        {
            RemoveAudioData(replacePoint, replacePoint.AddTimeDelta(duration));
            InsertAudioData(pcmData, replacePoint, duration);
        }

        /// <summary>
        /// Replaces with audio from a RIFF Wave file of a given duration at a given replace point
        /// </summary>
        /// <param name="riffWaveStream">The RIFF Wave file</param>
        /// <param name="replacePoint">The given replace point</param>
        /// <param name="duration">The duration of the audio to replace</param>
        public void ReplaceAudioDataFromRiffWave(Stream riffWaveStream, Time replacePoint, TimeDelta duration)
        {
            TimeDelta fileDuration;
            ParseRiffWaveStream(riffWaveStream, out fileDuration);
            if (fileDuration.IsLessThan(duration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Can not insert {0} of audio from RIFF Wave file since the file's duration is only {1}",
                                                                              duration, fileDuration));
            }
            ReplaceAudioData(riffWaveStream, replacePoint, duration);
        }

        /// <summary>
        /// Replaces with audio from a RIFF Wave file of a given duration at a given replace point
        /// </summary>
        /// <param name="path">The path of the RIFF Wave file</param>
        /// <param name="replacePoint">The given replace point</param>
        /// <param name="duration">The duration of the audio to replace</param>
        public void ReplaceAudioDataFromRiffWave(string path, Time replacePoint, TimeDelta duration)
        {
            Stream rwFS = OpenFileStream(path);
            try
            {
                ReplaceAudioDataFromRiffWave(rwFS, replacePoint, duration);
            }
            finally
            {
                rwFS.Close();
            }
        }

        /// <summary>
        /// Removes all audio after a given clip begin <see cref="Time"/>
        /// </summary>
        /// <param name="clipBegin">The clip begin</param>
        public virtual void RemoveAudioData(Time clipBegin)
        {
            RemoveAudioData(clipBegin, Time.Zero.AddTimeDelta(AudioDuration));
        }

        /// <summary>
        /// Removes all audio between given clip begin and end <see cref="Time"/>
        /// </summary>
        /// <param name="clipBegin">The givne clip begin <see cref="Time"/></param>
        /// <param name="clipEnd">The givne clip end <see cref="Time"/></param>
        public abstract void RemoveAudioData(Time clipBegin, Time clipEnd);

        ///// <summary>
        ///// Part of technical solution to make copy method return correct type. 
        ///// In implementing classes this method should return a copy of the class instances
        ///// </summary>
        ///// <returns>The copy</returns>
        //protected abstract AudioMediaData AudioMediaDataCopy();

        /// <summary>
        /// Part of technical solution to make copy method return correct type. 
        /// In implementing classes this method should return a copy of the class instances
        /// </summary>
        /// <returns>The copy</returns>
        protected abstract override MediaData CopyProtected();

        /// <summary>
        /// Gets a copy of <c>this</c>
        /// </summary>
        /// <returns>The copy</returns>
        public new AudioMediaData Copy()
        {
            return CopyProtected() as AudioMediaData;
        }

        /// <summary>
        /// Splits the audio media data at a given split point in time,
        /// <c>this</c> retaining the audio before the split point,
        /// creating a new <see cref="AudioMediaData"/> containing the audio after the split point
        /// </summary>
        /// <param name="splitPoint">The given split point</param>
        /// <returns>A audio media data containing the audio after the split point</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the given split point is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given split point is negative or is beyond the duration of <c>this</c>
        /// </exception>
        public virtual AudioMediaData Split(Time splitPoint)
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
                    "The split point can not be beyond the end of the AudioMediaData");
            }
            MediaData md = GetMediaDataFactory().Create(GetType());
            if (!(md is AudioMediaData))
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaDataFactory can not create a AudioMediaData matching Xuk QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            AudioMediaData secondPartAMD = (AudioMediaData) md;
            TimeDelta spDur = Time.Zero.AddTimeDelta(AudioDuration).GetTimeDelta(splitPoint);
            Stream secondPartAudioStream = GetAudioData(splitPoint);
            try
            {
                secondPartAMD.AppendAudioData(secondPartAudioStream, spDur);
            }
            finally
            {
                secondPartAudioStream.Close();
            }
            RemoveAudioData(splitPoint);
            return secondPartAMD;
        }

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
        public virtual void MergeWith(AudioMediaData other)
        {
            if (other == null)
            {
                throw new exception.MethodParameterIsNullException("Can not merge with a null AudioMediaData");
            }
            if (other == this)
            {
                throw new exception.OperationNotValidException("Can not merge a AudioMediaData with itself");
            }
            if (!PCMFormat.IsCompatibleWith(other.PCMFormat))
            {
                throw new exception.InvalidDataFormatException(
                    "Can not merge this with a AudioMediaData with incompatible audio data");
            }
            System.IO.Stream otherData = other.GetAudioData();
            try
            {
                AppendAudioData(otherData, other.AudioDuration);
            }
            finally
            {
                otherData.Close();
            }
            other.RemoveAudioData(Time.Zero);
        }

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>		
        public override bool ValueEquals(MediaData other)
        {
            if (!base.ValueEquals(other))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            AudioMediaData amdOther = (AudioMediaData) other;
            if (!PCMFormat.ValueEquals(amdOther.PCMFormat))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (GetPCMLength() != amdOther.GetPCMLength())
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            Stream thisData = GetAudioData();
            try
            {
                Stream otherdata = amdOther.GetAudioData();
                try
                {
                    if (!PCMDataInfo.CompareStreamData(thisData, otherdata, (int) thisData.Length))
                    {
                        //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                        return false;
                    }
                }
                finally
                {
                    otherdata.Close();
                }
            }
            finally
            {
                thisData.Close();
            }
            return true;
        }
    }
}