using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using AudioLib;
using urakawa.data;
using urakawa.exception;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.media.data.audio
{
    /// <summary>
    /// Abstract base class for audio <see cref="MediaData"/>.
    /// Implements PCM format accessors (number of channels, bit depth, sample rate) 
    /// and leaves all other methods abstract
    /// </summary>
    public abstract class AudioMediaData : MediaData
    {
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (!String.IsNullOrEmpty(OriginalRelativePath))
            {
                destination.WriteAttributeString(XukStrings.OriginalRelativePath, OriginalRelativePath);
            }

            if (DataProvider != null && !String.IsNullOrEmpty(DataProvider.Uid))
            {
                destination.WriteAttributeString(DataProvider.DataProvider_NAME.z(PrettyFormat), DataProvider.Uid);
            }

            //if (!Presentation.Project.PrettyFormat)
            //{
            //    //destination.WriteAttributeString(XukStrings.Uid, Uid);
            //}
        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string path = source.GetAttribute(XukStrings.OriginalRelativePath);
            string uid = XukAble.ReadXukAttribute(source, DataProvider.DataProvider_NAME);

            if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(uid))
            {
                DataProvider prov = Presentation.DataProviderManager.GetManagedObject(uid);

                InitializeAudio(prov, path);
            }

            //if (!Presentation.DataProviderManager.IsManagerOf(uid))
            //{
            //    throw new IsNotManagerOfException(
            //            String.Format("DataProvider cannot be found {0}", uid));
            //}
        }

        public void InitializeAudio(string path, string originalRelativePath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new MethodParameterIsNullException("The path of a audio can not be null");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found at " + path);
            }

            DataProvider imgDataProvider = Presentation.DataProviderFactory.Create(MimeType);
            ((FileDataProvider)imgDataProvider).InitByCopyingExistingFile(path);

            InitializeAudio(imgDataProvider, originalRelativePath);
        }

        public void InitializeAudio(DataProvider dataProv, string imgOriginalName)
        {
            if (dataProv == null)
            {
                throw new MethodParameterIsNullException("The data provider of a audio can not be null");
            }

            if (dataProv.MimeType != MimeType)
            {
                throw new OperationNotValidException("The mime type of the given DataProvider is not correct !");
            }

            if (string.IsNullOrEmpty(imgOriginalName))
            {
                throw new MethodParameterIsEmptyStringException("original name of audio cannot be null!");
            }

            DataProvider = dataProv;
            OriginalRelativePath = imgOriginalName;
        }

        public Stream OpenInputStream()
        {
            if (m_DataProvider != null)
            {
                return m_DataProvider.OpenInputStream();
            }

            return null;
        }

        public abstract string MimeType { get; }

        private DataProvider m_DataProvider;
        public DataProvider DataProvider
        {
            get
            {
                return m_DataProvider;
            }
            private set
            {
                m_DataProvider = value;
            }
        }

        public override IEnumerable<DataProvider> UsedDataProviders
        {
            get
            {
                yield return m_DataProvider;
                yield break;
            }
        }

        private string m_OriginalRelativePath;
        public string OriginalRelativePath
        {
            get
            {
                return m_OriginalRelativePath;
            }
            private set
            {
                m_OriginalRelativePath = value;
                if (m_OriginalRelativePath != null)
                {
                    m_OriginalRelativePath = m_OriginalRelativePath.Replace('\\', '/');
                }
            }
        }

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
        protected void NotifyAudioDataInserted(AudioMediaData source, Time insertPoint, Time duration)
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
        protected void NotifyAudioDataRemoved(AudioMediaData source, Time fromPoint, Time duration)
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

        private PCMFormatInfo mPCMFormat = null;

        /// <summary>
        /// Determines if a PCM Format change is ok
        /// </summary>
        /// <param name="newFormat">The new PCM Format value - assumed not to be <c>null</c></param>
        /// <param name="failReason">The <see cref="string"/> to which a failure reason must be written in case the change is not ok</param>
        /// <returns>A <see cref="bool"/> indicating if the change is ok</returns>
        protected virtual bool IsPCMFormatChangeOk(PCMFormatInfo newFormat, out string failReason)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            failReason = "";

            if (Presentation.MediaDataManager.EnforceSinglePCMFormat
                && !Presentation.MediaDataManager.DefaultPCMFormat.Data.IsCompatibleWith(newFormat.Data))
            {
                failReason =
                    "When the MediaDataManager enforces a single PCM Format, "
                    + "the PCM Format of the AudioMediaData must match the default defined by the manager";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the <see cref="PCMFormatInfo"/> of the audio media data 
        /// </summary>
        /// <returns>The PCMFormatInfo</returns>
        public PCMFormatInfo PCMFormat
        {
            get
            {
                if (OriginalRelativePath != null && DataProvider != null)
                {
                    throw new NotImplementedException();
                }

                if (mPCMFormat == null)
                {
                    mPCMFormat = Presentation.MediaDataManager.DefaultPCMFormat;
                }
                return mPCMFormat;
            }
            set
            {
                if (OriginalRelativePath != null && DataProvider != null)
                {
                    throw new NotImplementedException();
                }

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
        /// Gets the intrinsic duration of the audio data
        /// </summary>
        /// <returns>The duration as an <see cref="Time"/></returns>
        public abstract Time AudioDuration { get; }


        /// <summary>
        /// Gets an input <see cref="Stream"/> giving access to all audio data as raw PCM
        /// </summary>
        /// <returns>The input <see cref="Stream"/></returns>
        public Stream OpenPcmInputStream()
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            return OpenPcmInputStream(Time.Zero);
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
        public Stream OpenPcmInputStream(Time clipBegin)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            return OpenPcmInputStream(clipBegin, AudioDuration);
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
        public abstract Stream OpenPcmInputStream(Time clipBegin, Time clipEnd);

        /// <summary>
        /// Returns true if the actual underlying data content is not empty.
        /// </summary>
        /// <returns></returns>
        public abstract bool HasActualPcmData { get; }

        /// <summary>
        /// Appends audio of a given duration to <c>this</c>
        /// </summary>
        /// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
        /// <param name="duration">The duration of the audio to add</param>
        public virtual void AppendPcmData(Stream pcmData, Time duration)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            InsertPcmData(pcmData, AudioDuration, duration);
        }

        public abstract void AppendPcmData(DataProvider fileDataProvider);
        public abstract void AppendPcmData(DataProvider fileDataProvider, Time clipBegin, Time clipEnd);

        /// <summary>
        /// Appends audio data from a RIFF Wave file
        /// </summary>
        /// <param name="riffWaveStream">The RIFF Wave file</param>
        public void AppendPcmData_RiffHeader(Stream riffWaveStream)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            uint dataLength;
            AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(riffWaveStream, out dataLength);

            if (dataLength <= 0)
            {
                dataLength = (uint)(riffWaveStream.Length - riffWaveStream.Position);
            }

            if (!format.IsCompatibleWith(PCMFormat.Data))
            {
                throw new exception.InvalidDataFormatException(
                    String.Format("RIFF WAV file has incompatible PCM format"));
            }

            AppendPcmData(riffWaveStream, new Time(format.ConvertBytesToTime(dataLength)));
        }

        /// <summary>
        /// Appends audio data from a RIFF Wave file
        /// </summary>
        /// <param name="path">The path of the RIFF Wave file</param>
        public void AppendPcmData_RiffHeader(string path)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            Stream rwFS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                AppendPcmData_RiffHeader(rwFS);
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
        public abstract void InsertPcmData(Stream pcmData, Time insertPoint, Time duration);

        /// <summary>
        /// Inserts audio data from a RIFF Wave file at a given insert point and of a given duration
        /// </summary>
        /// <param name="riffWaveStream">The RIFF Wave file</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration - if <c>null</c> the entire RIFF Wave file is inserted</param>
        public void InsertPcmData_RiffHeader(Stream riffWaveStream, Time insertPoint, Time duration)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }


            uint dataLength;
            AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(riffWaveStream, out dataLength);

            if (!format.IsCompatibleWith(PCMFormat.Data))
            {
                throw new exception.InvalidDataFormatException(
                    String.Format("RIFF WAV file has incompatible PCM format"));
            }

            Time fileDuration = new Time(format.ConvertBytesToTime(dataLength));

            if (duration == null) duration = fileDuration;

            if (fileDuration.IsLessThan(duration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Can not insert {0} of audio from RIFF Wave file since the file's duration is only {1}",
                                                                              duration, fileDuration));
            }
            InsertPcmData(riffWaveStream, insertPoint, duration);
        }

        /// <summary>
        /// Inserts audio data from a RIFF Wave file at a given insert point and of a given duration
        /// </summary>
        /// <param name="path">The path of the RIFF Wave file</param>
        /// <param name="insertPoint">The insert point</param>
        /// <param name="duration">The duration - if <c>null</c> the entire RIFF Wave file is inserted</param>
        public void InsertPcmData_RiffHeader(string path, Time insertPoint, Time duration)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            Stream rwFS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                InsertPcmData_RiffHeader(rwFS, insertPoint, duration);
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
        public void ReplacePcmData(Stream pcmData, Time replacePoint, Time duration)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            RemovePcmData(replacePoint, new Time(replacePoint.AsTimeSpanTicks + duration.AsTimeSpanTicks, true));
            InsertPcmData(pcmData, replacePoint, duration);
        }

        /// <summary>
        /// Replaces with audio from a RIFF Wave file of a given duration at a given replace point
        /// </summary>
        /// <param name="riffWaveStream">The RIFF Wave file</param>
        /// <param name="replacePoint">The given replace point</param>
        /// <param name="duration">The duration of the audio to replace</param>
        public void ReplacePcmData_RiffHeader(Stream riffWaveStream, Time replacePoint, Time duration)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            uint dataLength;
            AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(riffWaveStream, out dataLength);

            if (!format.IsCompatibleWith(PCMFormat.Data))
            {
                throw new exception.InvalidDataFormatException(
                    String.Format("RIFF WAV file has incompatible PCM format"));
            }

            Time fileDuration = new Time(format.ConvertBytesToTime(dataLength));

            if (fileDuration.IsLessThan(duration))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "Can not insert {0} of audio from RIFF Wave file since the file's duration is only {1}",
                                                                              duration, fileDuration));
            }
            ReplacePcmData(riffWaveStream, replacePoint, duration);
        }

        /// <summary>
        /// Replaces with audio from a RIFF Wave file of a given duration at a given replace point
        /// </summary>
        /// <param name="path">The path of the RIFF Wave file</param>
        /// <param name="replacePoint">The given replace point</param>
        /// <param name="duration">The duration of the audio to replace</param>
        public void ReplacePcmData_RiffHeader(string path, Time replacePoint, Time duration)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            Stream rwFS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                ReplacePcmData_RiffHeader(rwFS, replacePoint, duration);
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
        public virtual void RemovePcmData(Time clipBegin)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            RemovePcmData(clipBegin, AudioDuration);
        }

        /// <summary>
        /// Removes all audio between given clip begin and end <see cref="Time"/>
        /// </summary>
        /// <param name="clipBegin">The givne clip begin <see cref="Time"/></param>
        /// <param name="clipEnd">The givne clip end <see cref="Time"/></param>
        public abstract void RemovePcmData(Time clipBegin, Time clipEnd);

        ///// <summary>
        ///// Part of technical solution to make copy method return correct type. 
        ///// In implementing classes this method should return a copy of the class instances
        ///// </summary>
        ///// <returns>The copy</returns>
        //protected abstract AudioMediaData AudioMediaDataCopy();


        protected override MediaData CopyProtected()
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                AudioMediaData copyVideoMediaData = (AudioMediaData)Presentation.MediaDataFactory.Create(GetType());
                // We do not Copy the FileDataProvider,
                // it is shared amongst AudioMediaData instances without concurrent access problems
                // because the file stream is read-only by design.
                copyVideoMediaData.InitializeAudio(m_DataProvider, OriginalRelativePath);
                return copyVideoMediaData;
            }
            return null;
        }

        protected override MediaData ExportProtected(Presentation destPres)
        {
            if (OriginalRelativePath != null && DataProvider != null)
            {
                AudioMediaData expImgData = (AudioMediaData)destPres.MediaDataFactory.Create(GetType());
                expImgData.InitializeAudio(m_DataProvider.Export(destPres), OriginalRelativePath);
                return expImgData;
            }
            return null;
        }

        //protected abstract override MediaData ExportProtected(Presentation destPres);
        //protected abstract override MediaData CopyProtected();

        public new AudioMediaData Copy()
        {
            return CopyProtected() as AudioMediaData;
        }

        public new AudioMediaData Export(Presentation destPres)
        {
            return ExportProtected(destPres) as AudioMediaData;
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
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            //throw new NotImplementedException("AudioMediaData.Split() should never be called ! (WavAudioMediaData instead)");

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
                    "The split point can not be beyond the end of the AudioMediaData");
            }
            MediaData md = Presentation.MediaDataFactory.Create(GetType());
            if (!(md is AudioMediaData))
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaDataFactory can not create a AudioMediaData matching Xuk QName {1}:{0}",
                                                                         GetXukName(), GetXukNamespace()));
            }
            AudioMediaData secondPartAMD = (AudioMediaData)md;
            Time spDur = AudioDuration.GetDifference(splitPoint);
            Stream secondPartAudioStream = OpenPcmInputStream(splitPoint);
            try
            {
                secondPartAMD.AppendPcmData(secondPartAudioStream, spDur);
            }
            finally
            {
                secondPartAudioStream.Close();
            }
            RemovePcmData(splitPoint);
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
            if (OriginalRelativePath != null && DataProvider != null)
            {
                throw new NotImplementedException();
            }

            //throw new NotImplementedException("AudioMediaData.MergeWith() should never be called ! (WavAudioMediaData instead)");

            if (other == null)
            {
                throw new exception.MethodParameterIsNullException("Can not merge with a null AudioMediaData");
            }
            if (other == this)
            {
                throw new exception.OperationNotValidException("Can not merge a AudioMediaData with itself");
            }
            if (!PCMFormat.Data.IsCompatibleWith(other.PCMFormat.Data))
            {
                throw new exception.InvalidDataFormatException(
                    "Can not merge this with a AudioMediaData with incompatible audio data");
            }

