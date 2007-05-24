using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of a <see cref="IMediaDataManager"/>
	/// </summary>
	public class MediaDataManager : IMediaDataManager
	{
		private const string DEFAULT_UID_PREFIX = "UID";

		private Dictionary<string, IMediaData> mMediaDataDictionary = new Dictionary<string, IMediaData>();
		private Dictionary<IMediaData, string> mReverseLookupMediaDataDictionary = new Dictionary<IMediaData, string>();
		private System.Threading.Mutex mUidMutex = new System.Threading.Mutex();
		private ulong mUidNo = 0;
		private string mUidPrefix = DEFAULT_UID_PREFIX;
		private IMediaDataFactory mFactory;

		/// <summary>
		/// Default constructor - initializes the constructed instance with a newly created <see cref="MediaDataFactory"/>
		/// </summary>
		public MediaDataManager() : this(new MediaDataFactory()) 
		{ 
		}

		/// <summary>
		/// Constructor initializing the constructed instance with a given <see cref="IMediaDataFactory"/>
		/// </summary>
		/// <param name="fact"></param>
		public MediaDataManager(IMediaDataFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The media data factory of the manager can not be null");
			}
			mFactory = fact;
			mFactory.setMediaDataManager(this);
		}

		#region IMediaDataManager Members

		private IMediaDataPresentation mPresentation;

		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has been associated with <c>this</c>
		/// </exception>
		public IMediaDataPresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException("The MediaDataManager has not been associated with a MediaDataPresentation");
			}
			return mPresentation;
		}

		/// <summary>
		/// Associates a <see cref="IMediaDataPresentation"/> with <c>this</c> - Initializer
		/// </summary>
		/// <param name="pres">The <see cref="IMediaDataPresentation"/> with which to associate <c>this</c></param>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="IMediaDataPresentation"/>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="pres"/> is <c>null</c>
		/// </exception>
		public void setPresentation(IMediaDataPresentation pres)
		{
			if (pres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The MediaDataPresentation associated with a MediaDataManager can not be null");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The MediaDataManager has already been associated with a MediaDataPresentation");
			}
			mPresentation = pres;
		}


		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/> associated with <c>this</c> 
		/// </summary>
		/// <returns>The <see cref="IMediaDataFactory"/></returns>
		public IMediaDataFactory getMediaDataFactory()
		{
			return mFactory;
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
		/// Gets the <see cref="IMediaData"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The <see cref="IMediaData"/> with the given UID 
		/// or <c>null</c> if no such <see cref="IMediaData"/> exists</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		public IMediaData getMediaData(string uid)
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
		/// Gets the UID of a given <see cref="IMediaData"/>
		/// </summary>
		/// <param name="data">The given <see cref="IMediaData"/></param>
		/// <returns>The UID of <see cref="IMediaData"/> <paramref name="data"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <c>this</c> is not the manager of <paramref name="data"/>
		/// </exception>
		public string getUidOfMediaData(IMediaData data)
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
		/// Adds a <see cref="IMediaData"/> to the <see cref="IMediaDataManager"/>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to add</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		public void addMediaData(IMediaData data)
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

		private void addMediaData(IMediaData data, string uid)
		{
			if (mMediaDataDictionary.ContainsKey(uid))
			{
				throw new exception.IsAlreadyManagerOfException(String.Format(
					"There is already another MediaData with uid {0}", uid));
			}
			mMediaDataDictionary.Add(uid, data);
			mReverseLookupMediaDataDictionary.Add(data, uid);
		}

		/// <summary>
		/// Detaches a <see cref="IMediaData"/> from <c>this</c>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to detach</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <paramref name="data"/> is not managed by <c>this</c>
		/// </exception>
		public void detachMediaData(IMediaData data)
		{
			string uid = getUidOfMediaData(data);
			detachMediaData(data, uid);
		}

		/// <summary>
		/// Deletes a <see cref="IMediaData"/>. 
		/// Convenience for <c>getMediaData(uid).delete()</c>
		/// </summary>
		/// <param name="uid">The UID of the <see cref="IMediaData"/> to delete</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when no <see cref="IMediaData"/> managed by <c>this</c> has the given UID
		/// </exception>
		public void deleteMediaData(string uid)
		{
			IMediaData data = getMediaData(uid);
			if (data == null)
			{
				throw new exception.IsNotManagerOfException(
					String.Format("The MediaDataManager does not manage a MediaData with uid {0}", uid));
			}
			data.delete();
		}

		private void detachMediaData(IMediaData data, string uid)
		{
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
		public IMediaData copyMediaData(IMediaData data)
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
		public IMediaData copyMediaData(string uid)
		{
			IMediaData data = getMediaData(uid);
			if (data == null)
			{
				throw new exception.IsNotManagerOfException(String.Format(
					"The media data manager does not manage a media data with UID {0}",
					uid));
			}
			return copyMediaData(data);
		}


		/// <summary>
		/// Gets a list of all <see cref="IMediaData"/> managed by <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		public IList<IMediaData> getListOfManagedMediaData()
		{
			return new List<IMediaData>(mMediaDataDictionary.Values);
		}

		/// <summary>
		/// Gets a list of the uids assigned to <see cref="IMediaData"/> by the manager
		/// </summary>
		/// <returns>The list of uids</returns>
		public IList<string> getListOfUids()
		{
			return new List<string>(mMediaDataDictionary.Keys);
		}

		#endregion

		#region IXukAble Members

		
		/// <summary>
		/// Reads the <see cref="MediaDataManager"/> from a MediaDataManager xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
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
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads the attributes of a MediaDataManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			return true;
		}

		/// <summary>
		/// Reads a child of a MediaDataManager xuk element. 
		/// More specifically the <see cref="MediaData"/> managed by <c>this</c>
		/// is read from the mMediaData child.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mMediaData":
						if (!XukInMediaData(source)) return false;
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem ||source.IsEmptyElement))
			{
				source.ReadSubtree().Close();
			}
			return true;
		}

		private bool XukInMediaData(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mMediaDataItem" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							if (!XukInMediaDataItem(source)) return false;
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
					if (source.EOF) break;
				}
			}
			return true;
		}

		private bool XukInMediaDataItem(XmlReader source)
		{
			string uid = source.GetAttribute("uid");
			IMediaData data = getMediaDataFactory().createMediaData(source.LocalName, source.NamespaceURI);
			if (data != null)
			{
				if (uid == null && uid == "") return false;
				if (getMediaData(uid)!=null) return false;
				if (!data.XukIn(source)) return false;
				addMediaData(data, uid);
			}
			return true;
		}

		
		/// <summary>
		/// Write a MediaDataManager element to a XUK file representing the <see cref="MediaDataManager"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="destination"/> is <c>null</c></exception>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("The destination XmlWriter is null");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a MediaDataManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a MediaDataManager element.
		/// Mode specifically the <see cref="MediaData"/> of <c>this</c> is written to a mMediaData element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mMediaData", ToolkitSettings.XUK_NS);
			foreach (string uid in mMediaDataDictionary.Keys)
			{
				destination.WriteStartElement("mMediaDataItem", ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("uid", uid);
				mMediaDataDictionary[uid].XukOut(destination);
				destination.WriteEndElement();
			}
			destination.WriteEndElement();
			return true;
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaDataManager"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaDataManager"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMediaDataManager> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(IMediaDataManager other)
		{
			if (other==null) return false;
			IList<IMediaData> otherMediaData = other.getListOfManagedMediaData();
			if (mMediaDataDictionary.Count != otherMediaData.Count) return false;
			foreach (IMediaData oMD in otherMediaData)
			{
				if (!oMD.ValueEquals(getMediaData(other.getUidOfMediaData(oMD)))) return false;
			}
			return false;
		}

		#endregion

	}
}
