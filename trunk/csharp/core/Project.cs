using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;
using urakawa.events;
using urakawa.events.project;

namespace urakawa
{
    /// <summary>
    /// Represents a projects - part of the facade API, provides methods for opening and saving XUK files
    /// </summary>
    public class Project : XukAble, IValueEquatable<Project>, IChangeNotifier
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.Project;
        }

        private bool m_PrettyFormat = true;

        public override bool IsPrettyFormat()
        {
            return m_PrettyFormat;
        }

        public override void SetPrettyFormat(bool pretty)
        {
            if (m_PrettyFormat != pretty)
            {
                m_PrettyFormat = pretty;
                //PresentationFactory.RefreshQNames();
                for (int i = 0; i < NumberOfPresentations; i++)
                {
                    GetPresentation(i).RefreshFactoryQNames();
                }
            }
        }

        static void Main()
        {
            Debug.WriteLine("Hello World, from Urakawa SDK project !");
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

        /// <summary>
        /// Event fired after a <see cref="Presentation"/> has been added to the <see cref="Project"/>
        /// </summary>
        public event EventHandler<PresentationAddedEventArgs> PresentationAdded;

        /// <summary>
        /// Fires the <see cref="PresentationAdded"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="Project"/> to which a <see cref="Presentation"/> was added
        /// </param>
        /// <param name="addedPres">The <see cref="Presentation"/> that was added</param>
        protected void NotifyPresentationAdded(Project source, Presentation addedPres)
        {
            EventHandler<PresentationAddedEventArgs> d = PresentationAdded;
            if (d != null) d(this, new PresentationAddedEventArgs(source, addedPres));
        }

        private void this_presentationAdded(object sender, PresentationAddedEventArgs e)
        {
            NotifyChanged(e);
            e.AddedPresentation.Changed += Presentation_changed;
        }

        private void Presentation_changed(object sender, DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        /// <summary>
        /// Event fired after a <see cref="Presentation"/> has been added to the <see cref="Project"/>
        /// </summary>
        public event EventHandler<PresentationRemovedEventArgs> PresentationRemoved;

        /// <summary>
        /// Fires the <see cref="PresentationRemoved"/> event
        /// </summary>
        /// <param name="source">
        /// The source, that is the <see cref="Project"/> to which a <see cref="Presentation"/> was added
        /// </param>
        /// <param name="removedPres">The <see cref="Presentation"/> that was added</param>
        protected void NotifyPresentationRemoved(Project source, Presentation removedPres)
        {
            EventHandler<PresentationRemovedEventArgs> d = PresentationRemoved;
            if (d != null) d(this, new PresentationRemovedEventArgs(source, removedPres));
        }

        private void this_presentationRemoved(object sender, PresentationRemovedEventArgs e)
        {
            e.RemovedPresentation.Changed -= Presentation_changed;
            NotifyChanged(e);
        }

        #endregion

        private List<Presentation> mPresentations;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Project()
        {
            mXukStrings = new XukStrings(this);
            mPresentations = new List<Presentation>();
            PresentationAdded += this_presentationAdded;
            PresentationRemoved += this_presentationRemoved;
        }

        private XukStrings mXukStrings;

        public XukStrings XukStrings
        {
            get { return mXukStrings; }
            set { mXukStrings = value; }
        }

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
        public void OpenXuk(Uri fileUri)
        {
            if (fileUri == null)
            {
                throw new exception.MethodParameterIsNullException("The source URI cannot be null");
            }
            OpenXukAction action = new OpenXukAction(this, fileUri);
            action.Execute();
        }

        /// <summary>
        /// Opens an XUK file and loads the project from this.
        /// DO NOT USE ! THE STREAM IS NULL. USE THE STREAM-BASED method instead !
        /// TODO: make public once the Stream issue is fixed.
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the source XUK file (can be null)</param>
        /// <param name="reader">The <see cref="XmlReader"/> of the source XUK file (cannot be null)</param>
        private void OpenXuk(Uri fileUri, XmlReader reader)
        {
            if (reader == null)
            {
                throw new exception.MethodParameterIsNullException("The source XML reader cannot be null");
            }
            OpenXukAction action = new OpenXukAction(this, fileUri, reader);
            action.Execute();
        }
        /// <summary>
        /// Saves the <see cref="Project"/> to a XUK file
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
        public void SaveXuk(Uri fileUri)
        {
            if (fileUri == null)
            {
                throw new exception.MethodParameterIsNullException("The destination URI cannot be null");
            }
            SaveXukAction action = new SaveXukAction(this, this, fileUri);
            action.Execute();
        }

        /// <summary>
        /// Saves the <see cref="Project"/> to a XUK file
        /// </summary>
        /// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
        /// <param name="writer">The <see cref="XmlWriter"/> of the destination XUK file</param>
        public void SaveXuk(Uri fileUri, XmlWriter writer)
        {
            if (writer == null)
            {
                throw new exception.MethodParameterIsNullException("The destination XML writer cannot be null");
            }
            SaveXukAction action = new SaveXukAction(this, this, fileUri, writer);
            action.Execute();
        }


        /// <summary>
        /// Gets the <see cref="urakawa.Presentation"/> of the <see cref="Project"/> at a given index
        /// </summary>
        /// <param name="index">The index of the <see cref="Presentation"/> to get</param>
        /// <returns>The presentation at the given index</returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="index"/> is not in <c>[0;this.getNumberOfPresentations()-1]</c>
        /// </exception>
        public Presentation GetPresentation(int index)
        {
            if (index < 0 || NumberOfPresentations <= index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    String.Format(
                        "There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
                        index, NumberOfPresentations - 1));
            }
            return mPresentations[index];
        }

        /// <summary>
        /// Gets a list of the <see cref="Presentation"/>s in the <see cref="Project"/>
        /// </summary>
        /// <returns>The list</returns>
        public List<Presentation> ListOfPresentations
        {
            get { return new List<Presentation>(mPresentations); }
        }

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
            if (index < 0 || NumberOfPresentations < index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
                                                                              index, NumberOfPresentations));
            }
            if (mPresentations.Contains(newPres))
            {
                if (mPresentations.IndexOf(newPres) != index)
                {
                    throw new exception.IsAlreadyManagerOfException(String.Format(
                                                                        "The new Presentation already exists in the Project at index {0:0}",
                                                                        mPresentations.IndexOf(newPres)));
                }
            }
            if (index < NumberOfPresentations)
            {
                RemovePresentation(index);
                mPresentations.Insert(index, newPres);
            }
            else
            {
                mPresentations.Add(newPres);
            }
            newPres.Project = this;
            NotifyPresentationAdded(this, newPres);
        }

        /// <summary>
        /// Adds a <see cref="Presentation"/> to the <see cref="Project"/>
        /// </summary>
        /// <param name="newPres">The <see cref="Presentation"/> to add</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="newPres"/> is <c>null</c></exception>
        /// <exception cref="exception.IsAlreadyManagerOfException">
        /// Thrown when <paramref name="newPres"/> already exists in <c>this</c>
        /// </exception>
        public void AddPresentation(Presentation newPres)
        {
            SetPresentation(newPres, NumberOfPresentations);
        }

        /// <summary>
        /// Adds a newly created <see cref="Presentation"/> to the <see cref="Project"/>, 
        /// as returned by <c>this.PresentationFactory.Create()</c>
        /// </summary>
        /// <returns>The newly created and added <see cref="Presentation"/></returns>
        public Presentation AddNewPresentation()
        {
            Presentation newPres = PresentationFactory.Create();
            AddPresentation(newPres);

            if (IsPrettyFormat())
            {
                warmUpAllFactories(newPres);
            }

            return newPres;
        }

        /// <summary>
        /// creates and immediately discards objects via each factory
        /// in order to initialize and cache the mapping between XUK names (pretty or compressed) and actual types.
        /// CAlling this method is not required, it is provided for use-cases where the XUK XML is required to 
        /// contain all the factory mappings, even though the types are not actually used in the document instance.
        /// (useful for debugging factory types in XUK)
        /// </summary>
        private static void warmUpAllFactories(Presentation pres)
        {
            Channel ch = pres.ChannelFactory.Create();
            pres.ChannelsManager.RemoveChannel(ch);
            ch = pres.ChannelFactory.CreateAudioChannel();
            pres.ChannelsManager.RemoveChannel(ch);
            ch = pres.ChannelFactory.CreateTextChannel();
            pres.ChannelsManager.RemoveChannel(ch);
            ch = pres.ChannelFactory.CreateImageChannel();
            pres.ChannelsManager.RemoveChannel(ch);
            //
            DataProvider dp = pres.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            pres.DataProviderManager.RemoveDataProvider(dp, true);
            //
            MediaData md = pres.MediaDataFactory.CreateAudioMediaData();
            pres.MediaDataManager.RemoveMediaData(md);
            //
            pres.CommandFactory.CreateCompositeCommand();
            //
            TreeNode treeNode = pres.TreeNodeFactory.Create();
            ManagedAudioMedia manMedia = pres.MediaFactory.CreateManagedAudioMedia();
            pres.CommandFactory.CreateManagedAudioMediaInsertDataCommand(treeNode, manMedia, manMedia, Time.Zero);
            pres.CommandFactory.CreateTreeNodeSetManagedAudioMediaCommand(treeNode, manMedia);
            //
            pres.MediaFactory.CreateExternalImageMedia();
            pres.MediaFactory.CreateExternalVideoMedia();
            pres.MediaFactory.CreateExternalTextMedia();
            pres.MediaFactory.CreateExternalAudioMedia();
            //MediaFactory.CreateManagedAudioMedia(); DONE ALREADY (see above)
            pres.MediaFactory.CreateSequenceMedia();
            pres.MediaFactory.CreateTextMedia();
            //
            pres.MetadataFactory.CreateMetadata();
            //
            pres.PropertyFactory.CreateChannelsProperty();
            pres.PropertyFactory.CreateXmlProperty();
            //
            //TreeNodeFactory.Create(); DONE ALREADY (see above)

            Debug.Assert(pres.DataProviderManager.IsEmpty);
            Debug.Assert(pres.ChannelsManager.IsEmpty);
            Debug.Assert(pres.MediaDataManager.IsEmpty);
        }
        /// <summary>
        /// Gets the number of <see cref="Presentation"/>s in the <see cref="Project"/>
        /// </summary>
        /// <returns>The number of <see cref="Presentation"/>s</returns>
        public int NumberOfPresentations
        {
            get { return mPresentations.Count; }
        }

        /// <summary>
        /// Removes the <see cref="Presentation"/> at a given index
        /// </summary>
        /// <param name="index">The given index</param>
        /// <returns>The removed <see cref="Presentation"/></returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when <paramref name="index"/> is not in <c>[0;this.getNumberOfPresentations()-1]</c>
        /// </exception>
        public Presentation RemovePresentation(int index)
        {
            if (index < 0 || NumberOfPresentations <= index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(String.Format(
                                                                              "There is no Presentation at index {0:0}, index must be between 0 and {1:0}",
                                                                              index, NumberOfPresentations - 1));
            }
            Presentation pres = GetPresentation(index);
            mPresentations.RemoveAt(index);
            NotifyPresentationRemoved(this, pres);
            return pres;
        }

        /// <summary>
        /// Removes all <see cref="Presentation"/>s from the <see cref="Project"/>
        /// </summary>
        public void RemoveAllPresentations()
        {
            mPresentations.Clear();
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
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukNamespaceUri)
            {
                if (source.LocalName == XukStrings.PresentationFactory)
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

        private void XukInPresentations(XmlReader source, ProgressHandler handler)
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
                            pres.XukIn(source, handler, this);
                            AddPresentation(pres);
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);
            PresentationFactory.XukOut(destination, baseUri, handler);
            destination.WriteStartElement(XukStrings.Presentations, XukNamespaceUri);
            foreach (Presentation pres in ListOfPresentations)
            {
                pres.DataProviderManager.RegenerateUids();
                pres.ChannelsManager.RegenerateUids();
                pres.MediaDataManager.RegenerateUids();

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
        public bool ValueEquals(Project other)
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
            if (NumberOfPresentations != other.NumberOfPresentations)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            for (int index = 0; index < NumberOfPresentations; index++)
            {
                if (!GetPresentation(index).ValueEquals(other.GetPresentation(index)))
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