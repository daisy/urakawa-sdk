using System;
using System.Collections.Generic;
using System.Diagnostics;
using urakawa.data;
using urakawa.events;

namespace urakawa.media.data
{
    /// <summary>
    /// Abstract implementation of <see cref="MediaData"/> that provides the common functionality 
    /// needed by any implementation of <see cref="MediaData"/>
    /// </summary>
    public abstract class MediaData : WithPresentation, IChangeNotifier
    {
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="MediaData"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        {
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        ///// <summary>
        ///// Event fired after the name of the <see cref="Media"/> has changed
        ///// </summary>
        //public event EventHandler<NameChangedEventArgs> NameChanged;

        ///// <summary>
        ///// Fires the <see cref="NameChanged"/> event
        ///// </summary>
        ///// <param name="source">The source, that is the <see cref="MediaData"/> whoose name has changed</param>
        ///// <param name="newName">The new name</param>
        ///// <param name="prevName">The name prior to the change</param>
        //protected void NotifyNameChanged(MediaData source, string newName, string prevName)
        //{
        //    EventHandler<NameChangedEventArgs> d = NameChanged;
        //    if (d != null) d(this, new NameChangedEventArgs(source, newName, prevName));
        //}

        //private void this_nameChanged(object sender, NameChangedEventArgs e)
        //{
        //    NotifyChanged(e);
        //}

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        protected MediaData()
        {
            //this.NameChanged += new EventHandler<NameChangedEventArgs>(this_nameChanged);
        }

        /// <summary>
        /// Gets the UID of <c>this</c>.
        /// Convenience for <c><see cref="Presentation.MediaDataManager"/>.<see cref="urakawa.media.data.MediaDataManager.GetUidOfMediaData"/>(this)</c>
        /// </summary>
        /// <returns>The UID</returns>
        //public override string Uid
        //{
        //    set { throw new NotImplementedException(); }
        //    get { return Presentation.MediaDataManager.GetUidOfMediaData(this); }
        //}

        //private string mName = "";

        ///// <summary>
        ///// Gets the name of <c>this</c>
        ///// </summary>
        ///// <returns>The name</returns>
        //public string Name
        //{
        //    get { return mName; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw new exception.MethodParameterIsNullException("The name of an AudioMediaData can not be null");
        //        }
        //        string prevName = mName;
        //        mName = value;
        //        NotifyNameChanged(this, value, prevName);
        //    }
        //}

        /// <summary>
        /// Gets a <see cref="List{IDataProvider}"/> of the <see cref="DataProvider"/>s used by <c>this</c>
        /// </summary>
        /// <returns>The <see cref="List{DataProvider}"/></returns>
        public abstract IEnumerable<DataProvider> UsedDataProviders { get; }

        /// <summary>
        /// Deletes the <see cref="MediaData"/>, detaching it from it's manager and releasing 
        /// any <see cref="DataProvider"/>s used
        /// </summary>
        public virtual void Delete()
        {
            Presentation.MediaDataManager.RemoveManagedObject(this);
        }

        /// <summary>
        /// Part of technical solution to make copy method return correct type. 
        /// In implementing classes this method should return a copy of the class instances
        /// </summary>
        /// <returns>The copy</returns>
        protected abstract MediaData CopyProtected();

        /// <summary>
        /// Creates a copy of the media data
        /// </summary>
        /// <returns>The copy</returns>
        public MediaData Copy()
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            return CopyProtected();
        }

        /// <summary>
        /// Part of technical solution to make export method return correct type. 
        /// In implementing classes this method should return a export of the class instances
        /// </summary>
        /// <param name="destPres">The destination presentation of the export</param>
        /// <returns>The export</returns>
        protected abstract MediaData ExportProtected(Presentation destPres);

        /// <summary>
        /// Exports the media data to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentation</param>
        /// <returns>The exported media data</returns>
        public MediaData Export(Presentation destPres)
        {
//#if DEBUG
//            Debugger.Break();
//#endif //DEBUG

            return ExportProtected(destPres);
        }

        #region IValueEquatable<AudioMediaData> Members


        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            MediaData otherz = other as MediaData;
            if (otherz == null)
            {
                return false;
            }

            //if (Name != otherz.Name)
            //{
            //    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
            //    return false;
            //}
            return true;
        }

        #endregion
    }
}