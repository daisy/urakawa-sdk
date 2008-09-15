using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// Default implementation of a <see cref="MediaDataManager"/>
    /// </summary>
    public class MediaDataManager : WithPresentation, IXukAble, IValueEquatable<MediaDataManager>
    {
        private const string DEFAULT_UID_PREFIX = "UID";

        private Dictionary<string, MediaData> mMediaDataDictionary = new Dictionary<string, MediaData>();
        private Dictionary<MediaData, string> mReverseLookupMediaDataDictionary = new Dictionary<MediaData, string>();
        private System.Threading.Mutex mUidMutex = new System.Threading.Mutex();
        private ulong mUidNo = 0;
        private string mUidPrefix = DEFAULT_UID_PREFIX;
        private audio.PCMFormatInfo mDefaultPCMFormat;
        private bool mEnforceSinglePCMFormat;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal MediaDataManager()
        {
            mDefaultPCMFormat = new audio.PCMFormatInfo();
            mEnforceSinglePCMFormat = false;
        }

        private bool IsNewDefaultPCMFormatOk(audio.PCMFormatInfo newDefault)
        {
            foreach (MediaData md in ListOfMediaData)
            {
                if (md is audio.AudioMediaData)
                {
                    audio.AudioMediaData amd = (audio.AudioMediaData) md;
                    if (!amd.PCMFormat.ValueEquals(newDefault)) return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Gets the <see cref="MediaDataFactory"/> associated with <c>this</c> 
        /// </summary>
        /// <returns>The <see cref="MediaDataFactory"/></returns>
        public MediaDataFactory MediaDataFactory
        {
            get { return Presentation.MediaDataFactory; }
        }


        /// <summary>
        /// Gets the <see cref="DataProviderFactory"/> associated with <c>this</c> 
        /// (via. the <see cref="Presentation"/> associated with <c>this</c>).
        /// Convenience for <c>getDataProviderManager().getDataProviderFactory()</c>
        /// </summary>
        /// <returns>The <see cref="DataProviderFactory"/></returns>
        public DataProviderFactory DataProviderFactory
        {
            get { return Presentation.DataProviderManager.DataProviderFactory; }
        }

        /// <summary>
        /// Gets (copy of) the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager 
        /// </summary>
        /// <returns>The default PCM format</returns>
        public PCMFormatInfo DefaultPCMFormat
        {
            get { return mDefaultPCMFormat.Copy(); }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The default PCM format of the manager can not be null");
                }
                if (!value.ValueEquals(mDefaultPCMFormat))
                {
                    if (EnforceSinglePCMFormat)
                    {
                        if (!IsNewDefaultPCMFormatOk(value))
                        {
                            throw new exception.InvalidDataFormatException(
                                "Cannot change the default PCMFormat, since single PCM format is enforced by the DataProviderManager "
                                + "and since at least one AudioMediaData is currently managed");
                        }
                    }
                    mDefaultPCMFormat = value.Copy();
                }
            }
        }

        /// <summary>
        /// Sets the number of channels of the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
        /// </summary>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new value is less than <c>1</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the manager is enforcing single PCM format and a managed <see cref="audio.AudioMediaData"/> has a different number of channels
        /// </exception>
        public ushort DefaultNumberOfChannels
        {
            set
            {
                audio.PCMFormatInfo newFormat = DefaultPCMFormat;
                newFormat.NumberOfChannels = value;
                DefaultPCMFormat = newFormat;
            }
        }

        /// <summary>
        /// Sets the sample rate of the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
        /// </summary>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the manager is enforcing single PCM format and a managed <see cref="audio.AudioMediaData"/> has a different sample rate
        /// </exception>
        public uint DefaultSampleRate
        {
            set
            {
                audio.PCMFormatInfo newFormat = DefaultPCMFormat;
                newFormat.SampleRate = value;
                DefaultPCMFormat = newFormat;
            }
        }

        /// <summary>
        /// Sets the number of channels of the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
        /// </summary>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new value is less than <c>1</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the manager is enforcing single PCM format and a managed <see cref="audio.AudioMediaData"/> has a different bit depth
        /// </exception>
        public ushort DefaultBitDepth
        {
            set
            {
                audio.PCMFormatInfo newFormat = DefaultPCMFormat;
                newFormat.BitDepth = value;
                DefaultPCMFormat = newFormat;
            }
        }

        /// <summary>
        /// Sets the default PCM format by number of channels, sample rate and bit depth
        /// </summary>
        /// <param name="numberOfChannels">The number of channels</param>
        /// <param name="sampleRate">The sample rate</param>
        /// <param name="bitDepth">The bit depth</param>
        public void SetDefaultPCMFormat(ushort numberOfChannels, uint sampleRate, ushort bitDepth)
        {
            audio.PCMFormatInfo newDefault = new urakawa.media.data.audio.PCMFormatInfo();
            newDefault.NumberOfChannels = numberOfChannels;
            newDefault.SampleRate = sampleRate;
            newDefault.BitDepth = bitDepth;
            DefaultPCMFormat = newDefault;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if a single 
        /// PCMFormat is enforced for all managed <see cref="audio.AudioMediaData"/>
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool EnforceSinglePCMFormat
        {
            get { return mEnforceSinglePCMFormat; }
            set
            {
                if (value)
                {
                    if (!IsNewDefaultPCMFormatOk(DefaultPCMFormat))
                    {
                        throw new exception.InvalidDataFormatException(
                            "Cannot enforce single PCM format, since at least one of the managed AudioMediaData "
                            + "has a PCMFormat that is different from the manager default");
                    }
                }
                mEnforceSinglePCMFormat = value;
            }
        }


        /// <summary>
        /// Gets the <see cref="MediaData"/> with a given UID
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <returns>The <see cref="MediaData"/> with the given UID 
        /// or <c>null</c> if no such <see cref="MediaData"/> exists</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="uid"/> is <c>null</c>
        /// </exception>
        public MediaData GetMediaData(string uid)
        {
            if (uid == null)
            {
                throw new exception.MethodParameterIsNullException("The UID must not be null");
            }
            if (mMediaDataDictionary.ContainsKey(uid))
            {
                return mMediaDataDictionary[uid];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the UID of a given <see cref="MediaData"/>
        /// </summary>
        /// <param name="data">The given <see cref="MediaData"/></param>
        /// <returns>The UID of <see cref="MediaData"/> <paramref name="data"/></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <c>this</c> is not the manager of <paramref name="data"/>
        /// </exception>
        public string GetUidOfMediaData(MediaData data)
        {
            if (data == null)
            {
                throw new exception.MethodParameterIsNullException("Can not get the UID of a null AudioMediaData");
            }
            if (!mReverseLookupMediaDataDictionary.ContainsKey(data))
            {
                throw new exception.IsNotManagerOfException(
                    "The given AudioMediaData is not managed by this MediaDataManager");
            }
            return mReverseLookupMediaDataDictionary[data];
        }

        private string GetNewUid()
        {
            while (true)
            {
                if (mUidNo < UInt64.MaxValue)
                {
                    mUidNo++;
                }
                else
                {
                    mUidPrefix += "X";
                }
                string key = String.Format("{0}{1:00000000}", mUidPrefix, mUidNo);
                if (!mMediaDataDictionary.ContainsKey(key))
                {
                    return key;
                }
            }
        }

        /// <summary>
        /// Adds a <see cref="MediaData"/> to the <see cref="MediaDataManager"/>
        /// </summary>
        /// <param name="data">The <see cref="MediaData"/> to add</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        public void AddMediaData(MediaData data)
        {
            if (data == null)
            {
                throw new exception.MethodParameterIsNullException("Can not add null AudioMediaData to the manager");
            }
            mUidMutex.WaitOne();
            try
            {
                string uid = GetNewUid();
                AddMediaData(data, uid);
            }
            finally
            {
                mUidMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Adds a <see cref="MediaData"/> to the <see cref="MediaDataManager"/>, assigning it a given uid
        /// </summary>
        /// <param name="data">The <see cref="MediaData"/> to add</param>
        /// <param name="uid">The uid to assign to the added <see cref="MediaData"/></param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsAlreadyManagerOfException">
        /// Thrown when another <see cref="MediaData"/> has the same uid
        /// </exception>
        protected void AddMediaData(MediaData data, string uid)
        {
            if (mMediaDataDictionary.ContainsKey(uid))
            {
                throw new exception.IsAlreadyManagerOfException(String.Format(
                                                                    "There is already another AudioMediaData with uid {0}",
                                                                    uid));
            }
            if (EnforceSinglePCMFormat)
            {
                if (data is audio.AudioMediaData)
                {
                    audio.AudioMediaData amdata = (audio.AudioMediaData) data;
                    if (!amdata.PCMFormat.ValueEquals(DefaultPCMFormat))
                    {
                        throw new exception.InvalidDataFormatException(
                            "The AudioMediaData being added has a PCM format that is in-compatible with the manager (breaks enforcing of single PCM format)");
                    }
                }
            }
            mMediaDataDictionary.Add(uid, data);
            mReverseLookupMediaDataDictionary.Add(data, uid);
        }

        /// <summary>
        /// Sets the uid of a given managed <see cref="MediaData"/> to a given value
        /// </summary>
        /// <param name="data">The given <see cref="MediaData"/></param>
        /// <param name="uid">The given uid value</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> or <paramref name="uid"/> is <c>null</c> 
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when the manager instance does not manage <paramref name="data"/>
        /// </exception>
        /// <exception cref="exception.IsAlreadyManagerOfException">
        /// Thrown when <paramref name="uid"/> is already the uid of another <see cref="MediaData"/>
        /// </exception>
        public void SetDataMediaDataUid(MediaData data, string uid)
        {
            RemoveMediaData(data);
            AddMediaData(data, uid);
        }

        /// <summary>
        /// Determines if the manager manages a <see cref="MediaData"/> with a given uid
        /// </summary>
        /// <param name="uid">The given uid</param>
        /// <returns>
        /// A <see cref="bool"/> indicating if the manager manages a <see cref="MediaData"/> with the given uid
        /// </returns>
        public bool IsManagerOf(string uid)
        {
            return mMediaDataDictionary.ContainsKey(uid);
        }

        /// <summary>
        /// Removes a <see cref="MediaData"/> from <c>this</c>
        /// </summary>
        /// <param name="data">The <see cref="MediaData"/> to remove</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <paramref name="data"/> is not managed by <c>this</c>
        /// </exception>
        public void RemoveMediaData(MediaData data)
        {
            RemoveMediaData(GetUidOfMediaData(data));
        }

        /// <summary>
        /// Removes a <see cref="MediaData"/> from <c>this</c>
        /// </summary>
        /// <param name="uid">The uid of the <see cref="MediaData"/> to remove</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="uid"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when no managed <see cref="MediaData"/> has the given uid
        /// </exception>
        public void RemoveMediaData(string uid)
        {
            MediaData data = GetMediaData(uid);
            mUidMutex.WaitOne();
            try
            {
                mMediaDataDictionary.Remove(uid);
                mReverseLookupMediaDataDictionary.Remove(data);
            }
            finally
            {
                mUidMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Creates a copy of a given media data
        /// </summary>
        /// <param name="data">The media data to copy</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <paramref name="data"/> is not managed by <c>this</c>
        /// </exception>
        public MediaData CopyMediaData(MediaData data)
        {
            if (data == null)
            {
                throw new exception.MethodParameterIsNullException("Can not copy a null AudioMediaData");
            }
            if (data.MediaDataManager != this)
            {
                throw new exception.IsNotManagerOfException(
                    "Can not copy a AudioMediaData that is not managed by this");
            }
            return data.Copy();
        }

        /// <summary>
        /// Creates a copy of the media data with a given UID
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <c>this</c> does not manage a media data with the given UID
        /// </exception>
        public MediaData CopyMediaData(string uid)
        {
            MediaData data = GetMediaData(uid);
            if (data == null)
            {
                throw new exception.IsNotManagerOfException(String.Format(
                                                                "The media data manager does not manage a media data with UID {0}",
                                                                uid));
            }
            return CopyMediaData(data);
        }

        /// <summary>
        /// Gets a list of all <see cref="MediaData"/> managed by <c>this</c>
        /// </summary>
        /// <returns>The list</returns>
        public List<MediaData> ListOfMediaData
        {
            get { return new List<MediaData>(mMediaDataDictionary.Values); }
        }

        /// <summary>
        /// Gets a list of the uids assigned to <see cref="MediaData"/> by the manager
        /// </summary>
        /// <returns>The list of uids</returns>
        public List<string> ListOfUids
        {
            get { return new List<string>(mMediaDataDictionary.Keys); }
        }

        #region IXukAble Members

        /// <summary>
        /// Clears the <see cref="MediaDataManager"/> disassociating any linked <see cref="MediaData"/>
        /// </summary>
        protected override void Clear()
        {
            mUidMutex.WaitOne();
            try
            {
                mMediaDataDictionary.Clear();
                mReverseLookupMediaDataDictionary.Clear();
            }
            finally
            {
                mUidMutex.ReleaseMutex();
            }
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a MediaDataManager xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string attr = source.GetAttribute("enforceSinglePCMFormat");
            if (attr == "true" || attr == "1")
            {
                EnforceSinglePCMFormat = true;
            }
            else
            {
                EnforceSinglePCMFormat = false;
            }
        }

        /// <summary>
        /// Reads a child of a MediaDataManager xuk element. 
        /// More specifically the <see cref="MediaData"/> managed by <c>this</c>
        /// is read from the mMediaData child.
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
                    case "mDefaultPCMFormat":
                        ;
                        XukInDefaultPCMFormat(source, handler);
                        break;
                    case "mMediaData":
                        XukInMediaData(source, handler);
                        break;
                    default:
                        readItem = false;
                        break;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close();
            }
        }

        private void XukInDefaultPCMFormat(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == "PCMFormatInfo" && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            audio.PCMFormatInfo newInfo = new urakawa.media.data.audio.PCMFormatInfo();
                            newInfo.XukIn(source, handler);
                            bool enf = EnforceSinglePCMFormat;
                            if (enf) EnforceSinglePCMFormat = false;
                            DefaultPCMFormat = newInfo;
                            if (enf) EnforceSinglePCMFormat = true;
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

        private void XukInMediaData(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == "mMediaDataItem" && source.NamespaceURI == XukAble.XUK_NS)
                        {
                            XukInMediaDataItem(source, handler);
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

        private void XukInMediaDataItem(XmlReader source, ProgressHandler handler)
        {
            string uid = source.GetAttribute("uid");
            MediaData data = null;
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        data = MediaDataFactory.Create(source.LocalName, source.NamespaceURI);
                        if (data != null)
                        {
                            data.XukIn(source, handler);
                        }
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
            if (data != null)
            {
                if (uid == null || uid == "")
                {
                    throw new exception.XukException(
                        "uid attribute is missing from mMediaDataItem attribute");
                }
                SetDataMediaDataUid(data, uid);
            }
        }

        /// <summary>
        /// Writes the attributes of a MediaDataManager element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("enforceSinglePCMFormat", EnforceSinglePCMFormat ? "true" : "false");
            base.XukOutAttributes(destination, baseUri);
        }

        /// <summary>
        /// Write the child elements of a MediaDataManager element.
        /// Mode specifically the <see cref="MediaData"/> of <c>this</c> is written to a mMediaData element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            destination.WriteStartElement("mDefaultPCMFormat", XukAble.XUK_NS);
            DefaultPCMFormat.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();
            destination.WriteStartElement("mMediaData", XukAble.XUK_NS);
            foreach (string uid in mMediaDataDictionary.Keys)
            {
                destination.WriteStartElement("mMediaDataItem", XukAble.XUK_NS);
                destination.WriteAttributeString("uid", uid);
                mMediaDataDictionary[uid].XukOut(destination, baseUri, handler);
                destination.WriteEndElement();
            }
            destination.WriteEndElement();
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<MediaDataManager> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public bool ValueEquals(MediaDataManager other)
        {
            if (other == null) return false;
            List<MediaData> otherMediaData = other.ListOfMediaData;
            if (mMediaDataDictionary.Count != otherMediaData.Count) return false;
            foreach (MediaData oMD in otherMediaData)
            {
                if (!oMD.ValueEquals(GetMediaData(other.GetUidOfMediaData(oMD)))) return false;
            }
            return true;
        }

        #endregion
    }
}