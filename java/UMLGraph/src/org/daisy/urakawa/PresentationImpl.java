package org.daisy.urakawa;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeHasParentException;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
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
import org.daisy.urakawa.property.PropertyFactory;
import org.daisy.urakawa.property.channel.Channel;
import org.daisy.urakawa.property.channel.ChannelDoesNotExistException;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.undo.CommandFactory;
import org.daisy.urakawa.undo.UndoRedoManager;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukAble;
import org.daisy.urakawa.xuk.XukAbleCreator;
import org.daisy.urakawa.xuk.XukAbleImpl;
import org.daisy.urakawa.xuk.XukAbleSetter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

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

	@SuppressWarnings("unchecked")
	public void cleanup() {
		CollectManagedMediaTreeNodeVisitor collectorVisitor = new CollectManagedMediaTreeNodeVisitor();
		if (getRootNode() != null) {
			try {
				getRootNode().acceptDepthFirst(collectorVisitor);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		List<MediaData> usedMediaData;
		try {
			usedMediaData = getUndoRedoManager().getListOfUsedMediaData();
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		for (ManagedMedia mm : collectorVisitor.getListOfCollectedMedia()) {
			if (!usedMediaData.contains(mm.getMediaData()))
				usedMediaData.add(mm.getMediaData());
		}
		List<DataProvider> usedDataProviders = new LinkedList<DataProvider>();
		try {
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
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		try {
			for (DataProvider dp : (List<DataProvider>) getDataProviderManager()
					.getListOfDataProviders()) {
				if (!usedDataProviders.contains(dp)) {
					dp.delete();
				}
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public TreeNode getRootNode() {
		if (!mRootNodeInitialized) {
			try {
				setRootNode(getTreeNodeFactory().createNode());
			} catch (TreeNodeHasParentException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (ObjectIsInDifferentPresentationException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (IsNotInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return mRootNode;
	}

	public void setRootNode(TreeNode newRoot)
			throws TreeNodeHasParentException,
			ObjectIsInDifferentPresentationException, IsNotInitializedException {
		if (newRoot != null) {
			if (newRoot.getParent() != null) {
				throw new TreeNodeHasParentException();
			}
			if (newRoot.getPresentation() != this) {
				throw new ObjectIsInDifferentPresentationException();
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
			try {
				mRootUri = new URI("file://TODO");
			} catch (URISyntaxException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		return mRootUri;
	}

	@SuppressWarnings("unused")
	public void setRootURI(URI newRootUri)
			throws MethodParameterIsNullException, URISyntaxException {
		if (newRootUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (!newRootUri.isAbsolute()) {
			URI.create("123www");
		}
		URI prev = mRootUri;
		mRootUri = newRootUri;
		if (mRootUri != prev) {
			// TODO: add event notification
			// notifyRootUriChanged(this, mRootUri, prev);
		}
	}

	@SuppressWarnings("unchecked")
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
					try {
						res.add(chProp.getMedia(ch));
					} catch (ChannelDoesNotExistException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
			}
		}
		return res;
	}

	private void collectUsedMedia(TreeNode node, List<Media> collectedMedia) {
		try {
			for (Media m : getListOfMediaUsedByTreeNode(node)) {
				if (!collectedMedia.contains(m))
					collectedMedia.add(m);
			}
		} catch (MethodParameterIsNullException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		}
		for (int i = 0; i < node.getChildCount(); i++) {
			try {
				collectUsedMedia(node.getChild(i), collectedMedia);
			} catch (MethodParameterIsOutOfBoundsException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
			} catch (IsAlreadyInitializedException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
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
				throw new RuntimeException("WTF ??!", e);
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

	@Override
	protected void clear() {
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
		mMetadata.clear();
		// super.clear();
	}

	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws XukDeserializationFailedException {
		String rootUri = source.getAttribute("rootUri");
		// TODO: use real directory
		URI baseUri;
		try {
			baseUri = new URI("file://TODO");
		} catch (URISyntaxException e1) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e1);
		}
		if (source.getBaseURI() != "") {
			try {
				baseUri = new URI(source.getBaseURI());
			} catch (URISyntaxException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
		if (rootUri == null) {
			try {
				setRootURI(baseUri);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (URISyntaxException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		} else {
			try {
				setRootURI(new URI(rootUri));
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (URISyntaxException e) {
				throw new XukDeserializationFailedException();
			}
		}
		String lang = source.getAttribute("language");
		if (lang != null) {
			lang = lang.trim();
		}
		if (lang == "") {
			lang = null;
		}
		try {
			setLanguage(lang);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		// super.xukInAttributes(source);
	}

	protected void xukInXukAbleFromChild(XmlDataReader source, XukAble xukAble)
			throws XukDeserializationFailedException {
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (source.getLocalName() == xukAble.getXukLocalName()
							&& source.getNamespaceURI() == xukAble
									.getXukNamespaceURI()) {
						try {
							xukAble.xukIn(source);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF()) {
					throw new XukDeserializationFailedException();
				}
			}
		}
	}

	private void xukInMetadata(XmlDataReader source)
			throws XukDeserializationFailedException {
		if (source.isEmptyElement())
			return;
		while (source.read()) {
			if (source.getNodeType() == XmlDataReader.ELEMENT) {
				Metadata newMeta = null;
				try {
					newMeta = getMetadataFactory().createMetadata(
							source.getLocalName(), source.getNamespaceURI());
				} catch (MethodParameterIsNullException e1) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e1);
				} catch (MethodParameterIsEmptyStringException e1) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e1);
				} catch (IsNotInitializedException e1) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e1);
				}
				if (newMeta != null) {
					mMetadata.add(newMeta);
					try {
						newMeta.xukIn(source);
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				} else if (!source.isEmptyElement()) {
					// Read past unidentified element
					source.readSubtree().close();
				}
			} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
				break;
			}
			if (source.isEOF()) {
				throw new XukDeserializationFailedException();
			}
		}
	}

	protected void xukInRootNode(XmlDataReader source)
			throws XukDeserializationFailedException {
		try {
			setRootNode(null);
		} catch (TreeNodeHasParentException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (ObjectIsInDifferentPresentationException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (!source.isEmptyElement()) {
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					TreeNode newRoot;
					try {
						newRoot = getTreeNodeFactory()
								.createNode(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsNullException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					} catch (MethodParameterIsEmptyStringException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					} catch (IsNotInitializedException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					}
					if (newRoot != null) {
						try {
							setRootNode(newRoot);
						} catch (TreeNodeHasParentException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (ObjectIsInDifferentPresentationException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
						try {
							newRoot.xukIn(source);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					} else if (!source.isEmptyElement()) {
						source.readSubtree().close();
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF()) {
					throw new XukDeserializationFailedException();
				}
			}
		}
	}

	@Override
	public void xukOutChildren(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException {
		try {
			// super.xukOutChildren(destination, baseUri);
			destination.writeStartElement("mTreeNodeFactory",
					XukAbleImpl.XUK_NS);
			getTreeNodeFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mPropertyFactory",
					XukAbleImpl.XUK_NS);
			getTreeNodeFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination
					.writeStartElement("mChannelFactory", XukAbleImpl.XUK_NS);
			getChannelFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mChannelsManager",
					XukAbleImpl.XUK_NS);
			getChannelsManager().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mMediaFactory", XukAbleImpl.XUK_NS);
			getMediaFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mDataProviderFactory",
					XukAbleImpl.XUK_NS);
			getDataProviderFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mDataProviderManager",
					XukAbleImpl.XUK_NS);
			getDataProviderManager().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mMediaDataFactory",
					XukAbleImpl.XUK_NS);
			getMediaDataFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mMediaDataManager",
					XukAbleImpl.XUK_NS);
			getMediaDataManager().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination
					.writeStartElement("mCommandFactory", XukAbleImpl.XUK_NS);
			getCommandFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mUndoRedoManager",
					XukAbleImpl.XUK_NS);
			getUndoRedoManager().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mMetadataFactory",
					XukAbleImpl.XUK_NS);
			getMetadataFactory().xukOut(destination, baseUri);
			destination.writeEndElement();
			destination.writeStartElement("mMetadata", XukAbleImpl.XUK_NS);
			for (Metadata md : mMetadata) {
				md.xukOut(destination, baseUri);
			}
			destination.writeEndElement();
			destination.writeStartElement("mRootNode", XukAbleImpl.XUK_NS);
			getRootNode().xukOut(destination, baseUri);
			destination.writeEndElement();
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@SuppressWarnings("unused")
	@Override
	public void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws XukSerializationFailedException {
		// base.xukOutAttributes(destination, baseUri);
		if (baseUri == null) {
			destination
					.writeAttributeString("rootUri", getRootURI().toString());
		} else {
			destination.writeAttributeString("rootUri", baseUri.resolve(
					getRootURI()).toString());
		}
		if (getLanguage() != null) {
			destination.writeAttributeString("language", getLanguage());
		}
	}

	private void xukInXukAbleFromChild(XmlDataReader source,
			XukAbleCreator creator, XukAbleSetter setter)
			throws XukDeserializationFailedException {
		if (!source.isEmptyElement()) {
			boolean foundObj = false;
			while (source.read()) {
				if (source.getNodeType() == XmlDataReader.ELEMENT) {
					if (foundObj) {
						if (!source.isEmptyElement()) {
							source.readSubtree().close();
						}
					} else {
						XukAble xuk = creator.createXukAble(source
								.getLocalName(), source.getNamespaceURI());
						if (xuk != null) {
							setter.setXukAble(xuk);
							foundObj = true;
							try {
								xuk.xukIn(source);
							} catch (MethodParameterIsNullException e) {
								// Should never happen
								throw new RuntimeException("WTF ??!", e);
							}
						} else if (!source.isEmptyElement()) {
							source.readSubtree().close();
						}
					}
				} else if (source.getNodeType() == XmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	public void xukInChild(XmlDataReader source)
			throws XukDeserializationFailedException {
		boolean readItem = false;
		if (source.getNamespaceURI() == XukAbleImpl.XUK_NS) {
			readItem = true;
			String str = source.getLocalName();
			if (str == "mTreeNodeFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createTreeNodeFactory(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setTreeNodeFactory((TreeNodeFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mPropertyFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createPropertyFactory(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setPropertyFactory((PropertyFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mChannelFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createChannelFactory(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setChannelFactory((ChannelFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mChannelsManager") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createChannelsManager(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setChannelsManager((ChannelsManager) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mMediaFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createMediaFactory(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setMediaFactory((MediaFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mMediaDataManager") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory()
									.createMediaDataManager(localName,
											namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setMediaDataManager((MediaDataManager) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mMediaDataFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory()
									.createMediaDataFactory(localName,
											namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setMediaDataFactory((MediaDataFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mDataProviderManager") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory()
									.createDataProviderManager(localName,
											namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setDataProviderManager((DataProviderManager) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mDataProviderFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory()
									.createDataProviderFactory(localName,
											namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setDataProviderFactory((DataProviderFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mUndoRedoManager") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createUndoRedoManager(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setUndoRedoManager((UndoRedoManager) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mCommandFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createCommandFactory(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setCommandFactory((CommandFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mMetadataFactory") {
				xukInXukAbleFromChild(source, new XukAbleCreator() {
					public XukAble createXukAble(String localName,
							String namespace) {
						try {
							return getDataModelFactory().createMetadataFactory(
									localName, namespace);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (MethodParameterIsEmptyStringException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsNotInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				}, new XukAbleSetter() {
					public void setXukAble(XukAble xuk) {
						try {
							setMetadataFactory((MetadataFactory) xuk);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						} catch (IsAlreadyInitializedException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				});
			} else if (str == "mMetadata") {
				xukInMetadata(source);
			} else if (str == "mRootNode") {
				xukInRootNode(source);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			// super.xukInChild(source);
		}
	}

	public boolean ValueEquals(Presentation other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		try {
			if (!getChannelsManager().ValueEquals(other.getChannelsManager()))
				return false;
			if (!getDataProviderManager().ValueEquals(
					other.getDataProviderManager()))
				return false;
			if (!getMediaDataManager().ValueEquals(other.getMediaDataManager()))
				return false;
			if (!getRootNode().ValueEquals(other.getRootNode()))
				return false;
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		List<Metadata> thisMetadata = getListOfMetadata();
		List<Metadata> otherMetadata = other.getListOfMetadata();
		if (thisMetadata.size() != otherMetadata.size())
			return false;
		for (Metadata m : thisMetadata) {
			boolean found = false;
			try {
				for (Metadata om : other.getListOfMetadata(m.getName())) {
					if (m.ValueEquals(om))
						found = true;
				}
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			} catch (MethodParameterIsEmptyStringException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
			if (!found)
				return false;
		}
		if (getLanguage() != other.getLanguage())
			return false;
		return true;
	}
}
