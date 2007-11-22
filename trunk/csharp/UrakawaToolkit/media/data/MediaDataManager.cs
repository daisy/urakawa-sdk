using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
		internal protected MediaDataManager()
		{
			mDefaultPCMFormat = new audio.PCMFormatInfo();
			mDefaultPCMFormat.FormatChanged += new EventHandler(DefaultPCMFormat_FormatChanged);
			mEnforceSinglePCMFormat = false;
		}

		void DefaultPCMFormat_FormatChanged(object sender, EventArgs e)
		{
			if (getEnforceSinglePCMFormat())
			{
				if (CheckSinglePCMFormatRule())
				{
					throw new exception.InvalidDataFormatException(
						"Can not change the default PCM format for the MediaDataManager, "
						+ "since the manager is enforcing single PCM format and the change will violate this rule");
				}
			}
		}

		private bool CheckSinglePCMFormatRule()
		{
			foreach (MediaData md in getListOfMediaData())
			{
				if (md is audio.AudioMediaData)
				{
					audio.AudioMediaData amd = (audio.AudioMediaData)md;
					if (!amd.getPCMFormat().valueEquals(getDefaultPCMFormat())) return false;
				}
			}
			return true;
		}


		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/> associated with <c>this</c> 
		/// </summary>
		/// <returns>The <see cref="MediaDataFactory"/></returns>
		public MediaDataFactory getMediaDataFactory()
		{
			return getPresentation().getMediaDataFactory();
		}


		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>).
		/// Convenience for <c>getDataProviderManager().getDataProviderFactory()</c>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		public IDataProviderFactory getDataProviderFactory()
		{
			return getPresentation().getDataProviderManager().getDataProviderFactory();
		}

		/// <summary>
		/// Gets the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager 
		/// by reference - changing the returned PCM format will change the default PCM format of the manager
		/// </summary>
		/// <returns>The default PCM format</returns>
		public audio.PCMFormatInfo getDefaultPCMFormat()
		{
			return mDefaultPCMFormat;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if a single 
		/// PCMFormat is enforced for all managed <see cref="audio.AudioMediaData"/>
		/// </summary>
		/// <returns>The <see cref="bool"/></returns>
		public bool getEnforceSinglePCMFormat()
		{
			return mEnforceSinglePCMFormat;
		}

		/// <summary>
		/// Sets a <see cref="bool"/> indicating if a single 
		/// PCMFormat is enforced for all managed <see cref="audio.AudioMediaData"/>
		/// </summary>
		/// <param name="newValue">The new value</param>
		public void setEnforceSinglePCMFormat(bool newValue)
		{
			mEnforceSinglePCMFormat = newValue;
			if (getEnforceSinglePCMFormat())
			{
				if (!CheckSinglePCMFormatRule())
				{
					throw new exception.InvalidDataFormatException(
						"Can not enforce single PCM formats since AudioMediaData with PCM format different "
						+ "from the default for the manager exists in the manager");
				}
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
		public MediaData getMediaData(string uid)
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
		public string getUidOfMediaData(MediaData data)
		{
			if (data == null)
			{
				throw new exception.MethodParameterIsNullException("Can not get the UID of a null MediaData");
			}
			if (!mReverseLookupMediaDataDictionary.ContainsKey(data))
			{
				throw new exception.IsNotManagerOfException("The given MediaData is not managed by this MediaDataManager");
			}
			return mReverseLookupMediaDataDictionary[data];
		}

		private string getNewUid()
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
		public void addMediaData(MediaData data)
		{
			if (data == null)
			{
				throw new exception.MethodParameterIsNullException("Can not add null MediaData to the manager");
			}
			mUidMutex.WaitOne();
			try
			{
				string uid = getNewUid();
				addMediaData(data, uid);
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
		protected void addMediaData(MediaData data, string uid)
		{
			if (mMediaDataDictionary.ContainsKey(uid))
			{
				throw new exception.IsAlreadyManagerOfException(String.Format(
					"There is already another MediaData with uid {0}", uid));
			}
			if (getEnforceSinglePCMFormat())
			{
				if (data is audio.AudioMediaData)
				{
					audio.AudioMediaData amdata = (audio.AudioMediaData)data;
					if (!amdata.getPCMFormat().valueEquals(getDefaultPCMFormat()))
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
		/// Thrown when <paramref name="data"/> or <see cref="uid"/> is null
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when the manager instance does not manage <paramref name="data"/>
		/// </exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when <paramref name="uid"/> is already the uid of another <see cref="MediaData"/>
		/// </exception>
		public void setDataMediaDataUid(MediaData data, string uid)
		{
			removeMediaData(data);
			addMediaData(data, uid);
		}

		/// <summary>
		/// Determines if the manager manages a <see cref="MediaData"/> with a given uid
		/// </summary>
		/// <param name="uid">The given uid</param>
		/// <returns>
		/// A <see cref="bool"/> indicating if the manager manages a <see cref="MediaData"/> with the given uid
		/// </returns>
		public bool isManagerOf(string uid)
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
		public void removeMediaData(MediaData data)
		{
			removeMediaData(getUidOfMediaData(data));
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
		public void removeMediaData(string uid)
		{
			MediaData data = getMediaData(uid);
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
		public MediaData copyMediaData(MediaData data)
		{
			if (data == null)
			{
				throw new exception.MethodParameterIsNullException("Can not copy a null MediaData");
			}
			if (data.getMediaDataManager() != this)
			{
				throw new exception.IsNotManagerOfException(
					"Can not copy a MediaData that is not managed by this");
			}
			return data.copy();
		}

		/// <summary>
		/// Creates a copy of the media data with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The copy</returns>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <c>this</c> does not manage a media data with the given UID
		/// </exception>
		public MediaData copyMediaData(string uid)
		{
			MediaData data = getMediaData(uid);
			if (data == null)
			{
				throw new exception.IsNotManagerOfException(String.Format(
					"The media data manager does not manage a media data with UID {0}",
					uid));
			}
			return copyMediaData(data);
		}

		/// <summary>
		/// Gets a list of all <see cref="MediaData"/> managed by <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		public List<MediaData> getListOfMediaData()
		{
			return new List<MediaData>(mMediaDataDictionary.Values);
		}

		/// <summary>
		/// Gets a list of the uids assigned to <see cref="MediaData"/> by the manager
		/// </summary>
		/// <returns>The list of uids</returns>
		public List<string> getListOfUids()
		{
			return new List<string>(mMediaDataDictionary.Keys);
		}

		/// <summary>
		/// Deletes any <see cref="MediaData"/> not assiciated with a <see cref="urakawa.core.TreeNode"/> 
		/// via. a <see cref="urakawa.property.channel.ChannelsProperty"/>
		/// </summary>
		public void deleteUnusedMediaData()
		{
			data.utilities.CollectManagedMediaTreeNodeVisitor visitor = new data.utilities.CollectManagedMediaTreeNodeVisitor();
			urakawa.core.TreeNode root = getPresentation().getRootNode();
			if (root != null)
			{
				root.acceptDepthFirst(visitor);
			}
			List<MediaData> usedMediaData = new List<MediaData>();
			foreach (IManagedMedia mm in getListOfMediaData())
			{
				if (!usedMediaData.Contains(mm.getMediaData())) usedMediaData.Add(mm.getMediaData());
			}
			foreach (MediaData md in getListOfMediaData())
			{
				if (!usedMediaData.Contains(md)) md.delete();
			}
		}

		#region IXukAble Members

		protected override void clear()
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
			base.clear();
		}

		/// <summary>
		/// Reads the attributes of a MediaDataManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string attr = source.GetAttribute("enforceSinglePCMFormat");
			if (attr == "true" || attr == "1")
			{
				setEnforceSinglePCMFormat(true);
			}
			else
			{
				setEnforceSinglePCMFormat(false);
			}
		}

		/// <summary>
		/// Reads a child of a MediaDataManager xuk element. 
		/// More specifically the <see cref="MediaData"/> managed by <c>this</c>
		/// is read from the mMediaData child.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mDefaultPCMFormat": ;
						xukInDefaultPCMFormat(source);
						break;
					case "mMediaData":
						xukInMediaData(source);
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

		private void xukInDefaultPCMFormat(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "PCMFormatInfo" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							bool enf = getEnforceSinglePCMFormat();
							if (enf) setEnforceSinglePCMFormat(false);
							getDefaultPCMFormat().xukIn(source);
							if (enf) setEnforceSinglePCMFormat(true);
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

		private void xukInMediaData(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mMediaDataItem" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							xukInMediaDataItem(source);
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

		private void xukInMediaDataItem(XmlReader source)
		{
			string uid = source.GetAttribute("uid");
			MediaData data = null;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						data = getMediaDataFactory().createMediaData(source.LocalName, source.NamespaceURI);
						if (data != null)
						{
							data.xukIn(source);
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
				if (uid == null && uid == "")
				{
					throw new exception.XukException(
						"uid attribute is missing from mMediaDataItem attribute");
				}
				setDataMediaDataUid(data, uid);
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
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("enforceSinglePCMFormat", getEnforceSinglePCMFormat()?"true":"false");
			base.xukOutAttributes(destination, baseUri);
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
		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			destination.WriteStartElement("mDefaultPCMFormat", ToolkitSettings.XUK_NS);
			getDefaultPCMFormat().xukOut(destination, baseUri);
			destination.WriteEndElement();
			destination.WriteStartElement("mMediaData", ToolkitSettings.XUK_NS);
			foreach (string uid in mMediaDataDictionary.Keys)
			{
				destination.WriteStartElement("mMediaDataItem", ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("uid", uid);
				mMediaDataDictionary[uid].xukOut(destination, baseUri);
				destination.WriteEndElement();
			}
			destination.WriteEndElement();
			base.xukOutChildren(destination, baseUri);
		}

		#endregion

		#region IValueEquatable<MediaDataManager> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool valueEquals(MediaDataManager other)
		{
			if (other == null) return false;
			List<MediaData> otherMediaData = other.getListOfMediaData();
			if (mMediaDataDictionary.Count != otherMediaData.Count) return false;
			foreach (MediaData oMD in otherMediaData)
			{
				if (!oMD.valueEquals(getMediaData(other.getUidOfMediaData(oMD)))) return false;
			}
			return true;
		}

		#endregion

	}
}
