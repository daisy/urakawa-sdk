using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using urakawa.events;

namespace urakawa.media.data
{
	/// <summary>
	/// Abstract implementation of <see cref="MediaData"/> that provides the common functionality 
	/// needed by any implementation of <see cref="MediaData"/>
	/// </summary>
	public abstract class MediaData : WithPresentation, IValueEquatable<MediaData>, IChangeNotifier
	{
		
		#region Event related members

		/// <summary>
		/// Event fired after the <see cref="MediaData"/> has changed. 
		/// The event fire before any change specific event 
		/// </summary>
		public event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;
		/// <summary>
		/// Fires the <see cref="changed"/> event 
		/// </summary>
		/// <param name="args">The arguments of the event</param>
		protected void notifyChanged(urakawa.events.DataModelChangedEventArgs args)
		{
			EventHandler<urakawa.events.DataModelChangedEventArgs> d = changed;
			if (d != null) d(this, args);
		}

		/// <summary>
		/// Event fired after the name of the <see cref="Media"/> has changed
		/// </summary>
		public event EventHandler<NameChangedEventArgs> nameChanged;
		/// <summary>
		/// Fires the <see cref="nameChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="MediaData"/> whoose name has changed</param>
		/// <param name="newName">The new name</param>
		/// <param name="prevName">The name prior to the change</param>
		protected void notifyNameChanged(MediaData source, string newName, string prevName)
		{
			EventHandler<NameChangedEventArgs> d = nameChanged;
			if (d != null) d(this, new NameChangedEventArgs(source, newName, prevName));
		}

		void this_nameChanged(object sender, NameChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public MediaData()
		{
			this.nameChanged += new EventHandler<NameChangedEventArgs>(this_nameChanged);
		}

		/// <summary>
		/// Gets the <see cref="MediaDataManager"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The assicoated <see cref="MediaDataManager"/></returns>
		public MediaDataManager getMediaDataManager()
		{
			return getPresentation().getMediaDataManager();
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
		/// <param name="newLocalName">The new name</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new name is <c>null</c></exception>
		public void setName(string newName)
		{
			if (newName == null)
			{
				throw new exception.MethodParameterIsNullException("The name of an MediaData can not be null");
			}
			string prevName = mName;
			mName = newName;
			notifyNameChanged(this, newName, prevName);
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
			getMediaDataManager().removeMediaData(this);
		}

		/// <summary>
		/// Part of technical solution to make copy method return correct type. 
		/// In implementing classes this method should return a copy of the class instances
		/// </summary>
		/// <returns>The copy</returns>
		protected abstract MediaData protectedCopy();

		/// <summary>
		/// Creates a copy of the media data
		/// </summary>
		/// <returns>The copy</returns>
		public MediaData copy()
		{
			return protectedCopy();
		}

		/// <summary>
		/// Part of technical solution to make export method return correct type. 
		/// In implementing classes this method should return a export of the class instances
		/// </summary>
		/// <param name="destPres">The destination presentation of the export</param>
		/// <returns>The export</returns>
		protected abstract MediaData protectedExport(Presentation destPres); 

		/// <summary>
		/// Exports the media data to a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The given destination presentation</param>
		/// <returns>The exported media data</returns>
		public MediaData export(Presentation destPres)
		{
			return protectedExport(destPres);
		}

		#region IValueEquatable<MediaData> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public virtual bool valueEquals(MediaData other)
		{
			if (other == null) return false;
			if (GetType() != other.GetType()) return false;
			if (getName() != other.getName()) return false;
			return true;
		}

		#endregion
	}
}
