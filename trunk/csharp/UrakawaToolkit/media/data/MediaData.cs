using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.media.data
{
	/// <summary>
	/// Abstract implementation of <see cref="IMediaData"/> that provides the common functionality 
	/// needed by any implementation of <see cref="IMediaData"/>
	/// </summary>
	public abstract class MediaData : IMediaData
	{
		#region IMediaData Members

		private IMediaDataManager mManager;

		/// <summary>
		/// Gets the <see cref="IMediaDataManager"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The assicoated <see cref="IMediaDataManager"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when <c>this</c> has not been associated with a <see cref="IMediaDataManager"/></exception>
		public IMediaDataManager getDataManager()
		{
			if (mManager == null)
			{
				throw new exception.IsNotInitializedException("The MediaData has not been initialized with a IMediaDataManager");
			}
			return mManager;
		}

		/// <summary>
		/// Initializer - associates <c>this</c> with a <see cref="IMediaDataManager"/>
		/// </summary>
		/// <param name="mngr">The <see cref="IMediaDataManager"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="mngr"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="IMediaDataManager"/>
		/// </exception>
		protected void setDataManager(IMediaDataManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException("The IMediaDataManager of a MediaData can not be null");
			}
			if (mManager != null)
			{
				throw new exception.IsAlreadyInitializedException("The MediaData has already been intialized with a IMediaDataManager");
			}
			mManager = mngr;
		}

		/// <summary>
		/// Gets the UID of <c>this</c>.
		/// Convenience for <c><see cref="getDataManager"/>().<see cref="IMediaDataManager.getUidOfMediaData"/>(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		public string getUid()
		{
			return getDataManager().getUidOfMediaData(this);
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
		/// Gets a <see cref="IList{IDataProvider}"/> of the <see cref="IDataProvider"/>s used by <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IList{IDataProvider}"/></returns>
		protected abstract IList<IDataProvider> getUsedDataProviders();

		/// <summary>
		/// Deletes the <see cref="MediaData"/> and all <see cref="IDataProvider"/>s associated with <c>this</c>
		/// </summary>
		public virtual void delete()
		{
			foreach (IDataProvider dp in getUsedDataProviders())
			{
				dp.delete();
			}
		}

		/// <summary>
		/// Creates a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public abstract IMediaData copy();

		#endregion

		#region IXukAble Members

		
		/// <summary>
		/// Reads the <see cref="MediaData"/> from a xuk element
		/// </summary>
		/// <param localName="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public abstract bool XukIn(XmlReader source);


		/// <summary>
		/// Write a element to a XUK file representing the <see cref="MediaData"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public abstract bool XukOut(XmlWriter destination);
		
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
	}
}
