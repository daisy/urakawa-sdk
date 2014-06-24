using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using urakawa.ExternalFiles;
using urakawa.media.data;
using urakawa.progress;
using urakawa.xuk;
using urakawa.events;

namespace urakawa
{
    [XukNameUglyPrettyAttribute("proj", "Project")]
    public class Project : XukAble, IValueEquatable<Project>, IChangeNotifier
    {
        public static string GetXukSchema(bool isPrettyFormat)
        {
            return ""; // TODO 
        }

        private bool m_PrettyFormat = true;
        public override bool PrettyFormat
        {
            set
            {
                m_PrettyFormat = value;
                m_PrettyFormat_STATIC = m_PrettyFormat;

                XukStrings.IsPrettyFormat = m_PrettyFormat;
            }
            get
            {
                m_PrettyFormat_STATIC = m_PrettyFormat;
                return m_PrettyFormat;
            }
        }


        static void Main()
        {
            Console.WriteLine("Testing CONSOLE.");
            Trace.WriteLine("Testing TRACE.");
            Debug.WriteLine("Testing DEBUG.");
        }

        #region Event related members

        public event EventHandler<urakawa.events.media.data.DataIsMissingEventArgs> dataIsMissing;
        public void notifyDataIsMissing(MediaData md, urakawa.exception.DataMissingException ex)
        {
            EventHandler<urakawa.events.media.data.DataIsMissingEventArgs> d = dataIsMissing;
            if (d != null) d(this, new urakawa.events.media.data.DataIsMissingEventArgs(md, ex));
        }

        /// <summary>
        /// Event fired after the <see cref="Project"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(DataModelChangedEventArgs args)
        {
            EventHandler<DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        ///// <summary>
        ///// Event fired after a <see cref="Presentation"/> has been added to the <see cref="Project"/>
        ///// </summary>
        //public event EventHandler<PresentationAddedEventArgs> PresentationAdded;

        ///// <summary>
        ///// Fires the <see cref="PresentationAdded"/> event
        ///// </summary>
        ///// <param name="source">
        ///// The source, that is the <see cref="Project"/> to which a <see cref="Presentation"/> was added
        ///// </param>
        ///// <param name="addedPres">The <see cref="Presentation"/> that was added</param>
        //protected void NotifyPresentationAdded(Project source, Presentation addedPres)
        //{
        //    EventHandler<PresentationAddedEventArgs> d = PresentationAdded;
        //    if (d != null) d(this, new PresentationAddedEventArgs(source, addedPres));
        //}

        private void this_presentationAdded(object sender, ObjectAddedEventArgs<Presentation> ev)
        {
            NotifyChanged(ev);
            ev.m_AddedObject.Changed += Presentation_changed;
        }

        private void Presentation_changed(object sender, DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        ///// <summary>
        ///// Event fired after a <see cref="Presentation"/> has been added to the <see cref="Project"/>
        ///// </summary>
        //public event EventHandler<PresentationRemovedEventArgs> PresentationRemoved;

        ///// <summary>
        ///// Fires the <see cref="PresentationRemoved"/> event
        ///// </summary>
        ///// <param name="source">
        ///// The source, that is the <see cref="Project"/> to which a <see cref="Presentation"/> was added
        ///// </param>
        ///// <param name="removedPres">The <see cref="Presentation"/> that was added</param>
        //protected void NotifyPresentationRemoved(Project source, Presentation removedPres)
        //{
        //    EventHandler<PresentationRemovedEventArgs> d = PresentationRemoved;
        //    if (d != null) d(this, new PresentationRemovedEventArgs(source, removedPres));
        //}

        private void this_presentationRemoved(object sender, ObjectRemovedEventArgs<Presentation> ev)
        {
            ev.m_RemovedObject.Changed -= Presentation_changed;
            NotifyChanged(ev);
        }

        #endregion

        private ObjectListProvider<Presentation> mPresentations;

        public ObjectListProvider<Presentation> Presentations
        {
            get
            {
                return mPresentations;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Project()
        {
            ////mXukStrings = new XukStrings(this);
            //XukStrings.RelocateProjectReference(this);

            mPresentations = new ObjectListProvider<Presentation>(this, false);
            mPresentations.ObjectAdded += this_presentationAdded;
            mPresentations.ObjectRemoved += this_presentationRemoved;
        }

        //private XukStrings mXukStrings;
        //public XukStrings XukStrings
        //{
        //    get { return mXukStrings; }
        //    set { mXukStrings = value; }
        //}

        private PresentationFactory mPresentationFactory;

        /// <summary>
        /// Gets the <see cref="PresentationFactory"/> of the <see cref="Project"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="PresentationFactory"/> of a <see cref="Project"/> is initialized lazily
        /// </remarks>
        public PresentationFactory PresentationFactory
        {
            get
            {
                if (mPresentationFactory == null) mPresentationFactory = new PresentationFactory();
                return mPresentationFactory;
            }
        }


        /// <summary>
        /// Opens an XUK file and loads the project from this
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the source XUK file (cannot be null)</param>
        //public void OpenXuk(Uri fileUri)
        //{
        //    if (fileUri == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("The source URI cannot be null");
        //    }
        //    OpenXukAction action = new OpenXukAction(this, fileUri);
        //    action.Execute();
        //}

        /// <summary>
        /// Opens an XUK file and loads the project from this.
        /// DO NOT USE ! THE STREAM IS NULL. USE THE STREAM-BASED method instead !
        /// TODO: make public once the Stream issue is fixed.
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the source XUK file (can be null)</param>
        /// <param name="reader">The <see cref="XmlReader"/> of the source XUK file (cannot be null)</param>
        //private void OpenXuk(Uri fileUri, XmlReader reader)
        //{
        //    if (reader == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("The source XML reader cannot be null");
        //    }
        //    OpenXukAction action = new OpenXukAction(this, fileUri, reader);
        //    action.Execute();
        //}
        /// <summary>
        /// Saves the <see cref="Project"/> to a XUK file
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
        //public void SaveXuk(Uri fileUri)
        //{
        //    if (fileUri == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("The destination URI cannot be null");
        //    }
        //    SaveXukAction action = new SaveXukAction(this, this, fileUri);
        //    action.Execute();
        //}

        /// <summary>
        /// Saves the <see cref="Project"/> to a XUK file
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
        /// <param name="writer">The <see cref="XmlWriter"/> of the destination XUK file</param>
        //public void SaveXuk(Uri fileUri, XmlWriter writer)
        //{
        //    if (writer == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("The destination XML writer cannot be null");
        //    }
        //    SaveXukAction action = new SaveXukAction(this, this, fileUri, writer);
        //    action.Execute();
        //}


        /// <summary>
        /// Sets the <see cref="Presentation"/> at a given index
        /// </summary>
        /// <param name="newPres">The <see cref="Presentation"/> to set</param>
        /// <param name="index">The given index</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="newPres"/> is <c>null</c></exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="index"/> is not in <c>[0;this.getNumberOfPresentations()]</c>
        /// </exception>
        /// <exception cref="exception.IsAlreadyManagerOfException">
        /// Thrown when <paramref name="newPres"/> already exists in <c>this</c> with another <paramref name="index"/>
        /// </exception>
        public void SetPresentation(Presentation newPres, int index)
        {
            if (newPres == null)
            {
                throw new exception.MethodParameterIsNullException("The new Presentation can not be null");
            }
            if (index < 0 || mPresentations.Count < index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
                                                                              index, mPresentations.Count));
            }
            if (mPresentations.IndexOf(newPres) != -1)
            {
                if (mPresentations.IndexOf(newPres) != index)
                {
                    throw new exception.IsAlreadyManagerOfException(String.Format(
                                                                        "The new Presentation already exists in the Project at index {0:0}",
                                                                        mPresentations.IndexOf(newPres)));
                }
            }
            if (index < mPresentations.Count)
            {
                mPresentations.Remove(mPresentations.Get(index));
                newPres.Project = this;
                mPresentations.Insert(index, newPres);
            }
            else
            {
                newPres.Project = this;
                mPresentations.Insert(mPresentations.Count, newPres);
            }
        }

        /// <summary>
        /// Adds a newly created <see cref="Presentation"/> to the <see cref="Project"/>, 
        /// as returned by <c>this.PresentationFactory.Create()</c>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>The newly created and added <see cref="Presentation"/></returns>
        public Presentation AddNewPresentation(Uri uri, string dataFolderPrefix)
        {
            Presentation newPres = PresentationFactory.Create(this, uri, dataFolderPrefix);

//#if DEBUG
//            newPres.WarmUpAllFactories();
//#endif
            mPresentations.Insert(mPresentations.Count, newPres);

            return newPres;
        }

        /// <summary>
        /// Removes all <see cref="Presentation"/>s from the <see cref="Project"/>
        /// </summary>
        public void RemoveAllPresentations()
        {
            foreach (Presentation pres in mPresentations.ContentsAs_ListCopy)
            {
                pres.Project = null;
                mPresentations.Remove(pres);
            }
        }

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="Project"/>, removing all <see cref="Presentation"/>s
        /// </summary>
        protected override void Clear()
        {
            RemoveAllPresentations();
            base.Clear();
        }

        /// <summary>
        /// Reads a child of a Project xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                if (XukAble.GetXukName(typeof(PresentationFactory)).Match(source.LocalName))
                {
                    PresentationFactory.XukIn(source, handler);
                    readItem = true;
                }
                else if (source.LocalName == XukStrings.Presentations)
                {
                    XukInPresentations(source, handler);
                    readItem = true;
                }
            }
            if (!readItem) base.XukInChild(source, handler);
        }

