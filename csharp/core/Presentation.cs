using System;
using System.Xml;
using System.Collections.Generic;
using urakawa.command;
using urakawa.core;
using urakawa.progress;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
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
    /// <item><see cref="IMedia"/></item>
    /// <item><see cref="MediaData"/></item>
    /// <item><see cref="IDataProvider"/>s</item>
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
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
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
        public event EventHandler<RootNodeChangedEventArgs> rootNodeChanged;

        /// <summary>
        /// Fires the <see cref="rootNodeChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="Presentation"/> whoose root node has changed</param>
        /// <param name="newRoot">The new root node</param>
        /// <param name="prevRoot">Thye root node prior to the change</param>
        protected void notifyRootNodeChanged(Presentation source, TreeNode newRoot, TreeNode prevRoot)
        {
            EventHandler<RootNodeChangedEventArgs> d = rootNodeChanged;
            if (d != null) d(this, new RootNodeChangedEventArgs(source, newRoot, prevRoot));
        }

        private void this_rootUriChanged(object sender, RootUriChangedEventArgs e)
        {
            notifyChanged(e);
        }

        private void rootNode_changed(object sender, DataModelChangedEventArgs e)
        {
            notifyChanged(e);
        }

        private void undoRedoManager_changed(object sender, DataModelChangedEventArgs e)
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
            e.DeletedMetadata.Changed -= new EventHandler<DataModelChangedEventArgs>(metadata_changed);
            notifyChanged(e);
        }

        private void this_metadataAdded(object sender, MetadataAddedEventArgs e)
        {
            e.AddedMetadata.Changed += new EventHandler<DataModelChangedEventArgs>(metadata_changed);
            notifyChanged(e);
        }

        private void metadata_changed(object sender, DataModelChangedEventArgs e)
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
            this.LanguageChanged += new EventHandler<LanguageChangedEventArgs>(this_languageChanged);
            this.RootUriChanged += new EventHandler<RootUriChangedEventArgs>(this_rootUriChanged);
            this.rootNodeChanged += new EventHandler<RootNodeChangedEventArgs>(this_rootNodeChanged);
            this.MetadataAdded += new EventHandler<MetadataAddedEventArgs>(this_metadataAdded);
            this.MetadataDeleted += new EventHandler<MetadataDeletedEventArgs>(this_metadataRemoved);
        }

        private Project mProject;
        private TreeNodeFactory mTreeNodeFactory;
        private PropertyFactory mPropertyFactory;
        private ChannelFactory mChannelFactory;
        private ChannelsManager mChannelsManager;
        private IMediaFactory mMediaFactory;
        private MediaDataManager mMediaDataManager;
        private MediaDataFactory mMediaDataFactory;
        private DataProviderManager mDataProviderManager;
        private DataProviderFactory mDataProviderFactory;
        private undo.UndoRedoManager mUndoRedoManager;
        private CommandFactory mCommandFactory;
        private TreeNode mRootNode;
        private bool mRootNodeInitialized;
        private Uri mRootUri;
        private string mLanguage;

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
        /// Removes any <see cref="MediaData"/> and <see cref="IDataProvider"/>s that are not used by any <see cref="TreeNode"/> in the document tree
        /// or by any <see cref="ICommand"/> in the <see cref="undo.UndoRedoManager"/> stacks (undo/redo/transaction).
        /// </summary>
        public void Cleanup()
        {
            urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor collectorVisitor
                = new urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor();
            if (RootNode != null)
            {
                RootNode.AcceptDepthFirst(collectorVisitor);
            }
            List<MediaData> usedMediaData = UndoRedoManager.ListOfUsedMediaData;
            foreach (IManagedMedia mm in collectorVisitor.ListOfCollectedMedia)
            {
                if (!usedMediaData.Contains(mm.MediaData)) usedMediaData.Add(mm.MediaData);
            }
            List<IDataProvider> usedDataProviders = new List<IDataProvider>();
            foreach (MediaData md in MediaDataManager.ListOfMediaData)
            {
                if (usedMediaData.Contains(md))
                {
                    if (md is urakawa.media.data.audio.codec.WavAudioMediaData)
                    {
                        ((urakawa.media.data.audio.codec.WavAudioMediaData) md).ForceSingleDataProvider();
                    }
                    foreach (IDataProvider dp in md.ListOfUsedDataProviders)
                    {
                        if (!usedDataProviders.Contains(dp)) usedDataProviders.Add(dp);
                    }
                }
                else
                {
                    md.Delete();
                }
            }
            foreach (IDataProvider dp in DataProviderManager.ListOfDataProviders)
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
                if (!mRootNodeInitialized) RootNode = TreeNodeFactory.CreateNode();
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
                        prevRoot.Changed -= new EventHandler<DataModelChangedEventArgs>(rootNode_changed);
                    mRootNode = value;
                    if (mRootNode != null)
                        mRootNode.Changed += new EventHandler<DataModelChangedEventArgs>(rootNode_changed);
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
                    TreeNodeFactory = DataModelFactory.CreateTreeNodeFactory();
                }
                return mTreeNodeFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The TreeNodeFactory can not be null");
                }
                if (mTreeNodeFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a TreeNodeFactory");
                }
                mTreeNodeFactory = value;
                mTreeNodeFactory.Presentation = this;
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
                    PropertyFactory = DataModelFactory.CreatePropertyFactory();
                }
                return mPropertyFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The PropertyFactory can not be null");
                }
                if (mPropertyFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a PropertyFactory");
                }
                mPropertyFactory = value;
                mPropertyFactory.Presentation = this;
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
                mUndoRedoManager.Changed += new EventHandler<DataModelChangedEventArgs>(undoRedoManager_changed);
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
                    CommandFactory = DataModelFactory.CreateCommandFactory();
                }
                return mCommandFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The CommandFactory can not be null");
                }
                if (mCommandFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a CommandFactory");
                }
                mCommandFactory = value;
                mCommandFactory.Presentation = this;
            }
        }

        /// <summary>
        /// Gets the <see cref="IMediaFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="IMediaFactory"/> of the <see cref="Presentation"/></returns>
        /// <remark>
        /// The <see cref="IMediaFactory"/> of a <see cref="urakawa.Project"/> is initialized lazily
        /// </remark>
        public IMediaFactory MediaFactory
        {
            get
            {
                if (mMediaFactory == null)
                {
                    MediaFactory = DataModelFactory.CreateMediaFactory();
                }
                return mMediaFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The IMediaFactory can not be null");
                }
                if (mMediaFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a IMediaFactory");
                }
                mMediaFactory = value;
                mMediaFactory.Presentation = this;
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
        /// Gets a list of the <see cref="IMedia"/> used by a given <see cref="TreeNode"/>. 
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The list</returns>
        /// <remarks>
        /// An <see cref="IMedia"/> is considered to be used by a <see cref="TreeNode"/> if the media
        /// is linked to the node via. a <see cref="ChannelsProperty"/>
        /// </remarks>
        protected virtual List<IMedia> getListOfMediaUsedByTreeNode(TreeNode node)
        {
            List<IMedia> res = new List<IMedia>();
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
        /// Gets the list of <see cref="IMedia"/> used by the <see cref="TreeNode"/> tree of the presentation. 
        /// Remark that a 
        /// </summary>
        /// <returns>The list</returns>
        public List<IMedia> ListOfUsedMedia
        {
            get
            {
                List<IMedia> res = new List<IMedia>();
                if (RootNode != null)
                {
                    collectUsedMedia(RootNode, res);
                }
                return res;
            }
        }

        private void collectUsedMedia(TreeNode node, List<IMedia> collectedMedia)
        {
            foreach (IMedia m in getListOfMediaUsedByTreeNode(node))
            {
                if (!collectedMedia.Contains(m)) collectedMedia.Add(m);
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                collectUsedMedia(node.GetChild(i), collectedMedia);
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
                    ChannelFactory = DataModelFactory.CreateChannelFactory();
                }
                return mChannelFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The ChannelFactory can not be null");
                }
                if (mChannelFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a ChannelFactory");
                }
                mChannelFactory = value;
                mChannelFactory.Presentation = this;
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
                    MediaDataFactory = DataModelFactory.CreateMediaDataFactory();
                }
                return mMediaDataFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The MediaDataFactory can not be null");
                }
                if (mMediaDataFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a MediaDataFactory");
                }
                mMediaDataFactory = value;
                mMediaDataFactory.Presentation = this;
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
                    DataProviderFactory = DataModelFactory.CreateDataProviderFactory();
                }
                return mDataProviderFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The DataProviderFactory can not be null");
                }
                if (mDataProviderFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a DataProviderFactory");
                }
                mDataProviderFactory = value;
                mDataProviderFactory.Presentation = this;
            }
        }

        #region Metadata

        private List<Metadata> mMetadata;
        private MetadataFactory mMetadataFactory;


        /// <summary>
        /// Gets the <see cref="MetadataFactory"/> of <c>this</c>
        /// </summary>
        /// <returns>The factory</returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="core.TreeNodeFactory"/>
        /// </exception>
        public MetadataFactory MetadataFactory
        {
            get
            {
                if (mMetadataFactory == null)
                {
                    MetadataFactory = DataModelFactory.CreateMetadataFactory();
                }
                return mMetadataFactory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The MetadataFactory can not be null");
                }
                if (mMetadataFactory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The Presentation has already been initialized with a MetadataFactory");
                }
                mMetadataFactory = value;
                mMetadataFactory.Presentation = this;
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
        protected override void clear()
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
            base.clear();
        }

        /// <summary>
        /// Reads the attributes of a Presentation xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void xukInAttributes(XmlReader source)
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
            base.xukInAttributes(source);
        }

        /// <summary>
        /// Reads an <see cref="xuk.IXukAble"/> instance from one of the children of a xuk element,
        /// more specifically the one with matching Xuk QName
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="xukAble">The instance to read</param>
        /// <param name="handler">The handler for progress</param>
        protected void xukInXukAbleFromChild(XmlReader source, IXukAble xukAble, ProgressHandler handler)
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

        private void xukInMetadata(XmlReader source, ProgressHandler handler)
        {
            if (source.IsEmptyElement) return;
            while (source.Read())
            {
                if (source.NodeType == XmlNodeType.Element)
                {
                    Metadata newMeta = MetadataFactory.CreateMetadata(source.LocalName, source.NamespaceURI);
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
        protected void xukInRootNode(XmlReader source, ProgressHandler handler)
        {
            RootNode = null;
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        TreeNode newRoot = TreeNodeFactory.CreateNode(source.LocalName, source.NamespaceURI);
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

        private void xukInXukAbleFromChild<T>(XmlReader source, T instanceVar, CreatorDelegate<T> creator,
                                              SetDelegate<T> setter, ProgressHandler handler) where T : IXukAble
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
                            instanceVar = creator(source.LocalName, source.NamespaceURI);
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
        protected override void xukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == ToolkitSettings.XUK_NS)
            {
                readItem = true;
                switch (source.LocalName)
                {
                    case "mTreeNodeFactory":
                        xukInXukAbleFromChild<TreeNodeFactory>(
                            source, null,
                            new CreatorDelegate<TreeNodeFactory>(DataModelFactory.CreateTreeNodeFactory),
                            new SetDelegate<TreeNodeFactory>(delegate(TreeNodeFactory val) { TreeNodeFactory = val; }),
                            handler);
                        break;
                    case "mPropertyFactory":
                        xukInXukAbleFromChild<PropertyFactory>(
                            source, null,
                            new CreatorDelegate<PropertyFactory>(DataModelFactory.CreatePropertyFactory),
                            new SetDelegate<PropertyFactory>(delegate(PropertyFactory val) { PropertyFactory = val; }),
                            handler);
                        break;
                    case "mChannelFactory":
                        xukInXukAbleFromChild<ChannelFactory>(
                            source, null,
                            new CreatorDelegate<ChannelFactory>(DataModelFactory.CreateChannelFactory),
                            new SetDelegate<ChannelFactory>(delegate(ChannelFactory val) { ChannelFactory = val; }),
                            handler);
                        break;
                    case "mChannelsManager":
                        xukInXukAbleFromChild<ChannelsManager>(
                            source, null,
                            new CreatorDelegate<ChannelsManager>(DataModelFactory.CreateChannelsManager),
                            new SetDelegate<ChannelsManager>(delegate(ChannelsManager val) { ChannelsManager = val; }),
                            handler);
                        break;
                    case "mMediaFactory":
                        xukInXukAbleFromChild<IMediaFactory>(
                            source, null,
                            new CreatorDelegate<IMediaFactory>(DataModelFactory.CreateMediaFactory),
                            new SetDelegate<IMediaFactory>(delegate(IMediaFactory val) { MediaFactory = val; }), handler);
                        break;
                    case "mMediaDataManager":
                        xukInXukAbleFromChild<MediaDataManager>(
                            source, null,
                            new CreatorDelegate<MediaDataManager>(DataModelFactory.CreateMediaDataManager),
                            new SetDelegate<MediaDataManager>(delegate(MediaDataManager val) { MediaDataManager = val; }),
                            handler);
                        break;
                    case "mMediaDataFactory":
                        xukInXukAbleFromChild<MediaDataFactory>(
                            source, null,
                            new CreatorDelegate<MediaDataFactory>(DataModelFactory.CreateMediaDataFactory),
                            new SetDelegate<MediaDataFactory>(delegate(MediaDataFactory val) { MediaDataFactory = val; }),
                            handler);
                        break;
                    case "mDataProviderManager":
                        xukInXukAbleFromChild<DataProviderManager>(
                            source, null,
                            new CreatorDelegate<DataProviderManager>(DataModelFactory.CreateDataProviderManager),
                            new SetDelegate<DataProviderManager>(
                                delegate(DataProviderManager val) { DataProviderManager = val; }), handler);
                        break;
                    case "mDataProviderFactory":
                        xukInXukAbleFromChild<DataProviderFactory>(
                            source, null,
                            new CreatorDelegate<DataProviderFactory>(DataModelFactory.CreateDataProviderFactory),
                            new SetDelegate<DataProviderFactory>(
                                delegate(DataProviderFactory val) { DataProviderFactory = val; }), handler);
                        break;
                    case "mUndoRedoManager":
                        xukInXukAbleFromChild<UndoRedoManager>(
                            source, null,
                            new CreatorDelegate<UndoRedoManager>(DataModelFactory.CreateUndoRedoManager),
                            new SetDelegate<UndoRedoManager>(delegate(UndoRedoManager val) { UndoRedoManager = val; }),
                            handler);
                        break;
                    case "mCommandFactory":
                        xukInXukAbleFromChild<CommandFactory>(
                            source, null,
                            new CreatorDelegate<CommandFactory>(DataModelFactory.CreateCommandFactory),
                            new SetDelegate<CommandFactory>(delegate(CommandFactory val) { CommandFactory = val; }),
                            handler);
                        break;
                    case "mMetadataFactory":
                        xukInXukAbleFromChild<metadata.MetadataFactory>(
                            source, null,
                            new CreatorDelegate<MetadataFactory>(DataModelFactory.CreateMetadataFactory),
                            new SetDelegate<MetadataFactory>(delegate(MetadataFactory val) { MetadataFactory = val; }),
                            handler);
                        break;
                    case "mMetadata":
                        xukInMetadata(source, handler);
                        break;
                    case "mRootNode":
                        xukInRootNode(source, handler);
                        break;
                    default:
                        readItem = false;
                        break;
                }
            }
            if (!readItem) base.xukInChild(source, handler);
        }

        /// <summary>
        /// Writes the attributes of a Presentation element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.xukOutAttributes(destination, baseUri);
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
        protected override void xukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            base.xukOutChildren(destination, baseUri, handler);
            destination.WriteStartElement("mTreeNodeFactory", urakawa.ToolkitSettings.XUK_NS);
            TreeNodeFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mPropertyFactory", urakawa.ToolkitSettings.XUK_NS);
            TreeNodeFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mChannelFactory", urakawa.ToolkitSettings.XUK_NS);
            ChannelFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mChannelsManager", urakawa.ToolkitSettings.XUK_NS);
            ChannelsManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mMediaFactory", urakawa.ToolkitSettings.XUK_NS);
            MediaFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mDataProviderFactory", urakawa.ToolkitSettings.XUK_NS);
            DataProviderFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mDataProviderManager", urakawa.ToolkitSettings.XUK_NS);
            DataProviderManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mMediaDataFactory", urakawa.ToolkitSettings.XUK_NS);
            MediaDataFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mMediaDataManager", urakawa.ToolkitSettings.XUK_NS);
            MediaDataManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mCommandFactory", urakawa.ToolkitSettings.XUK_NS);
            CommandFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mUndoRedoManager", urakawa.ToolkitSettings.XUK_NS);
            UndoRedoManager.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mMetadataFactory", urakawa.ToolkitSettings.XUK_NS);
            MetadataFactory.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            destination.WriteStartElement("mMetadata", urakawa.ToolkitSettings.XUK_NS);
            foreach (Metadata md in mMetadata)
            {
                md.XukOut(destination, baseUri, handler);
            }
            destination.WriteEndElement();

            destination.WriteStartElement("mRootNode", ToolkitSettings.XUK_NS);
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