#if DEBUG
            Debugger.Break();
#endif // DEBUG

            Stream otherData = other.OpenPcmInputStream();
            try
            {
                AppendPcmData(otherData, other.AudioDuration);
            }
            finally
            {
                otherData.Close();
            }
            other.RemovePcmData(Time.Zero);
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            AudioMediaData otherz = other as AudioMediaData;
            if (otherz == null)
            {
                return false;
            }

            if (OriginalRelativePath != null && DataProvider != null)
            {
                if (OriginalRelativePath != otherz.OriginalRelativePath)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }

                if (!DataProvider.ValueEquals(otherz.DataProvider))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                    return false;
                }

                return true;
            }

            if (!PCMFormat.ValueEquals(otherz.PCMFormat))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            if (PCMFormat.Data.ConvertTimeToBytes(AudioDuration.AsLocalUnits)
                != otherz.PCMFormat.Data.ConvertTimeToBytes(otherz.AudioDuration.AsLocalUnits))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            if (HasActualPcmData != otherz.HasActualPcmData)
            {
                return false;
            }

            if (HasActualPcmData)
            {
                Stream thisData = OpenPcmInputStream();
                try
                {
                    Stream otherdata = otherz.OpenPcmInputStream();
                    try
                    {
                        if (!AudioLibPCMFormat.CompareStreamData(thisData, otherdata, (int)thisData.Length))
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
            }

            return true;
        }
    }
}