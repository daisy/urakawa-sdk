using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of a <see cref="IMediaDataManager"/>
	/// </summary>
	public class MediaDataManager : IMediaDataManager
	{
		private Dictionary<string, IMediaData> mMediaDataDictionary = new Dictionary<string,IMediaData>();
		private Dictionary<IMediaData, string> mReverseLookupMediaDataDictionary = new Dictionary<IMediaData, string>();
		private System.Threading.Mutex mUidMutex;
		private string mUidPrefix = "UID";
		private ulong mUidNo = 0;

		#region IMediaDataManager Members

		private IMediaDataPresentation mPresentation;

		/// <summary>
		/// Gets the <see cref="IMediaDataPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaDataPresentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="IMediaDataPresentation"/> has been associated with <c>this</c>
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
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>).
		/// Convenience for <c><see cref="getPresentation"/>().<see cref="IMediaDataPresentation.getMediaDataFactory"/>()</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataFactory"/></returns>
		public IMediaDataFactory getMediaDataFactory()
		{
			return getPresentation().getMediaDataFactory();
		}


		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>).
		/// Convenience for <c><see cref="getPresentation"/>().<see cref="IMediaDataPresentation.getDataProviderFactory"/>()</c>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		public IDataProviderFactory getDataProviderFactory()
		{
			return getPresentation().getDataProviderFactory();
		}

		/// <summary>
		/// Gets the <see cref="IMediaData"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The <see cref="IMediaData"/> with the given UID or <c>null</c> if no such <see cref="IMediaData"/> exists</returns>
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
				mMediaDataDictionary.Add(uid, data);
				mReverseLookupMediaDataDictionary.Add(data, uid);
			}
			finally
			{
				mUidMutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Deletes a <see cref="IMediaData"/>
		/// </summary>
		/// <param name="data">The <see cref="IMediaData"/> to delete</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		public void deleteMediaData(IMediaData data)
		{
			string uid = getUidOfMediaData(data);
			deleteMediaData(uid);
		}

		/// <summary>
		/// Deletes a <see cref="IMediaData"/>
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
			mUidMutex.WaitOne();
			try
			{
				IMediaData data = getMediaData(uid);
				if (data == null)
				{
					throw new exception.IsNotManagerOfException(
						String.Format("The MediaDataManager does not manage a MediaData with uid {0}", uid));
				}
				mMediaDataDictionary.Remove(uid);
				mReverseLookupMediaDataDictionary.Remove(data);
			}
			finally
			{
				mUidMutex.ReleaseMutex();
			}
		}


		public IMediaData copyMediaData(IMediaData data)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IMediaData copyMediaData(string uid)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IXukAble Members

		public bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

	}
}
