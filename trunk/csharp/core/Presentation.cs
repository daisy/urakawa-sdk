using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using AudioLib;
using urakawa.commands;
using urakawa.core;
using urakawa.data;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.media.data.video;
using urakawa.metadata.daisy;
using urakawa.progress;
using urakawa.property;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data;
using urakawa.metadata;
using urakawa.undo;
using urakawa.xuk;
using urakawa.events;
using urakawa.events.presentation;
using urakawa.ExternalFiles;
using urakawa.property.alt;

namespace urakawa
{
    [XukNameUglyPrettyAttribute("prs", "Presentation")]
    public class Presentation : XukAble, IValueEquatable<Presentation>, IChangeNotifier
    {
        //public string GetNewUid(string prefix, ref ulong startIndex)
        //{
        //    string strFormat = prefix + "{0:00000}";

        //    while (startIndex < UInt64.MaxValue)
        //    {
        //        string newId = String.Format(strFormat, startIndex);

        //        if (!DataProviderManager.IsManagerOf(newId)
        //            && !ChannelsManager.IsManagerOf(newId)
        //            && !MediaDataManager.IsManagerOf(newId)
        //            && !ExternalFilesDataManager.IsManagerOf(newId))
        //        {
        //            return newId;
        //        }

        //        startIndex++;
        //    }
        //    throw new OverflowException("YOU HAVE WAY TOO MANY UIDs !!!");
        //}


        public override bool PrettyFormat
        {
            set
            {
                throw new NotImplementedException("PrettyFormat");
                //Project.PrettyFormat = value;
            }
            get
            {
                return Project.PrettyFormat;
            }
        }

