using System;
using System.Xml;
using System.Collections.Generic;
using urakawa.command;
using urakawa.core;
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
using CommandFactory=urakawa.command.CommandFactory;

namespace urakawa
{
    /// <summary>
    /// The primary container for a document tree consisting of <see cref="TreeNode"/>s,
    /// includes factories and managers for:
    /// <list type="bullet">
    /// <item><see cref="Property"/>s</item>
    /// <item><see cref="Channel"/>s</item>
    /// <item><see cref="Media"/></item>
    /// <item><see cref="MediaData"/></item>
    /// <item><see cref="DataProvider"/>s</item>
    /// <item><see cref="Metadata"/></item>
    /// </list>
    /// </summary>
    public class Presentation : XukAble, IValueEquatable<Presentation>, IChangeNotifier
    {
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

        private void UndoRedoManager_Changed(object sender, DataModelChangedEventArgs e)
        {
            notifyChanged(e);
        }

        /// <summary>
        /// Event fired after a <see cref="Metadata"/> item has been added to the <see cref="Presentation"/>
        /// </summary>
        public event EventHandler<MetadataAddedEventArgs> MetadataAdded;

        /// <summary>
        /// Fires the <see cref="MetadataAdded"/> event
        /// </summary>
        /// <param name="addee">The <see cref="Metadata"/> item that was added</param>
        protected void notifyMetadataAdded(Metadata addee)
        {
            EventHandler<MetadataAddedEventArgs> d = MetadataAdded;
            if (d != null) d(this, new MetadataAddedEventArgs(this, addee));
        }

        /// <summary>
        /// Event fired after a <see cref="Metadata"/> item has been removed from the <see cref="Presentation"/>
        /// </summary>
        public event EventHandler<MetadataDeletedEventArgs> MetadataDeleted;

        /// <summary>
        /// Fires the <see cref="MetadataDeleted"/> event
        /// </summary>
        /// <param name="deletee">The <see cref="Metadata"/> item that was removed</param>
        protected void notifyMetadataDeleted(Metadata deletee)
        {
            EventHandler<MetadataDeletedEventArgs> d = MetadataDeleted;
            if (d != null) d(this, new MetadataDeletedEventArgs(this, deletee));
        }

        private void this_metadataRemoved(object sender, MetadataDeletedEventArgs e)
        {
            e.DeletedMetadata.Changed -= Metadata_Changed;
            notifyChanged(e);
        }

        private void this_metadataAdded(object sender, MetadataAddedEventArgs e)
        {
            e.AddedMetadata.Changed += Metadata_Changed;
            notifyChanged(e);
        }

