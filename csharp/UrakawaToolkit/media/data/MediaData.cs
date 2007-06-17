using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace urakawa.media.data
{
	/// <summary>
	/// Abstract implementation of <see cref="MediaData"/> that provides the common functionality 
	/// needed by any implementation of <see cref="MediaData"/>
	/// </summary>
	public abstract class MediaData : xuk.IXukAble, IValueEquatable<MediaData>
	{

		#region MediaData Members

		private MediaDataManager mManager;

		/// <summary>
		/// Gets the <see cref="MediaDataManager"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The assicoated <see cref="MediaDataManager"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when <c>this</c> has not been associated with a <see cref="MediaDataManager"/>
		/// </exception>
		public MediaDataManager getMediaDataManager()
		{
			if (mManager == null)
			{
				throw new exception.IsNotInitializedException("The MediaData has not been initialized with a MediaDataManager");
			}
			return mManager;
		}

		/// <summary>
		/// Associates <c>this</c> with a <see cref="MediaDataManager"/> - 
		/// initializer that is called in method <see cref="MediaDataManager.addMediaData"/> method. 
		/// Calling the initializer elsewhere may corrupt the data model.
		/// </summary>
		/// <param name="mngr">The <see cref="MediaDataManager"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="mngr"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="MediaDataManager"/>
		/// </exception>
		public void setMediaDataManager(MediaDataManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException("The MediaDataManager of a MediaData can not be null");
			}
			if (mManager != null)
			{
				throw new exception.IsAlreadyInitializedException("The MediaData has already been intialized with a MediaDataManager");
			}
			mManager = mngr;
			mManager.addMediaData(this);
		}

		/// <summary>
		/// Gets the UID of <c>this</c>.
		/// Convenience for <c><see cref="getMediaDataManager"/>().<see cref="MediaDataManager.getUidOfMediaData"/>(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		public string getUid()
		{
			return getMediaDataManager().getUidOfMediaData(this);
		}

		private string mName = "";

		/// <summary>
		/// Gets the name of <c>this</c>
		/// </summary>
		/// <returns>The name</returns>
		public string getName()
		{
			return mName;
		}

		/// <summary>
		/// Sets the name of <c>this</c>
		/// </summary>
		/// <param name="newName">The new name</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new name is <c>null</c></exception>
		public void setName(string newName)
		{
			if (newName == null)
			{
				throw new exception.MethodParameterIsNullException("The name of an MediaData can not be null");
			}
			mName = newName;
		}

		/// <summary>
		/// Gets a <see cref="List{IDataProvider}"/> of the <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The <see cref="List{IDataProvider}"/></returns>
		public abstract List<IDataProvider> getListOfUsedDataProviders();

		/// <summary>
		/// Deletes the <see cref="MediaData"/>, detaching it from it's manager and releasing 
		/// any <see cref="IDataProvider"/>s used
		/// </summary>
		public virtual void delete()
		{
			getMediaDataManager().detachMediaData(this);
		}

		/// <summary>
		/// Part of technical solution to make copy method return correct type. 
		/// In implementing classes this method should return a copy of the class instances
		/// </summary>
		/// <returns>The copy</returns>
		protected abstract MediaData mediaDataCopy();

		/// <summary>
		/// Creates a copy of the media data
		/// </summary>
		/// <returns>The copy</returns>
		public MediaData copy()
		{
			return mediaDataCopy();
		}

		#endregion

		#region IXukAble Members

		
		/// <summary>
		/// Reads the <see cref="MediaData"/> from a xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public abstract void XukIn(XmlReader source);


		/// <summary>
		/// Write a element to a XUK file representing the <see cref="MediaData"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		public abstract void XukOut(XmlWriter destination);
		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaData"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaData"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}


		#endregion

		#region IValueEquatable<MediaData> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public abstract bool ValueEquals(MediaData other);

		#endregion
	}
}