        //public void RefreshFactoryQNames()
        //{
        //    Project.PresentationFactory.RefreshQNames();
        //    ChannelFactory.RefreshQNames();
        //    DataProviderFactory.RefreshQNames();
        //    MediaDataFactory.RefreshQNames();
        //    CommandFactory.RefreshQNames();
        //    MediaFactory.RefreshQNames();
        //    MetadataFactory.RefreshQNames();
        //    PropertyFactory.RefreshQNames();
        //    TreeNodeFactory.RefreshQNames();
        //    ExternalFilesDataFactory.RefreshQNames();
        //}

        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="Presentation"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void notifyChanged(DataModelChangedEventArgs args)
        {
            EventHandler<DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        /// <summary>
        /// Event fired after the language of the <see cref="Presentation"/> has changed
        /// </summary>
        public event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        /// <summary>
        /// Fires the <see cref="LanguageChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="Presentation"/> whoose language has changed</param>
        /// <param name="newLang">The new language</param>
        /// <param name="prevLang">The language prior to the change</param>
        protected void notifyLanguageChanged(Presentation source, string newLang, string prevLang)
        {
            EventHandler<LanguageChangedEventArgs> d = LanguageChanged;
            if (d != null) d(this, new LanguageChangedEventArgs(source, newLang, prevLang));
        }

        private void this_languageChanged(object sender, LanguageChangedEventArgs e)
        {
            notifyChanged(e);
        }

        /// <summary>
        /// Fired when the base <see cref="Uri"/> has changed
        /// </summary>
        public event EventHandler<RootUriChangedEventArgs> RootUriChanged;

        /// <summary>
        /// Fires the <see cref="RootUriChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="Presentation"/> whoose root uri changed</param>
        /// <param name="newUri"></param>
        /// <param name="prevUri"></param>
        protected void notifyRootUriChanged(Presentation source, Uri newUri, Uri prevUri)
        {
            EventHandler<RootUriChangedEventArgs> d = RootUriChanged;
            if (d != null) d(this, new RootUriChangedEventArgs(source, newUri, prevUri));
        }

        private void this_rootNodeChanged(object sender, RootNodeChangedEventArgs e)
        {
            notifyChanged(e);
        }

        /// <summary>
        /// Event fired after the root <see cref="TreeNode"/> of the <see cref="Presentation"/> has changed
        /// </summary>
        public event EventHandler<RootNodeChangedEventArgs> RootNodeChanged;

        /// <summary>
        /// Fires the <see cref="RootNodeChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="Presentation"/> whoose root node has changed</param>
        /// <param name="newRoot">The new root node</param>
        /// <param name="prevRoot">Thye root node prior to the change</param>
        protected void notifyRootNodeChanged(Presentation source, TreeNode newRoot, TreeNode prevRoot)
        {
            EventHandler<RootNodeChangedEventArgs> d = RootNodeChanged;
            if (d != null) d(this, new RootNodeChangedEventArgs(source, newRoot, prevRoot));
        }

        private void this_rootUriChanged(object sender, RootUriChangedEventArgs e)
        {
            notifyChanged(e);
        }

        private void RootNode_Changed(object sender, DataModelChangedEventArgs e)
        {
            notifyChanged(e);
        }

        //private void UndoRedoManager_Changed(object sender, DataModelChangedEventArgs e)
        //{
        //    notifyChanged(e);
        //}

        ///// <summary>
        ///// Event fired after a <see cref="Metadata"/> item has been added to the <see cref="Presentation"/>
        ///// </summary>
        //public event EventHandler<MetadataAddedEventArgs> MetadataAdded;

        ///// <summary>
        ///// Fires the <see cref="MetadataAdded"/> event
        ///// </summary>
        ///// <param name="addee">The <see cref="Metadata"/> item that was added</param>
        //protected void notifyMetadataAdded(Metadata addee)
        //{
        //    EventHandler<MetadataAddedEventArgs> d = MetadataAdded;
        //    if (d != null) d(this, new MetadataAddedEventArgs(this, addee));
        //}

        ///// <summary>
        ///// Event fired after a <see cref="Metadata"/> item has been removed from the <see cref="Presentation"/>
        ///// </summary>
        //public event EventHandler<MetadataDeletedEventArgs> MetadataDeleted;

        ///// <summary>
        ///// Fires the <see cref="MetadataDeleted"/> event
        ///// </summary>
        ///// <param name="deletee">The <see cref="Metadata"/> item that was removed</param>
        //protected void notifyMetadataDeleted(Metadata deletee)
        //{
        //    EventHandler<MetadataDeletedEventArgs> d = MetadataDeleted;
        //    if (d != null) d(this, new MetadataDeletedEventArgs(this, deletee));
        //}

        private void this_metadataRemoved(object sender, ObjectRemovedEventArgs<Metadata> ev)
        {
            ev.m_RemovedObject.Changed -= Metadata_Changed;
            notifyChanged(ev);
        }

        private void this_metadataAdded(object sender, ObjectAddedEventArgs<Metadata> ev)
        {
            ev.m_AddedObject.Changed += Metadata_Changed;
            notifyChanged(ev);
        }

        private void Metadata_Changed(object sender, DataModelChangedEventArgs e)
        {
            notifyChanged(e);
        }

        #endregion

        /// <summary>
        /// Default constructor - for system use only.
        /// <see cref="Presentation"/>s should be created using the <see cref="PresentationFactory"/>
        /// </summary>
        public Presentation()
        {
            mMetadata = new ObjectListProvider<Metadata>(this, true);
            mRootNodeInitialized = false;
            LanguageChanged += this_languageChanged;
            RootUriChanged += this_rootUriChanged;
            RootNodeChanged += this_rootNodeChanged;

            mMetadata.ObjectAdded += this_metadataAdded;
            mMetadata.ObjectRemoved += this_metadataRemoved;
        }

        private Project mProject;
        //
        private TreeNodeFactory mTreeNodeFactory;
        private PropertyFactory mPropertyFactory;
        private ChannelFactory mChannelFactory;
        private MediaFactory mMediaFactory;
        private MediaDataFactory mMediaDataFactory;
        private DataProviderFactory mDataProviderFactory;
        private CommandFactory mCommandFactory;
        private MetadataFactory mMetadataFactory;
        private ExternalFileDataFactory m_ExternalFileDataFactory;
        private property.alt.AlternateContentFactory m_AlternateContentFactory;
        //
        // use this to bypass XUK parsing/serialization of the UndoRedoManager
        // (to avoid issues with our current XukAble Commands implementation)
        private bool m_IgnoreUndoRedoStack = true;

        private UndoRedoManager mUndoRedoManager;
        private DataProviderManager mDataProviderManager;
        private MediaDataManager mMediaDataManager;
        private ExternalFilesDataManager m_ExternalFileDataManager;
        private ChannelsManager mChannelsManager;
        //
        private TreeNode mRootNode;
        private bool mRootNodeInitialized;
        private Uri mRootUri;
        private string mLanguage;
        private TreeNode mHeadNode;

        private ObjectListProvider<Metadata> mMetadata;
        public ObjectListProvider<Metadata> Metadatas
        {
            get
            {
                return mMetadata;
            }
        }


        public void XukIn(XmlReader source, IProgressHandler handler, Project project)
        {
            mProject = project;
            XukIn(source, handler);
            mProject = null;


            List<Metadata> identifiers = new List<Metadata>();

            foreach (Metadata md in this.Metadatas.ContentsAs_Enumerable)
            {
                if (SupportedMetadata_Z39862005.DC_Identifier.Equals(md.NameContentAttribute.Name, StringComparison.OrdinalIgnoreCase))
                {
                    identifiers.Add(md);
                }
            }

            if (identifiers.Count == 0)
            {
                // NOP
                return;
            }
            else if (identifiers.Count == 1)
            {
                if (identifiers[0].IsMarkedAsPrimaryIdentifier)
                {
                    return;
                }

#if DEBUG
                Debugger.Break();
#endif
                identifiers[0].IsMarkedAsPrimaryIdentifier = true;
                return;
            }
            else if (identifiers.Count > 1)
            {
                foreach (Metadata md in identifiers)
                {
                    if (md.IsMarkedAsPrimaryIdentifier)
                    {
                        return;
                    }
                }
                foreach (Metadata md in identifiers)
                {
                    Metadata toMark = null;
                    foreach (MetadataAttribute metadataAttribute in md.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (@"id".Equals(metadataAttribute.Name, StringComparison.OrdinalIgnoreCase)
                            || @"xml:id".Equals(metadataAttribute.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            toMark = md;
                            break;
                        }
                    }
                    if (toMark != null)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        toMark.IsMarkedAsPrimaryIdentifier = true;
                        return;
                    }
                }
            }
#if DEBUG
            Debugger.Break();
#endif
        }


        /// <summary>
        /// Gets the <see cref="Project"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public Project Project
        {
            get
            {
                if (mProject == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The Presentation has not been initialized with an owning Project");
                }
                return mProject;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The Project can not be null");
                }
                if (mProject != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with an owning Project");
                }
                mProject = value;
            }
        }

        /// <summary>
        /// Gets the language of the presentation
        /// </summary>
        /// <returns>The language</returns>
        public string Language
        {
            get { return mLanguage; }
            set
            {
                if (value == "")
                {
                    throw new exception.MethodParameterIsEmptyStringException(
                        "The language can not be an empty string");
                }
                string prevLang = mLanguage;
                mLanguage = value;
                if (mLanguage != prevLang) notifyLanguageChanged(this, mLanguage, prevLang);
            }
        }

        /// <summary>
        /// Gets the head node for holding header data <see cref="TreeNode"/> of <c>this</c>
        /// </summary>
        /// <returns>The Head node</returns>
        /// <remarks>
        /// <see cref="HeadNode "/> is initialized lazily: 
        /// If <see cref="HeadNode"/> is retrieved before it is explicitly set (possibly explicitly set to null),
        /// it is initialized to a newly created <see cref="TreeNode"/>
        /// </remarks>
        public TreeNode HeadNode
        {
            get
            {
                if (mHeadNode == null) HeadNode = TreeNodeFactory.Create();
                return mHeadNode;
            }
            set
            {
                if (value != null)
                {
                    if (value.Parent != null)
                    {
                        throw new exception.NodeHasParentException(
                            "A TreeNode with a parent can not be the Head node of a Presentation");
                    }
                    if (value.Presentation != this)
                    {
                        throw new exception.NodeInDifferentPresentationException(
                            "The head TreeNode of a Presentation has to belong to that Presentation");
                    }
                }
                mHeadNode = value;
                notifyChanged(new DataModelChangedEventArgs(this));
            }
        }


