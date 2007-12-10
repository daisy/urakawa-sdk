package org.daisy.urakawa;

import java.net.URI;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeHasParentException;
import org.daisy.urakawa.core.TreeNodeIsInDifferentPresentationException;
import org.daisy.urakawa.core.command.TreeNodeInsert;
import org.daisy.urakawa.core.event.TreeNodeAddedEvent;
import org.daisy.urakawa.core.event.TreeNodeAddedRemovedListener;
import org.daisy.urakawa.core.event.TreeNodeChangedEvent;
import org.daisy.urakawa.core.event.TreeNodeChangedListener;
import org.daisy.urakawa.core.event.TreeNodeRemovedEvent;
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
import org.daisy.urakawa.property.GenericPropertyFactory;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.property.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.XmlPropertyFactory;
import org.daisy.urakawa.undo.CommandFactory;
import org.daisy.urakawa.undo.UndoRedoManager;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class PresentationImpl implements Presentation {
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

	public TreeNodeFactory getTreeNodeFactory() throws IsNotInitializedException {
		if (mTreeNodeFactory == null) {
			try {
				setTreeNodeFactory(getDataModelFactory().createTreeNodeFactory());
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
}