        private void XukInPresentations(XmlReader source, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        Presentation pres = PresentationFactory.Create(source.LocalName, source.NamespaceURI);
                        if (pres != null)
                        {
#if DEBUG
                            pres.Project = this;
                            pres.WarmUpAllFactories();
#endif

                            pres.XukIn(source, handler, this);
                            pres.Project = this;

                            mPresentations.Insert(mPresentations.Count, pres);

                            pres.ExternalFilesDataManager.RegenerateUids();
                            pres.ChannelsManager.RegenerateUids();
                            pres.MediaDataManager.RegenerateUids();
                            pres.DataProviderManager.RegenerateUids();
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

        /// <summary>
        /// Write the child elements of a Project element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);
            PresentationFactory.XukOut(destination, baseUri, handler);
            destination.WriteStartElement(XukStrings.Presentations, XukAble.XUK_NS);
            foreach (Presentation pres in mPresentations.ContentsAs_Enumerable)
            {
                pres.ExternalFilesDataManager.RegenerateUids();
                pres.ChannelsManager.RegenerateUids();
                pres.MediaDataManager.RegenerateUids();
                pres.DataProviderManager.RegenerateUids();

                pres.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();
        }

        #endregion

        #region IValueEquatable<Project> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public virtual bool ValueEquals(Project other)
        {
            if (other == null)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (GetType() != other.GetType())
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (mPresentations.Count != other.Presentations.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            for (int index = 0; index < mPresentations.Count; index++)
            {
                if (!mPresentations.Get(index).ValueEquals(other.Presentations.Get(index)))
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !");
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}