package org.daisy.urakawa;

import java.net.MalformedURLException;
import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeHasParentException;
import org.daisy.urakawa.core.TreeNodeIsInDifferentPresentationException;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProvider;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.ManagedMedia;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;
import org.daisy.urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor;
import org.daisy.urakawa.metadata.Metadata;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.channel.Channel;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.undo.CommandFactory;
import org.daisy.urakawa.undo.UndoRedoManager;
import org.daisy.urakawa.xuk.XukAbleImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PresentationImpl extends XukAbleImpl implements Presentation {
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
	private TreeNode mRootNode;
	private boolean mRootNodeInitialized;
	private URI mRootUri;
	private String mLanguage;
	private List<Metadata> mMetadata;
	private MetadataFactory mMetadataFactory;

	/**
	 * 
	 */
	public PresentationImpl() {
		mMetadata = new LinkedList<Metadata>();
		mRootNodeInitialized = false;
		// TODO: add events
		/*
		 * this.languageChanged += new EventHandler<LanguageChangedEventArgs>(
		 * this_languageChanged); this.rootUriChanged += new EventHandler<RootUriChangedEventArgs>(
		 * this_rootUriChanged); this.rootNodeChanged += new EventHandler<RootNodeChangedEventArgs>(
		 * this_rootNodeChanged);
		 */
	}

	public Project getProject() throws IsNotInitializedException {
		if (mProject == null) {
			throw new IsNotInitializedException();
		}
		return mProject;
	}

	public void setProject(Project proj) throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (proj == null) {
			throw new MethodParameterIsNullException();
		}
		if (mProject != null) {
			throw new IsAlreadyInitializedException();
		}
		mProject = proj;
	}

	public DataModelFactory getDataModelFactory()
			throws IsNotInitializedException {
		return getProject().getDataModelFactory();
	}

	public void setLanguage(String lang)
			throws MethodParameterIsEmptyStringException {
		if (lang == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		// String prevLang = mLanguage;
		mLanguage = lang;
		// TODO: add event notification
		// if (mLanguage != prevLang) notifyLanguageChanged(this, mLanguage,
		// prevLang);
	}

	public String getLanguage() {
		return mLanguage;
	}

	public void cleanup() {
		CollectManagedMediaTreeNodeVisitor collectorVisitor = new CollectManagedMediaTreeNodeVisitor();
		if (getRootNode() != null) {
			try {
				getRootNode().acceptDepthFirst(collectorVisitor);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		List<MediaData> usedMediaData = getUndoRedoManager()
				.getListOfUsedMediaData();
		for (ManagedMedia mm : collectorVisitor.getListOfCollectedMedia()) {
			if (!usedMediaData.contains(mm.getMediaData()))
				usedMediaData.add(mm.getMediaData());
		}
		List<DataProvider> usedDataProviders = new LinkedList<DataProvider>();
		for (MediaData md : (List<MediaData>) getMediaDataManager()
				.getListOfMediaData()) {
			if (usedMediaData.contains(md)) {
				if (md instanceof WavAudioMediaData) {
					((WavAudioMediaData) md).forceSingleDataProvider();
				}
				for (DataProvider dp : (List<DataProvider>) md
						.getListOfUsedDataProviders()) {
					if (!usedDataProviders.contains(dp))
						usedDataProviders.add(dp);
				}
			} else {
				md.delete();
			}
		}
		for (DataProvider dp : (List<DataProvider>) getDataProviderManager()
				.getListOfDataProviders()) {
			if (!usedDataProviders.contains(dp)) {
				dp.delete();
			}
		}
	}

	public TreeNode getRootNode() {
		if (!mRootNodeInitialized) {
			try {
				setRootNode(getTreeNodeFactory().createNode());
			} catch (TreeNodeHasParentException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (TreeNodeIsInDifferentPresentationException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mRootNode;
	}

	public void setRootNode(TreeNode newRoot)
			throws TreeNodeHasParentException,
			TreeNodeIsInDifferentPresentationException,
			IsNotInitializedException {
		if (newRoot != null) {
			if (newRoot.getParent() != null) {
				throw new TreeNodeHasParentException();
			}
			if (newRoot.getPresentation() != this) {
				throw new TreeNodeIsInDifferentPresentationException();
			}
			mRootNodeInitialized = true;
		}
		if (newRoot != mRootNode) {
			// TODO: add events and notification
			/*
			 * TreeNode prevRoot = mRootNode; if (prevRoot != null) {
			 * prevRoot.changed -= new EventHandler<DataModelChangedEventArgs>(
			 * rootNode_changed);
			 */
			mRootNode = newRoot;
			/*
			 * if (mRootNode != null) { mRootNode.changed += new EventHandler<DataModelChangedEventArgs>(
			 * rootNode_changed); } notifyRootNodeChanged(this, mRootNode,
			 * prevRoot);
			 */
		}
	}

	public TreeNodeFactory getTreeNodeFactory()
			throws IsNotInitializedException {
		if (mTreeNodeFactory == null) {
			try {
				setTreeNodeFactory(getDataModelFactory()
						.createTreeNodeFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mTreeNodeFactory;
	}

	public void setTreeNodeFactory(TreeNodeFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mTreeNodeFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mTreeNodeFactory = factory;
		mTreeNodeFactory.setPresentation(this);
	}

	public PropertyFactory getPropertyFactory()
			throws IsNotInitializedException {
		if (mPropertyFactory == null) {
			try {
				setPropertyFactory(getDataModelFactory()
						.createPropertyFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mPropertyFactory;
	}

	public void setPropertyFactory(PropertyFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mPropertyFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mPropertyFactory = factory;
		mPropertyFactory.setPresentation(this);
	}

	public UndoRedoManager getUndoRedoManager()
			throws IsNotInitializedException {
		if (mUndoRedoManager == null) {
			try {
				setUndoRedoManager(getDataModelFactory()
						.createUndoRedoManager());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mUndoRedoManager;
	}

	public void setUndoRedoManager(UndoRedoManager man)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (man == null) {
			throw new MethodParameterIsNullException();
		}
		if (mUndoRedoManager != null) {
			throw new IsAlreadyInitializedException();
		}
		mUndoRedoManager = man;
		mUndoRedoManager.setPresentation(this);
	}

	public CommandFactory getCommandFactory() throws IsNotInitializedException {
		if (mCommandFactory == null) {
			try {
				setCommandFactory(getDataModelFactory().createCommandFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mCommandFactory;
	}

	public void setCommandFactory(CommandFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mCommandFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mCommandFactory = factory;
		mCommandFactory.setPresentation(this);
	}

	public MediaFactory getMediaFactory() throws IsNotInitializedException {
		if (mMediaFactory == null) {
			try {
				setMediaFactory(getDataModelFactory().createMediaFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mMediaFactory;
	}

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mMediaFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mMediaFactory = factory;
		mMediaFactory.setPresentation(this);
	}

	public URI getRootURI() {
		if (mRootUri == null) {
			// TODO: use a proper default URI (based on ClassLoader ?)
			mRootUri = new URI("file://TODO");
		}
		return mRootUri;
	}

	public void setRootURI(URI newRootUri)
			throws MethodParameterIsNullException, MalformedURLException {
		if (newRootUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (!newRootUri.isAbsolute()) {
			throw new MalformedURLException();
		}
		URI prev = mRootUri;
		mRootUri = newRootUri;
		if (mRootUri != prev) {
			// TODO: add event notification
			// notifyRootUriChanged(this, mRootUri, prev);
		}
	}

	public List<Media> getListOfMediaUsedByTreeNode(TreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		List<Media> res = new LinkedList<Media>();
		for (Property prop : (List<Property>) node.getListOfProperties()) {
			if (prop instanceof ChannelsProperty) {
				ChannelsProperty chProp = (ChannelsProperty) prop;
				for (Channel ch : chProp.getListOfUsedChannels()) {
					res.add(chProp.getMedia(ch));
				}
			}
		}
		return res;
	}

	private void collectUsedMedia(TreeNode node, List<Media> collectedMedia) {
		for (Media m : getListOfMediaUsedByTreeNode(node)) {
			if (!collectedMedia.contains(m))
				collectedMedia.add(m);
		}
		for (int i = 0; i < node.getChildCount(); i++) {
			collectUsedMedia(node.getChild(i), collectedMedia);
		}
	}

	public List<Media> getListOfUsedMedia() {
		List<Media> res = new LinkedList<Media>();
		if (getRootNode() != null) {
			collectUsedMedia(getRootNode(), res);
		}
		return res;
	}

	public ChannelFactory getChannelFactory() throws IsNotInitializedException {
		if (mChannelFactory == null) {
			try {
				setChannelFactory(getDataModelFactory().createChannelFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mChannelFactory;
	}

	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mChannelFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mChannelFactory = factory;
		mChannelFactory.setPresentation(this);
	}

	public ChannelsManager getChannelsManager()
			throws IsNotInitializedException {
		if (mChannelsManager == null) {
			try {
				setChannelsManager(getDataModelFactory()
						.createChannelsManager());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mChannelsManager;
	}

	public void setChannelsManager(ChannelsManager man)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (man == null) {
			throw new MethodParameterIsNullException();
		}
		if (mChannelsManager != null) {
			throw new IsAlreadyInitializedException();
		}
		mChannelsManager = man;
		mChannelsManager.setPresentation(this);
	}

	public MediaDataManager getMediaDataManager()
			throws IsNotInitializedException {
		if (mMediaDataManager == null) {
			try {
				setMediaDataManager(getDataModelFactory()
						.createMediaDataManager());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mMediaDataManager;
	}

	public void setMediaDataManager(MediaDataManager man)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (man == null) {
			throw new MethodParameterIsNullException();
		}
		if (mMediaDataManager != null) {
			throw new IsAlreadyInitializedException();
		}
		mMediaDataManager = man;
		mMediaDataManager.setPresentation(this);
	}

	public MediaDataFactory getMediaDataFactory()
			throws IsNotInitializedException {
		if (mMediaDataFactory == null) {
			try {
				setMediaDataFactory(getDataModelFactory()
						.createMediaDataFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mMediaDataFactory;
	}

	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mMediaDataFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mMediaDataFactory = factory;
		mMediaDataFactory.setPresentation(this);
	}

	public DataProviderManager getDataProviderManager()
			throws IsNotInitializedException {
		if (mDataProviderManager == null) {
			try {
				setDataProviderManager(getDataModelFactory()
						.createDataProviderManager());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mDataProviderManager;
	}

	public void setDataProviderManager(DataProviderManager man)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (man == null) {
			throw new MethodParameterIsNullException();
		}
		if (mDataProviderManager != null) {
			throw new IsAlreadyInitializedException();
		}
		mDataProviderManager = man;
		mDataProviderManager.setPresentation(this);
	}

	public DataProviderFactory getDataProviderFactory()
			throws IsNotInitializedException {
		if (mDataProviderFactory == null) {
			try {
				setDataProviderFactory(getDataModelFactory()
						.createDataProviderFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mDataProviderFactory;
	}

	public void setDataProviderFactory(DataProviderFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mDataProviderFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mDataProviderFactory = factory;
		mDataProviderFactory.setPresentation(this);
	}

	public MetadataFactory getMetadataFactory()
			throws IsNotInitializedException {
		if (mMetadataFactory == null) {
			try {
				setMetadataFactory(getDataModelFactory()
						.createMetadataFactory());
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
		return mMetadataFactory;
	}

	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (factory == null) {
			throw new MethodParameterIsNullException();
		}
		if (mMetadataFactory != null) {
			throw new IsAlreadyInitializedException();
		}
		mMetadataFactory = factory;
		mMetadataFactory.setPresentation(this);
	}

	public void addMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
		if (metadata == null) {
			throw new MethodParameterIsNullException();
		}
		mMetadata.add(metadata);
	}

	public List<Metadata> getListOfMetadata() {
		return new LinkedList<Metadata>(mMetadata);
	}

	public List<Metadata> getListOfMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (name == null) {
			throw new MethodParameterIsNullException();
		}
		if (name.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		List<Metadata> list = new LinkedList<Metadata>();
		for (Metadata md : mMetadata) {
			if (md.getName() == name)
				list.add(md);
		}
		return list;
	}

	public void deleteMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (name == null) {
			throw new MethodParameterIsNullException();
		}
		if (name.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		for (Metadata md : getListOfMetadata(name)) {
			try {
				deleteMetadata(md);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!");
			}
		}
	}

	public void deleteMetadata(Metadata metadata)
			throws MethodParameterIsNullException {
		if (metadata == null) {
			throw new MethodParameterIsNullException();
		}
		mMetadata.remove(metadata);
	}
}