        /// <summary>
        /// Gets the root <see cref="TreeNode"/> of <c>this</c>
        /// </summary>
        /// <returns>The root</returns>
        /// <remarks>
        /// <see cref="RootNode"/> is initialized lazily: 
        /// If <see cref="RootNode"/> is retrieved before it is explicitly set (possibly explicitly set to null),
        /// it is initialized to a newly created <see cref="TreeNode"/>
        /// </remarks>
        public TreeNode RootNode
        {
            get
            {
                if (!mRootNodeInitialized) RootNode = TreeNodeFactory.Create();
                return mRootNode;
            }
            set
            {
                mRootNodeInitialized = true;
                if (value != null)
                {
                    if (value.Parent != null)
                    {
                        throw new exception.NodeHasParentException(
                            "A TreeNode with a parent can not be the root of a Presentation");
                    }
                    if (value.Presentation != this)
                    {
                        throw new exception.NodeInDifferentPresentationException(
                            "The root TreeNode of a Presentation has to belong to that Presentation");
                    }
                }
                if (value != mRootNode)
                {
                    TreeNode prevRoot = mRootNode;
                    if (prevRoot != null)
                        prevRoot.Changed -= RootNode_Changed;
                    mRootNode = value;
                    if (mRootNode != null)
                        mRootNode.Changed += RootNode_Changed;
                    notifyRootNodeChanged(this, mRootNode, prevRoot);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="TreeNodeFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="TreeNodeFactory"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="TreeNodeFactory"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public TreeNodeFactory TreeNodeFactory
        {
            get
            {
                if (mTreeNodeFactory == null)
                {
                    mTreeNodeFactory = new TreeNodeFactory(this);
                }
                return mTreeNodeFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="PropertyFactory"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="PropertyFactory"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public PropertyFactory PropertyFactory
        {
            get
            {
                if (mPropertyFactory == null)
                {
                    mPropertyFactory = new PropertyFactory(this);
                }
                return mPropertyFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="AlternateContentFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="AlternateContentFactory"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="AlternateContentFactory"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public AlternateContentFactory AlternateContentFactory
        {
            get
            {
                if (m_AlternateContentFactory == null)
                {
                    m_AlternateContentFactory = new AlternateContentFactory(this);
                }
                return m_AlternateContentFactory;
            }
        }


        /// <summary>
        /// Gets the <see cref="UndoRedoManager"/> of <c>this</c>
        /// </summary>
        /// <remark>
        /// The <see cref="UndoRedoManager"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public UndoRedoManager UndoRedoManager
        {
            get
            {
                if (mUndoRedoManager == null)
                {
                    mUndoRedoManager = new UndoRedoManager(this);
                    //mUndoRedoManager.Changed += new EventHandler<DataModelChangedEventArgs>(UndoRedoManager_Changed);
                }
                return mUndoRedoManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="CommandFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="CommandFactory"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="CommandFactory"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public CommandFactory CommandFactory
        {
            get
            {
                if (mCommandFactory == null)
                {
                    mCommandFactory = new CommandFactory(this);
                }
                return mCommandFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="MediaFactory"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="MediaFactory"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public MediaFactory MediaFactory
        {
            get
            {
                if (mMediaFactory == null)
                {
                    mMediaFactory = new MediaFactory(this);
                }
                return mMediaFactory;
            }
        }

        /// <summary>
        /// Gets the root <see cref="Uri"/> of the <see cref="Presentation"/>
        /// </summary>
        /// <returns>The root <see cref="Uri"/></returns>
        /// <remarks>
        /// The root <see cref="Uri"/> is initialized lazily
        /// </remarks>
        public Uri RootUri
        {
            get
            {
                if (mRootUri == null)
                {
                    mRootUri = new Uri(System.IO.Directory.GetCurrentDirectory());
                }
                return mRootUri;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The base Uri can not be null");
                }
                if (!value.IsAbsoluteUri)
                {
                    throw new exception.InvalidUriException("The base uri must be absolute");
                }
                Uri prev = mRootUri;
                mRootUri = value;
                if (mRootUri != prev)
                {
                    notifyRootUriChanged(this, mRootUri, prev);
                }
            }
        }

        ///// <summary>
        ///// Gets a list of the <see cref="Media"/> used by a given <see cref="TreeNode"/>. 
        ///// </summary>
        ///// <param name="node">The node</param>
        ///// <returns>The list</returns>
        ///// <remarks>
        ///// An <see cref="Media"/> is considered to be used by a <see cref="TreeNode"/> if the media
        ///// is linked to the node via. a <see cref="ChannelsProperty"/>
        ///// </remarks>
        //protected virtual List<Media> GetMediaUsedByTreeNode(TreeNode node)
        //{
        //    List<Media> res = new List<Media>();
        //    foreach (Property prop in node.Properties.ContentsAs_Enumerable)
        //    {
        //        if (prop is ChannelsProperty)
        //        {
        //            ChannelsProperty chProp = (ChannelsProperty)prop;
        //            foreach (Channel ch in chProp.UsedChannels)
        //            {
        //                res.Add(chProp.GetMedia(ch));
        //            }
        //        }
        //    }
        //    return res;
        //}

        ///// <summary>
        ///// Gets the list of <see cref="Media"/> used by the <see cref="TreeNode"/> tree of the presentation. 
        ///// Remark that a 
        ///// </summary>
        ///// <returns>The list</returns>
        //public List<Media> UsedMedia
        //{
        //    get
        //    {
        //        List<Media> res = new List<Media>();
        //        if (RootNode != null)
        //        {
        //            CollectUsedMedia(RootNode, res);
        //        }
        //        return res;
        //    }
        //}

        //private void CollectUsedMedia(TreeNode node, ICollection<Media> collectedMedia)
        //{
        //    foreach (Media m in GetMediaUsedByTreeNode(node))
        //    {
        //        if (!collectedMedia.Contains(m)) collectedMedia.Add(m);
        //    }
        //    for (int i = 0; i < node.Children.Count; i++)
        //    {
        //        CollectUsedMedia(node.Children.Get(i), collectedMedia);
        //    }
        //}

        /// <summary>
        /// Gets the <see cref="ChannelFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public ChannelFactory ChannelFactory
        {
            get
            {
                if (mChannelFactory == null)
                {
                    mChannelFactory = new ChannelFactory(this);
                }
                return mChannelFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="ChannelsManager"/> of <c>this</c>
        /// </summary>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public ChannelsManager ChannelsManager
        {
            get
            {
                if (mChannelsManager == null)
                {
                    mChannelsManager = new ChannelsManager(this);
                }
                return mChannelsManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaDataManager"/> of <c>this</c>
        /// </summary>
        public MediaDataManager MediaDataManager
        {
            get
            {
                if (mMediaDataManager == null)
                {
                    mMediaDataManager = new MediaDataManager(this);
                }
                return mMediaDataManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaDataFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public MediaDataFactory MediaDataFactory
        {
            get
            {
                if (mMediaDataFactory == null)
                {
                    mMediaDataFactory = new MediaDataFactory(this);
                }
                return mMediaDataFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataProviderManager"/> of <c>this</c>
        /// </summary>
        public DataProviderManager DataProviderManager
        {
            get
            {
                if (mDataProviderManager == null)
                {
                    mDataProviderManager = new DataProviderManager(this);
                }
                return mDataProviderManager;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataProviderFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public DataProviderFactory DataProviderFactory
        {
            get
            {
                if (mDataProviderFactory == null)
                {
                    mDataProviderFactory = new DataProviderFactory(this);
                }
                return mDataProviderFactory;
            }
        }

        public ExternalFilesDataManager ExternalFilesDataManager
        {
            get
            {
                if (m_ExternalFileDataManager == null)
                {
                    m_ExternalFileDataManager = new ExternalFilesDataManager(this);
                }
                return m_ExternalFileDataManager;
            }
        }

        public ExternalFileDataFactory ExternalFilesDataFactory
        {
            get
            {
                if (m_ExternalFileDataFactory == null)
                {
                    m_ExternalFileDataFactory = new ExternalFileDataFactory(this);
                }
                return m_ExternalFileDataFactory;
            }
        }


        #region Metadata


        /// <summary>
        /// Gets the <see cref="MetadataFactory"/> of <c>this</c>
        /// </summary>
        public MetadataFactory MetadataFactory
        {
            get
            {
                if (mMetadataFactory == null)
                {
                    mMetadataFactory = new MetadataFactory(this);
                }
                return mMetadataFactory;
            }
        }

        internal TreeNode.StringChunk m_PreviousTextLocal;

        internal long m_XukedInTreeNodes;
        public long XukedInTreeNodes
        {
            get { return m_XukedInTreeNodes; }
        }


        /// <summary>
        /// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
        /// in the <see cref="urakawa.Project"/> with a given name
        /// </summary>
        /// <param name="name">The given name</param>
        /// <returns>The <see cref="List{Metadata}"/> of <see cref="Metadata"/></returns>
        public List<Metadata> GetMetadata(string name)
        {
            List<Metadata> list = new List<Metadata>();
            foreach (Metadata md in mMetadata.ContentsAs_Enumerable)
            {
                if (md.NameContentAttribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(md);
                }
            }
            return list;
        }

        /// <summary>
        /// Deletes all <see cref="Metadata"/>s with a given name
        /// </summary>
        /// <param name="name">The given name</param>
        public void DeleteMetadata(string name)
        {
            foreach (Metadata md in GetMetadata(name))
            {
                Metadatas.Remove(md);
            }
        }


        #endregion

        #region IXUKAble members

        /// <summary>
        /// Clears the <see cref="Presentation"/>,
        /// setting all owned members to <c>null</c>
        /// </summary>
        protected override void Clear()
        {
            mTreeNodeFactory = null;
            mPropertyFactory = null;
            mChannelFactory = null;
            mChannelsManager = null;
            mMediaFactory = null;
            mMediaDataManager = null;
            mMediaDataFactory = null;
            mDataProviderManager = null;
            mDataProviderFactory = null;
            mUndoRedoManager = null;
            mCommandFactory = null;
            mRootNode = null;
            mRootNodeInitialized = false;
            mRootUri = null;
            mLanguage = null;
            mHeadNode = null;

            foreach (Metadata md in mMetadata.ContentsAs_ListCopy)
            {
                mMetadata.Remove(md);
            }
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a Presentation xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            Uri baseUri = new Uri(System.IO.Directory.GetCurrentDirectory());
            if (!string.IsNullOrEmpty(source.BaseURI)) baseUri = new Uri(baseUri, source.BaseURI);
            RootUri = new Uri(baseUri, "./");

            //string rootUri = source.GetAttribute(XukStrings.RootUri);
            //if (rootUri == null)
            //{
            //    RootUri = baseUri;
            //}
            //else
            //{
            //    RootUri = new Uri(baseUri, rootUri);
            //}
            string lang = XukAble.ReadXukAttribute(source, XukAble.Language_NAME);
            if (lang != null) lang = lang.Trim();
            if (lang == "") lang = null;
            Language = lang;
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Reads an <see cref="xuk.IXukAble"/> instance from one of the children of a xuk element,
        /// more specifically the one with matching Xuk QName
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="xukAble">The instance to read</param>
        /// <param name="handler">The handler for progress</param>
        protected static void XukInXukAbleFromChild(XmlReader source, IXukAble xukAble, IProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.NamespaceURI == xukAble.GetXukNamespace()
                            && xukAble.GetXukName_().Match(source.LocalName))
                        {
                            xukAble.XukIn(source, handler);
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

        private void XukInMetadata(XmlReader source, IProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    Metadata newMeta = MetadataFactory.Create(source.LocalName, source.NamespaceURI);
                    if (newMeta != null)
                    {
                        newMeta.XukIn(source, handler);
                        mMetadata.Insert(mMetadata.Count, newMeta);
                    }
                    else if (!source.IsEmptyElement)
                    {
                        //Read past unidentified element
                        source.ReadSubtree().Close();
                    }
                }
                else if (source.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (source.EOF)
                {
                    throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }


        /// <summary>
        /// Reads the Head node <see cref="TreeNode"/> of <c>this</c> from a <c>mHeadNode</c> xuk xml element
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        /// <remarks>The read is considered succesful even if no valid Head node is found</remarks>
        protected void XukInHeadNode(XmlReader source, IProgressHandler handler)
        {
            HeadNode = null;
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        TreeNode newHeadNode = TreeNodeFactory.Create(source.LocalName, source.NamespaceURI);
                        if (newHeadNode != null)
                        {
                            newHeadNode.XukIn(source, handler);
                            HeadNode = newHeadNode;
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
        /// Reads the root <see cref="TreeNode"/> of <c>this</c> from a <c>mRootNode</c> xuk xml element
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        /// <remarks>The read is considered succesful even if no valid root node is found</remarks>
        protected void XukInRootNode(XmlReader source, IProgressHandler handler)
        {
            RootNode = null;
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        TreeNode newRoot = TreeNodeFactory.Create(source.LocalName, source.NamespaceURI);
                        if (newRoot != null)
                        {
                            newRoot.XukIn(source, handler);
                            RootNode = newRoot;
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

        //private delegate T CreatorDelegate<T>(string ln, string ns);

        //private delegate void SetDelegate<T>(T obj);

        //private static void XukInXukAbleFromChild<T>(XmlReader source, CreatorDelegate<T> creator, SetDelegate<T> setter, ProgressHandler handler) where T : class, IXukAble
        //{
        //    if (!source.IsEmptyElement)
        //    {
        //        bool foundObj = false;
        //        while (source.Read())
        //        {
        //            if (source.NodeType == XmlNodeType.Element)
        //            {
        //                if (foundObj)
        //                {
        //                    if (!source.IsEmptyElement)
        //                    {
        //                        source.ReadSubtree().Close();
        //                    }
        //                }
        //                else
        //                {
        //                    T instanceVar = creator(source.LocalName, source.NamespaceURI);
        //                    if (instanceVar != null)
        //                    {
        //                        setter(instanceVar);
        //                        foundObj = true;
        //                        instanceVar.XukIn(source, handler);
        //                    }
        //                    else if (!source.IsEmptyElement)
        //                    {
        //                        source.ReadSubtree().Close();
        //                    }
        //                }
        //            }
        //            else if (source.NodeType == XmlNodeType.EndElement)
        //            {
        //                break;
        //            }
        //            if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
        //        }
        //    }
        //}


        /// <summary>
        /// Reads a child of a Presentation xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                if (
                    //XukAble.GetXukName(typeof(TreeNodeFactory))
                    TreeNodeFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    TreeNodeFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(PropertyFactory))
                    PropertyFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    PropertyFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(ChannelFactory))
                    ChannelFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    ChannelFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(ChannelsManager))
                    ChannelsManager.GetXukName_()
                    .Match(source.LocalName))
                {
                    ChannelsManager.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(MediaFactory))
                    MediaFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    MediaFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(MediaDataFactory))
                    MediaDataFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    MediaDataFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(MediaDataManager))
                    MediaDataManager.GetXukName_()
                    .Match(source.LocalName))
                {
                    MediaDataManager.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(DataProviderFactory))
                    DataProviderFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    DataProviderFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(DataProviderManager))
                    DataProviderManager.GetXukName_()
                    .Match(source.LocalName))
                {
                    DataProviderManager.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(CommandFactory))
                    CommandFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    CommandFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(UndoRedoManager))
                    UndoRedoManager.GetXukName_()
                    .Match(source.LocalName))
                {
                    if (!m_IgnoreUndoRedoStack)
                        UndoRedoManager.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(MetadataFactory))
                    MetadataFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    MetadataFactory.XukIn(source, handler);
                }
                else if (source.LocalName == XukStrings.Metadatas)
                {
                    XukInMetadata(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(ExternalFileDataFactory))
                    ExternalFilesDataFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    ExternalFilesDataFactory.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(ExternalFileDataManager))
                    ExternalFilesDataManager.GetXukName_()
                    .Match(source.LocalName))
                {
                    ExternalFilesDataManager.XukIn(source, handler);
                }
                else if (
                    //XukAble.GetXukName(typeof(AlternateContentFactory))
                    AlternateContentFactory.GetXukName_()
                    .Match(source.LocalName))
                {
                    AlternateContentFactory.XukIn(source, handler);
                }
                else if (source.LocalName == XukStrings.RootNode)
                {
                    m_PreviousTextLocal = null;
                    m_XukedInTreeNodes = 0;
                    XukInRootNode(source, handler);
                }
                else if (source.LocalName == XukStrings.HeadNode)
                {

                    XukInHeadNode(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!readItem) base.XukInChild(source, handler);
        }

        /// <summary>
        /// Writes the attributes of a Presentation element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            //if (baseUri == null)
            //{
            //    destination.WriteAttributeString(XukStrings.RootUri, RootUri.AbsoluteUri);
            //}
            //else
            //{
            //    destination.WriteAttributeString(XukStrings.RootUri, baseUri.MakeRelativeUri(RootUri).ToString());
            //}
            if (!String.IsNullOrEmpty(Language))
            {
                destination.WriteAttributeString(XukAble.Language_NAME.z(PrettyFormat), Language);
            }
        }

        /// <summary>
        /// Write the child elements of a Presentation element.
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


            TreeNodeFactory.XukOut(destination, baseUri, handler);

            PropertyFactory.XukOut(destination, baseUri, handler);

            ChannelFactory.XukOut(destination, baseUri, handler);

            MediaFactory.XukOut(destination, baseUri, handler);

            DataProviderFactory.XukOut(destination, baseUri, handler);

            MediaDataFactory.XukOut(destination, baseUri, handler);

            CommandFactory.XukOut(destination, baseUri, handler);

            MetadataFactory.XukOut(destination, baseUri, handler);

            ExternalFilesDataFactory.XukOut(destination, baseUri, handler);

            ChannelsManager.XukOut(destination, baseUri, handler);

            DataProviderManager.XukOut(destination, baseUri, handler);

            MediaDataManager.XukOut(destination, baseUri, handler);

            ExternalFilesDataManager.XukOut(destination, baseUri, handler);

            AlternateContentFactory.XukOut(destination, baseUri, handler);

            destination.WriteStartElement(XukStrings.Metadatas, XukAble.XUK_NS);
            foreach (Metadata md in mMetadata.ContentsAs_Enumerable)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();


            if (!m_IgnoreUndoRedoStack)
                UndoRedoManager.XukOut(destination, baseUri, handler);

            if (mHeadNode != null)
            {
                destination.WriteStartElement(XukStrings.HeadNode, XukAble.XUK_NS);
                HeadNode.XukOut(destination, baseUri, handler);
                destination.WriteEndElement();
            }

            destination.WriteStartElement(XukStrings.RootNode, XukAble.XUK_NS);
            RootNode.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();
        }

        #endregion

        #region IValueEquatable<Presentation> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public virtual bool ValueEquals(Presentation other)
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
            if (!ChannelsManager.ValueEquals(other.ChannelsManager))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (!DataProviderManager.ValueEquals(other.DataProviderManager))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (!MediaDataManager.ValueEquals(other.MediaDataManager))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            //ReadOnlyCollection<Metadata> thisMetadata = Metadatas.ContentsAs_ListAsReadOnly;
            //ReadOnlyCollection<Metadata> otherMetadata = other.Metadatas.ContentsAs_ListAsReadOnly;
            if (Metadatas.Count != other.Metadatas.Count)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            foreach (Metadata m in Metadatas.ContentsAs_Enumerable)
            {
                bool found = false;
                foreach (Metadata om in other.Metadatas.ContentsAs_Enumerable)
                {
                    if (m.ValueEquals(om)) found = true;
                }
                if (!found)
                {
                    //System.Diagnostics.Debug.Fail("! ValueEquals !");
                    return false;
                }
            }
            if (Language != other.Language)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (!RootNode.ValueEquals(other.RootNode))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            return true;
        }

        #endregion

        [Conditional("DEBUG")]
        public virtual void WarmUpAllFactories()
        {
            Channel ch = ChannelFactory.Create();
            ChannelsManager.RemoveManagedObject(ch);

            ch = ChannelFactory.CreateTextChannel();
            ChannelsManager.RemoveManagedObject(ch);

            ch = ChannelFactory.CreateImageChannel();
            ChannelsManager.RemoveManagedObject(ch);

            ch = ChannelFactory.CreateVideoChannel();
            ChannelsManager.RemoveManagedObject(ch);

            ch = ChannelFactory.CreateAudioXChannel();
            ChannelsManager.RemoveManagedObject(ch);
            //
            DataProvider dpAudio = DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpAudio, true);
            //
            dpAudio = DataProviderFactory.Create(DataProviderFactory.AUDIO_MP3_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpAudio, true);
            //
            dpAudio = DataProviderFactory.Create(DataProviderFactory.AUDIO_MP4_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpAudio, true);
            //
            dpAudio = DataProviderFactory.Create(DataProviderFactory.AUDIO_OGG_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpAudio, true);
            //
            DataProvider dpImage = DataProviderFactory.Create(DataProviderFactory.IMAGE_BMP_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpImage, true);
            //
            dpImage = DataProviderFactory.Create(DataProviderFactory.IMAGE_PNG_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpImage, true);
            //
            dpImage = DataProviderFactory.Create(DataProviderFactory.IMAGE_JPG_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpImage, true);
            //
            dpImage = DataProviderFactory.Create(DataProviderFactory.IMAGE_GIF_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpImage, true);
            //
            dpImage = DataProviderFactory.Create(DataProviderFactory.IMAGE_SVG_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpImage, true);
            //
            DataProvider dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_AVI_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_MOV_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_MPG_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_MP4_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_WMV_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_WEBM_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            dpVideo = DataProviderFactory.Create(DataProviderFactory.VIDEO_OGG_MIME_TYPE);
            DataProviderManager.RemoveDataProvider(dpVideo, true);
            //
            CommandFactory.CreateCompositeCommand();
            //
            MediaData mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_JPG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_JPEG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_BMP_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_PNG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_GIF_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_SVG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            mdImage = MediaDataFactory.CreateImageMediaData(DataProviderFactory.IMAGE_SVGZ_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdImage);
            //
            ManagedImageMedia manImgMedia = MediaFactory.CreateManagedImageMedia();
            manImgMedia.MediaData = mdImage;
            //
            MediaData mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_MPG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_MPEG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_MP4_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_AVI_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_WEBM_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_OGG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_MOV_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            mdVideo = MediaDataFactory.CreateVideoMediaData(DataProviderFactory.VIDEO_WMV_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdVideo);
            //
            ManagedVideoMedia manVideoMedia = MediaFactory.CreateManagedVideoMedia();
            manVideoMedia.MediaData = mdVideo;
            //
            MediaData mdAudio = MediaDataFactory.CreateAudioMediaData(DataProviderFactory.AUDIO_MP3_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdAudio);
            mdAudio = MediaDataFactory.CreateAudioMediaData(DataProviderFactory.AUDIO_MP4_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdAudio);
            mdAudio = MediaDataFactory.CreateAudioMediaData(DataProviderFactory.AUDIO_OGG_EXTENSION);
            MediaDataManager.RemoveManagedObject(mdAudio);
            mdAudio = MediaDataFactory.CreateAudioMediaData(DataProviderFactory.AUDIO_WAV_EXTENSION);
            //
            TreeNode treeNode = TreeNodeFactory.Create();
            ManagedAudioMedia manMedia = MediaFactory.CreateManagedAudioMedia();
            manMedia.MediaData = mdAudio;
            TreeNodeSetManagedAudioMediaCommand cmd1 = CommandFactory.CreateTreeNodeSetManagedAudioMediaCommand(treeNode, manMedia, treeNode);

            foreach (MediaData mediaData in cmd1.UsedMediaData)
            {
                DebugFix.Assert(mediaData == cmd1.ManagedAudioMedia.AudioMediaData);
                DebugFix.Assert(mediaData == mdAudio);
                MediaDataManager.RemoveManagedObject(mediaData);
            }
            //
            ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
            Channel audioChannel = ChannelFactory.CreateAudioChannel();
            chProp.SetMedia(audioChannel, manMedia);
            ManagedAudioMedia manMedia2 = MediaFactory.CreateManagedAudioMedia();
            manMedia2.MediaData = mdAudio.Copy();
            ManagedAudioMediaInsertDataCommand cmd2 = CommandFactory.CreateManagedAudioMediaInsertDataCommand(treeNode, manMedia2, 0, treeNode);

            foreach (MediaData mediaData in cmd2.UsedMediaData)
            {
                DebugFix.Assert(mediaData == cmd2.OriginalManagedAudioMedia.AudioMediaData
                    || mediaData == cmd2.ManagedAudioMediaSource.AudioMediaData);
                //DebugFix.Assert(mediaData == mdAudio);
                MediaDataManager.RemoveManagedObject(mediaData);
            }
            //
            TreeNodeAndStreamSelection selection = new TreeNodeAndStreamSelection();
            selection.m_TreeNode = treeNode;
            selection.m_LocalStreamLeftMark = -1;
            selection.m_LocalStreamRightMark = -1;
            TreeNodeAudioStreamDeleteCommand cmd3 = CommandFactory.CreateTreeNodeAudioStreamDeleteCommand(selection, treeNode);

            foreach (MediaData mediaData in cmd3.UsedMediaData)
            {
                DebugFix.Assert(mediaData == cmd3.OriginalManagedAudioMedia.AudioMediaData);
                //DebugFix.Assert(mediaData == mdAudio);
                MediaDataManager.RemoveManagedObject(mediaData);
            }
            //
            ChannelsManager.RemoveManagedObject(audioChannel);
            //
            Metadata meta = MetadataFactory.CreateMetadata();
            meta.NameContentAttribute = new MetadataAttribute();
            meta.NameContentAttribute.Name = "dummy name";
            meta.NameContentAttribute.NamespaceUri = "dummy namespace";
            meta.NameContentAttribute.Value = "dummy content";
            //
            CommandFactory.CreateTreeNodeSetIsMarkedCommand(treeNode, false);
            //
            ChannelsProperty chPropTxt = treeNode.GetOrCreateChannelsProperty();
            Channel textChannel = ChannelFactory.CreateTextChannel();
            TextMedia txtMedia = MediaFactory.CreateTextMedia();
            txtMedia.Text = "ping";
            chPropTxt.SetMedia(textChannel, txtMedia);

            treeNode.XukInAfter_TextMediaCache();

            CommandFactory.CreateTreeNodeChangeTextCommand(treeNode, "pong");
            ChannelsManager.RemoveManagedObject(textChannel);
            //
            CommandFactory.CreateMetadataAddCommand(meta);
            CommandFactory.CreateMetadataRemoveCommand(meta);
            CommandFactory.CreateMetadataSetContentCommand(meta, "dummy");
            CommandFactory.CreateMetadataSetNameCommand(meta, "dummy");
            CommandFactory.CreateMetadataSetIdCommand(meta, false);
            //
            MediaFactory.CreateExternalImageMedia();
            MediaFactory.CreateExternalVideoMedia();
            MediaFactory.CreateExternalTextMedia();
            MediaFactory.CreateExternalAudioMedia();
            //
#if ENABLE_SEQ_MEDIA
            MediaFactory.CreateSequenceMedia();
#endif// ENABLE_SEQ_MEDIA
            //
            PropertyFactory.CreateChannelsProperty();
            PropertyFactory.CreateXmlProperty();
            //
            AlternateContentProperty altContentProp = PropertyFactory.CreateAlternateContentProperty();

            CommandFactory.CreateAlternateContentMetadataAddCommand(treeNode, altContentProp, null, meta, null);
            CommandFactory.CreateAlternateContentMetadataRemoveCommand(treeNode, altContentProp, null, meta, null);
            CommandFactory.CreateAlternateContentMetadataSetNameCommand(altContentProp, null, meta.NameContentAttribute, "sample name");
            CommandFactory.CreateAlternateContentMetadataSetContentCommand(altContentProp, null, meta.NameContentAttribute, "sample content");
            //CommandFactory.CreateAlternateContentMetadataSetIdCommand(altContentProp, null, meta, false);

            AlternateContent altContent = AlternateContentFactory.CreateAlternateContent();

            CommandFactory.CreateAlternateContentAddCommand(treeNode, altContent);
            CommandFactory.CreateAlternateContentRemoveCommand(treeNode, altContent);

            CommandFactory.CreateAlternateContentMetadataAddCommand(treeNode, null, altContent, meta, null);
            CommandFactory.CreateAlternateContentMetadataRemoveCommand(treeNode, null, altContent, meta, null);
            CommandFactory.CreateAlternateContentMetadataSetNameCommand(null, altContent, meta.NameContentAttribute, "sample name");
            CommandFactory.CreateAlternateContentMetadataSetContentCommand(null, altContent, meta.NameContentAttribute, "sample content");
            //CommandFactory.CreateAlternateContentMetadataSetIdCommand(null, altContent, meta, false);

            CommandFactory.CreateAlternateContentSetManagedMediaCommand(treeNode, altContent, txtMedia);
            CommandFactory.CreateAlternateContentRemoveManagedMediaCommand(treeNode, altContent, txtMedia);

            //
            //
            GenericExternalFileData exFileDataGeneric = ExternalFilesDataFactory.Create<GenericExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataGeneric);
            //
            CSSExternalFileData exFileDataCSS = ExternalFilesDataFactory.Create<CSSExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataCSS);
            //
            XSLTExternalFileData exFileDataXSLT = ExternalFilesDataFactory.Create<XSLTExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataXSLT);
            //
            DTDExternalFileData exFileDataDTD = ExternalFilesDataFactory.Create<DTDExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataDTD);
            //
            NCXExternalFileData exFileDataNCX = ExternalFilesDataFactory.Create<NCXExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataNCX);
            //
            NavDocExternalFileData exFileDataNavDoc = ExternalFilesDataFactory.Create<NavDocExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataNavDoc);
            //
            CoverImageExternalFileData exFileDataCoverImage = ExternalFilesDataFactory.Create<CoverImageExternalFileData>();
            ExternalFilesDataManager.RemoveManagedObject(exFileDataCoverImage);
            ///
            //PLSExternalFileData exFileDataPLS = ExternalFilesDataFactory.Create<PLSExternalFileData>();
            //ExternalFilesDataManager.RemoveManagedObject(exFileDataPLS);
            //JSExternalFileData exFileDataJS = ExternalFilesDataFactory.Create<JSExternalFileData>();
            //ExternalFilesDataManager.RemoveManagedObject(exFileDataJS);
            //
            DebugFix.Assert(ExternalFilesDataManager.ManagedObjects.Count == 0);
            DebugFix.Assert(DataProviderManager.ManagedObjects.Count == 0);
            DebugFix.Assert(ChannelsManager.ManagedObjects.Count == 0);
            DebugFix.Assert(MediaDataManager.ManagedObjects.Count == 0);

#if DEBUG
            return;

            Debugger.Break();

            try
            {
                List<TypeAndQNames> registeredTypeAndQNames = new List<TypeAndQNames>();

                registeredTypeAndQNames.AddRange(Project.PresentationFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(ChannelFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(DataProviderFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(MediaDataFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(CommandFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(MediaFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(MetadataFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(PropertyFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(TreeNodeFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(ExternalFilesDataFactory.EnumerateRegisteredTypeAndQNames());
                registeredTypeAndQNames.AddRange(AlternateContentFactory.EnumerateRegisteredTypeAndQNames());


                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Console.WriteLine(ass.FullName);

                    foreach (Type type in ass.GetTypes())
                    {
                        if (type == typeof(XukAble) || typeof(XukAble).IsAssignableFrom(type))
                        {
                            Console.WriteLine("-----------");
                            Console.WriteLine(type.FullName);


                            if (type.ContainsGenericParameters)
                            {
                                DebugFix.Assert(type.IsAbstract);
                            }

                            string ns = GetXukNamespace(type);
                            DebugFix.Assert(!string.IsNullOrEmpty(ns));

                            if (type == typeof(XukAble))
                            {
                                Console.WriteLine(ns);
                            }

                            if (type.FullName.StartsWith("Obi."))
                            {
                                DebugFix.Assert(ns == "http://www.daisy.org/urakawa/obi");
                            }
                            else
                            {
                                DebugFix.Assert(ns == "http://www.daisy.org/urakawa/xuk/2.0");
                            }

                            if (type.IsAbstract)
                            {
                                Console.WriteLine("abstract");
                                continue;
                            }

                            if (type.FullName.StartsWith("Obi.Commands"))
                            {
                                continue;
                            }

                            if (type.Name == "DummyCommand")
                            {
                                continue;
                            }

                            if (type.Name == typeof(UndoRedoManager).Name
                                || type.Name == typeof(Project).Name)
                            {
                                continue;
                            }

                            if (type.Name == typeof(MetadataAttribute).Name
                                || type.Name == typeof(XmlAttribute).Name
                                || type.Name == typeof(PCMFormatInfo).Name)
                            {
                                continue;
                            }

                            if (IsAssignableFrom(type, typeof(GenericXukAbleFactory<>))
                                || IsAssignableFrom(type, typeof(XukAbleManager<>))
                                )
                            {
                                continue;
                            }

                            TypeAndQNames foundType = null;
                            foreach (TypeAndQNames tq in registeredTypeAndQNames)
                            {
                                if (tq.Type == type)
                                {
                                    DebugFix.Assert(foundType == null);
                                    foundType = tq;
                                }
                            }

                            DebugFix.Assert(foundType != null);

                            if (foundType != null)
                            {
                                DebugFix.Assert(foundType.QName != null);

                                if (foundType.QName != null)
                                {
                                    DebugFix.Assert(foundType.QName.NamespaceUri == ns);

                                    DebugFix.Assert(foundType.QName.LocalName != null);

                                    if (foundType.QName.LocalName != null)
                                    {
                                        string pretty = GetXukName(type, true);
                                        string ugly = GetXukName(type, false);

                                        DebugFix.Assert(!string.IsNullOrEmpty(pretty));
                                        DebugFix.Assert(!string.IsNullOrEmpty(ugly));

                                        DebugFix.Assert(foundType.QName.LocalName.Pretty == pretty);
                                        DebugFix.Assert(foundType.QName.LocalName.Ugly == ugly);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            Debugger.Break();
#endif
        }

        public static bool IsAssignableFrom(Type extendType, Type baseType)
        {
            while (extendType != null && !baseType.IsAssignableFrom(extendType))
            {
                if (extendType == typeof(object))
                {
                    return false;
                }

                if (extendType.IsGenericType && !extendType.IsGenericTypeDefinition)
                {
                    extendType = extendType.GetGenericTypeDefinition();
                }
                else
                {
                    extendType = extendType.BaseType;
                }
            }

            return true;
        }
    }
}