        private void Metadata_Changed(object sender, DataModelChangedEventArgs e)
        {
            notifyChanged(e);
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        protected internal Presentation()
        {
            mMetadata = new List<Metadata>();
            mRootNodeInitialized = false;
            LanguageChanged += this_languageChanged;
            RootUriChanged += this_rootUriChanged;
            RootNodeChanged += this_rootNodeChanged;
            MetadataAdded += this_metadataAdded;
            MetadataDeleted += this_metadataRemoved;
        }

        private Project mProject;
        private TreeNodeFactory mTreeNodeFactory;
        private PropertyFactory mPropertyFactory;
        private ChannelFactory mChannelFactory;
        private ChannelsManager mChannelsManager;
        private MediaFactory mMediaFactory;
        private MediaDataManager mMediaDataManager;
        private MediaDataFactory mMediaDataFactory;
        private DataProviderManager mDataProviderManager;
        private DataProviderFactory mDataProviderFactory;
        private UndoRedoManager mUndoRedoManager;
        private CommandFactory mCommandFactory;
        private MetadataFactory mMetadataFactory;
        private TreeNode mRootNode;
        private bool mRootNodeInitialized;
        private Uri mRootUri;
        private string mLanguage;
        private List<Metadata> mMetadata;

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
        /// Gets the <see cref="DataModelFactory"/> associated with the <see cref="Presentation"/>
        /// via. it's owning <see cref="urakawa.Project"/>
        /// </summary>
        /// <returns>The <see cref="DataModelFactory"/></returns>
        public DataModelFactory DataModelFactory
        {
            get { return Project.DataModelFactory; }
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
        /// Removes any <see cref="MediaData"/> and <see cref="DataProvider"/>s that are not used by any <see cref="TreeNode"/> in the document tree
        /// or by any <see cref="Command"/> in the <see cref="undo.UndoRedoManager"/> stacks (undo/redo/transaction).
        /// </summary>
        public void Cleanup()
        {
            media.data.utilities.CollectManagedMediaTreeNodeVisitor collectorVisitor
                = new media.data.utilities.CollectManagedMediaTreeNodeVisitor();
            if (RootNode != null)
            {
                RootNode.AcceptDepthFirst(collectorVisitor);
            }
            List<MediaData> usedMediaData = UndoRedoManager.ListOfUsedMediaData;
            foreach (IManaged mm in collectorVisitor.ListOfCollectedMedia)
            {
                if (!usedMediaData.Contains(mm.MediaData)) usedMediaData.Add(mm.MediaData);
            }
            List<DataProvider> usedDataProviders = new List<DataProvider>();
            foreach (MediaData md in MediaDataManager.ListOfMediaData)
            {
                if (usedMediaData.Contains(md))
                {
                    if (md is media.data.audio.codec.WavAudioMediaData)
                    {
                        ((media.data.audio.codec.WavAudioMediaData) md).ForceSingleDataProvider();
                    }
                    foreach (DataProvider dp in md.ListOfUsedDataProviders)
                    {
                        if (!usedDataProviders.Contains(dp)) usedDataProviders.Add(dp);
                    }
                }
                else
                {
                    md.Delete();
                }
            }
            foreach (DataProvider dp in DataProviderManager.ListOfDataProviders)
            {
                if (!usedDataProviders.Contains(dp)) dp.Delete();
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
                    mTreeNodeFactory = new TreeNodeFactory();
                    mTreeNodeFactory.Presentation = this;
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
                    mPropertyFactory = new PropertyFactory();
                    mPropertyFactory.Presentation = this;
                }
                return mPropertyFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="UndoRedoManager"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="UndoRedoManager"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="UndoRedoManager"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public UndoRedoManager UndoRedoManager
        {
            get
            {
                if (mUndoRedoManager == null)
                {
                    UndoRedoManager = DataModelFactory.CreateUndoRedoManager();
                }
                return mUndoRedoManager;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The UndoRedoManager can not be null");
                }
                if (mUndoRedoManager != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a UndoRedoManager");
                }
                mUndoRedoManager = value;
                mUndoRedoManager.Presentation = this;
                mUndoRedoManager.Changed += new EventHandler<DataModelChangedEventArgs>(UndoRedoManager_Changed);
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
                    mCommandFactory = new CommandFactory();
                    mCommandFactory.Presentation = this;
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
                    mMediaFactory = new MediaFactory();
                    mMediaFactory.Presentation = this;
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

        /// <summary>
        /// Gets a list of the <see cref="Media"/> used by a given <see cref="TreeNode"/>. 
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The list</returns>
        /// <remarks>
        /// An <see cref="Media"/> is considered to be used by a <see cref="TreeNode"/> if the media
        /// is linked to the node via. a <see cref="ChannelsProperty"/>
        /// </remarks>
        protected virtual List<Media> GetListOfMediaUsedByTreeNode(TreeNode node)
        {
            List<Media> res = new List<Media>();
            foreach (Property prop in node.GetListOfProperties())
            {
                if (prop is ChannelsProperty)
                {
                    ChannelsProperty chProp = (ChannelsProperty) prop;
                    foreach (Channel ch in chProp.ListOfUsedChannels)
                    {
                        res.Add(chProp.GetMedia(ch));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the list of <see cref="Media"/> used by the <see cref="TreeNode"/> tree of the presentation. 
        /// Remark that a 
        /// </summary>
        /// <returns>The list</returns>
        public List<Media> ListOfUsedMedia
        {
            get
            {
                List<Media> res = new List<Media>();
                if (RootNode != null)
                {
                    CollectUsedMedia(RootNode, res);
                }
                return res;
            }
        }

        private void CollectUsedMedia(TreeNode node, ICollection<Media> collectedMedia)
        {
            foreach (Media m in GetListOfMediaUsedByTreeNode(node))
            {
                if (!collectedMedia.Contains(m)) collectedMedia.Add(m);
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                CollectUsedMedia(node.GetChild(i), collectedMedia);
            }
        }

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
                    mChannelFactory = new ChannelFactory();
                    mChannelFactory.Presentation = this;
                }
                return mChannelFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="ChannelsManager"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public ChannelsManager ChannelsManager
        {
            get
            {
                if (mChannelsManager == null)
                {
                    ChannelsManager = DataModelFactory.CreateChannelsManager();
                }
                return mChannelsManager;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The ChannelsManager can not be null");
                }
                if (mChannelsManager != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a ChannelsManager");
                }
                mChannelsManager = value;
                mChannelsManager.Presentation = this;
            }
        }

        /// <summary>
        /// Gets the <see cref="MediaDataManager"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public MediaDataManager MediaDataManager
        {
            get
            {
                if (mMediaDataManager == null)
                {
                    MediaDataManager = DataModelFactory.CreateMediaDataManager();
                }
                return mMediaDataManager;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The MediaDataManager can not be null");
                }
                if (mMediaDataManager != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a MediaDataManager");
                }
                mMediaDataManager = value;
                mMediaDataManager.Presentation = this;
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
                    mMediaDataFactory = new MediaDataFactory();
                    mMediaDataFactory.Presentation = this;
                }
                return mMediaDataFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataProviderManager"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public DataProviderManager DataProviderManager
        {
            get
            {
                if (mDataProviderManager == null)
                {
                    DataProviderManager = DataModelFactory.CreateDataProviderManager();
                }
                return mDataProviderManager;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The IDataProviderManager can not be null");
                }
                if (mDataProviderManager != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a IDataProviderManager");
                }
                mDataProviderManager = value;
                mDataProviderManager.Presentation = this;
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
                    mDataProviderFactory = new DataProviderFactory();
                    mDataProviderFactory.Presentation = this;
                }
                return mDataProviderFactory;
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
                    mMetadataFactory = new MetadataFactory();
                    mMetadataFactory.Presentation = this;
                }
                return mMetadataFactory;
            }
        }


        /// <summary>
        /// Adds a <see cref="Metadata"/> to the <see cref="Presentation"/>
        /// </summary>
        /// <param name="metadata">The <see cref="Metadata"/> to add</param>
        public void AddMetadata(Metadata metadata)
        {
            mMetadata.Add(metadata);
            notifyMetadataAdded(metadata);
        }

        /// <summary>
        /// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
        /// in the <see cref="urakawa.Project"/>
        /// </summary>
        /// <returns>The <see cref="List{Metadata}"/> of metadata <see cref="Metadata"/></returns>
        public List<Metadata> ListOfMetadata
        {
            get { return new List<Metadata>(mMetadata); }
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
            foreach (Metadata md in mMetadata)
            {
                if (md.Name == name) list.Add(md);
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
                DeleteMetadata(md);
            }
        }

        /// <summary>
        /// Deletes a given <see cref="Metadata"/>
        /// </summary>
        /// <param name="metadata">The given <see cref="Metadata"/></param>
        /// <exception cref="exception.IsNotManagerOfException">
        /// When <paramref name="metadata"/> does not belong to the <see cref="Presentation"/>
        /// </exception>
        public void DeleteMetadata(Metadata metadata)
        {
            if (!mMetadata.Contains(metadata))
            {
                throw new exception.IsNotManagerOfException(
                    "The given Metadata item does not belong to the Presentation");
            }
            mMetadata.Remove(metadata);
            notifyMetadataDeleted(metadata);
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
            mMetadata.Clear();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a Presentation xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string rootUri = source.GetAttribute("rootUri");
            Uri baseUri = new Uri(System.IO.Directory.GetCurrentDirectory());
            if (source.BaseURI != "") baseUri = new Uri(baseUri, source.BaseURI);
            if (rootUri == null)
            {
                RootUri = baseUri;
            }
            else
            {
                RootUri = new Uri(baseUri, rootUri);
            }
            string lang = source.GetAttribute("language");
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
        protected static void XukInXukAbleFromChild(XmlReader source, IXukAble xukAble, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == xukAble.XukLocalName && source.NamespaceURI == xukAble.XukNamespaceUri)
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

        private void XukInMetadata(XmlReader source, ProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    Metadata newMeta = MetadataFactory.Create(source.LocalName, source.NamespaceURI);
                    if (newMeta != null)
                    {
                        mMetadata.Add(newMeta);
                        newMeta.XukIn(source, handler);
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
        /// Reads the root <see cref="TreeNode"/> of <c>this</c> from a <c>mRootNode</c> xuk xml element
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        /// <remarks>The read is considered succesful even if no valid root node is found</remarks>
        protected void XukInRootNode(XmlReader source, ProgressHandler handler)
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
                            RootNode = newRoot;
                            newRoot.XukIn(source, handler);
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

        private delegate T CreatorDelegate<T>(string ln, string ns);

        private delegate void SetDelegate<T>(T obj);

        private static void XukInXukAbleFromChild<T>(XmlReader source, CreatorDelegate<T> creator, SetDelegate<T> setter, ProgressHandler handler) where T : class, IXukAble
        {
            if (!source.IsEmptyElement)
            {
                bool foundObj = false;
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (foundObj)
                        {
                            if (!source.IsEmptyElement)
                            {
                                source.ReadSubtree().Close();
                            }
                        }
                        else
                        {
                            T instanceVar = creator(source.LocalName, source.NamespaceURI);
                            if (instanceVar != null)
                            {
                                setter(instanceVar);
                                foundObj = true;
                                instanceVar.XukIn(source, handler);
                            }
                            else if (!source.IsEmptyElement)
                            {
                                source.ReadSubtree().Close();
                            }
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
        /// Reads a child of a Presentation xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XUK_NS)
            {
                readItem = true;
                switch (source.LocalName)
                {
                    case "TreeNodeFactory":
                        TreeNodeFactory.XukIn(source, handler);
                        break;
                    case "PropertyFactory":
                        PropertyFactory.XukIn(source, handler);
                        break;
                    case "ChannelFactory":
                        ChannelFactory.XukIn(source, handler);
                        break;
                    case "mChannelsManager":
                        XukInXukAbleFromChild<ChannelsManager>(
                            source,
                            DataModelFactory.CreateChannelsManager,
                            delegate(ChannelsManager val) { ChannelsManager = val; },
                            handler);
                        break;
                    case "MediaFactory":
                        MediaFactory.XukIn(source, handler);
                        break;
                    case "MediaDataFactory":
                        MediaDataFactory.XukIn(source, handler);
                        break;
                    case "mMediaDataManager":
                        XukInXukAbleFromChild<MediaDataManager>(
                            source,
                            DataModelFactory.CreateMediaDataManager,
                            delegate(MediaDataManager val) { MediaDataManager = val; },
                            handler);
                        break;
                    case "DataProviderFactory":
                        DataProviderFactory.XukIn(source, handler);
                        break;
                    case "mDataProviderManager":
                        XukInXukAbleFromChild<DataProviderManager>(
                            source,
                            DataModelFactory.CreateDataProviderManager,
                            delegate(DataProviderManager val) { DataProviderManager = val; }, handler);
                        break;
                    case "CommandFactory":
                        CommandFactory.XukIn(source, handler);
                        break;
                    case "mUndoRedoManager":
                        XukInXukAbleFromChild<UndoRedoManager>(
                            source,
                            DataModelFactory.CreateUndoRedoManager,
                            delegate(UndoRedoManager val) { UndoRedoManager = val; },
                            handler);
                        break;
                    case "MetadataFactory":
                        MetadataFactory.XukIn(source, handler);
                        break;
                    case "mMetadata":
                        XukInMetadata(source, handler);
                        break;
                    case "mRootNode":
                        XukInRootNode(source, handler);
                        break;
                    default:
                        readItem = false;
                        break;
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
            if (baseUri == null)
            {
                destination.WriteAttributeString("rootUri", RootUri.AbsoluteUri);
            }
            else
            {
                destination.WriteAttributeString("rootUri", baseUri.MakeRelativeUri(RootUri).ToString());
            }
            if (Language != null)
            {
                destination.WriteAttributeString("language", Language);
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
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            base.XukOutChildren(destination, baseUri, handler);

            TreeNodeFactory.XukOut(destination, baseUri, handler);

            PropertyFactory.XukOut(destination, baseUri, handler);

            ChannelFactory.XukOut(destination, baseUri, handler);

            destination.WriteStartElement("mChannelsManager", XUK_NS);
            ChannelsManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            MediaFactory.XukOut(destination, baseUri, handler);

            DataProviderFactory.XukOut(destination, baseUri, handler);

            destination.WriteStartElement("mDataProviderManager", XUK_NS);
            DataProviderManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            MediaDataFactory.XukOut(destination, baseUri, handler);

            destination.WriteStartElement("mMediaDataManager", XUK_NS);
            MediaDataManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            CommandFactory.XukOut(destination, baseUri, handler);

            destination.WriteStartElement("mUndoRedoManager", XUK_NS);
            UndoRedoManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            MetadataFactory.XukOut(destination, baseUri, handler);

            destination.WriteStartElement("mMetadata", XUK_NS);
            foreach (Metadata md in mMetadata)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            destination.WriteStartElement("mRootNode", XUK_NS);
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
        public bool ValueEquals(Presentation other)
        {
            if (other == null) return false;
            if (!ChannelsManager.ValueEquals(other.ChannelsManager)) return false;
            if (!DataProviderManager.ValueEquals(other.DataProviderManager)) return false;
            if (!MediaDataManager.ValueEquals(other.MediaDataManager)) return false;
            if (!RootNode.ValueEquals(other.RootNode)) return false;
            List<Metadata> thisMetadata = ListOfMetadata;
            List<Metadata> otherMetadata = other.ListOfMetadata;
            if (thisMetadata.Count != otherMetadata.Count) return false;
            foreach (Metadata m in thisMetadata)
            {
                bool found = false;
                foreach (Metadata om in other.GetMetadata(m.Name))
                {
                    if (m.ValueEquals(om)) found = true;
                }
                if (!found) return false;
            }
            if (Language != other.Language) return false;
            return true;
        }

        #endregion
    }
}