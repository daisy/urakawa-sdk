package org.daisy.urakawa;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.command.CommandFactory;
import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.core.TreeNodeHasParentException;
import org.daisy.urakawa.core.visitor.examples.CollectManagedMediaTreeNodeVisitor;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.IEventListener;
import org.daisy.urakawa.event.LanguageChangedEvent;
import org.daisy.urakawa.event.presentation.MetadataAddedEvent;
import org.daisy.urakawa.event.presentation.MetadataRemovedEvent;
import org.daisy.urakawa.event.presentation.RootNodeChangedEvent;
import org.daisy.urakawa.event.presentation.RootUriChangedEvent;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.IDataProvider;
import org.daisy.urakawa.media.data.IDataProviderManager;
import org.daisy.urakawa.media.data.IManagedMedia;
import org.daisy.urakawa.media.data.IMediaData;
import org.daisy.urakawa.media.data.IMediaDataManager;
import org.daisy.urakawa.media.data.InputStreamIsOpenException;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.media.data.OutputStreamIsOpenException;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;
import org.daisy.urakawa.metadata.IMetadata;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.property.IProperty;
import org.daisy.urakawa.property.PropertyFactory;
import org.daisy.urakawa.property.channel.ChannelDoesNotExistException;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.property.channel.IChannel;
import org.daisy.urakawa.property.channel.IChannelsManager;
import org.daisy.urakawa.property.channel.IChannelsProperty;
import org.daisy.urakawa.undo.IUndoRedoManager;
import org.daisy.urakawa.undo.UndoRedoManager;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class Presentation extends WithPresentation implements IPresentation {
	/**
	 * This interface is used internally for the purpose of the Java
	 * implementation only. It basically fulfills the role of a function
	 * delegate, Java-style (with an anonymous inline class).
	 */
	public interface IXukAbleCreator {
		/**
		 * @param localName
		 * @param namespace
		 * @return bla
		 */
		public IXukAble createXukAble(String localName, String namespace);
	}

	/**
	 * This interface is used internally for the purpose of the Java
	 * implementation only. It basically fulfills the role of a function
	 * delegate, Java-style (with an anonymous inline class).
	 */
	public interface IXukAbleSetter {
		/**
		 * @param xuk
		 */
		public void setXukAble(IXukAble xuk);
	}

	private IProject mProject;

	private TreeNodeFactory mTreeNodeFactory;
	private PropertyFactory mPropertyFactory;
	private ChannelFactory mChannelFactory;
	private MediaFactory mMediaFactory;
	private CommandFactory mCommandFactory;
	private MediaDataFactory mMediaDataFactory;
	private DataProviderFactory mDataProviderFactory;
	private MetadataFactory mMetadataFactory;

	private IChannelsManager mChannelsManager;

	private IMediaDataManager mMediaDataManager;

	private IDataProviderManager mDataProviderManager;

	private IUndoRedoManager mUndoRedoManager;

	private ITreeNode mRootNode;
	private boolean mRootNodeInitialized;
	private URI mRootUri;
	private String mLanguage;
	private List<IMetadata> mMetadata;

	// The 3 event bus below handle events related to language, URI and
	// root-node change events for this IPresentation.
	// Please note that this class automatically adds a listener for the
	// mRootNodeChangedEventNotifier event bus,
	// in order to handle the (de)registration of a special listener
	// (mBubbleEventListener) which
	// forwards the bubbling events from the tree of TreeNodes. See comment for
	// mBubbleEventListener.
	protected IEventHandler<Event> mLanguageChangedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mRootUriChangedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mRootNodeChangedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mMetadataAddedEventNotifier = new EventHandler();
	protected IEventHandler<Event> mMetadataRemovedEventNotifier = new EventHandler();
	// This event bus receives all the events that are raised from within the
	// Data Model of the underlying objects that make this IPresentation (i.e.
	// the tree of TreeNodes), including the above built-in events. The IProject
	// which owns this IPresentation will register a listener on this generic
	// event bus, behind the scenes when the IPresentation is added to the
	// project. This is how events are forwarded from this level to the upper
	// IProject level.
	protected IEventHandler<Event> mDataModelEventNotifier = new EventHandler();

	// This "hub" method automatically dispatches the notify() call to the
	// appropriate IEventHandler (either mLanguageChangedEventNotifier,
	// mRootUriChangedEventNotifier, mRootNodeChangedEventNotifier or
	// mDataModelEventNotifier), based on
	// the type of the given event. Please note that the built-in events for
	// this IPresentation
	// (language, URI and root-node change) are passed to the generic
	// mDataModelEventNotifier event bus as well as to their corresponding
	// notifiers.
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (LanguageChangedEvent.class.isAssignableFrom(event.getClass())) {
			mLanguageChangedEventNotifier.notifyListeners(event);
		} else if (RootUriChangedEvent.class.isAssignableFrom(event.getClass())) {
			mRootUriChangedEventNotifier.notifyListeners(event);
		} else if (RootNodeChangedEvent.class
				.isAssignableFrom(event.getClass())) {
			mRootNodeChangedEventNotifier.notifyListeners(event);
		} else if (MetadataAddedEvent.class.isAssignableFrom(event.getClass())) {
			mMetadataAddedEventNotifier.notifyListeners(event);
		} else if (MetadataRemovedEvent.class
				.isAssignableFrom(event.getClass())) {
			mMetadataRemovedEventNotifier.notifyListeners(event);
		}
		mDataModelEventNotifier.notifyListeners(event);
	}

	// This "hub" method automatically dispatches the registerListener() call to
	// the
	// appropriate IEventHandler (either mLanguageChangedEventNotifier,
	// mRootUriChangedEventNotifier, mRootNodeChangedEventNotifier or
	// mDataModelEventNotifier), based on
	// the class type given. Please note that the listeners for language, URI
	// and root-node change are not registered with the generic
	// mDataModelEventNotifier event bus (only to their corresponding
	// notifiers).
	public <K extends DataModelChangedEvent> void registerListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (LanguageChangedEvent.class.isAssignableFrom(klass)) {
			mLanguageChangedEventNotifier.registerListener(listener, klass);
		} else if (RootUriChangedEvent.class.isAssignableFrom(klass)) {
			mRootUriChangedEventNotifier.registerListener(listener, klass);
		} else if (RootNodeChangedEvent.class.isAssignableFrom(klass)) {
			mRootNodeChangedEventNotifier.registerListener(listener, klass);
		} else if (MetadataAddedEvent.class.isAssignableFrom(klass)) {
			mMetadataAddedEventNotifier.registerListener(listener, klass);
		} else if (MetadataRemovedEvent.class.isAssignableFrom(klass)) {
			mMetadataRemovedEventNotifier.registerListener(listener, klass);
		} else {
			mDataModelEventNotifier.registerListener(listener, klass);
		}
	}

	// Same as above, for de-registration.
	public <K extends DataModelChangedEvent> void unregisterListener(
			IEventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		if (LanguageChangedEvent.class.isAssignableFrom(klass)) {
			mLanguageChangedEventNotifier.unregisterListener(listener, klass);
		} else if (RootUriChangedEvent.class.isAssignableFrom(klass)) {
			mRootUriChangedEventNotifier.unregisterListener(listener, klass);
		} else if (RootNodeChangedEvent.class.isAssignableFrom(klass)) {
			mRootNodeChangedEventNotifier.unregisterListener(listener, klass);
		} else if (MetadataAddedEvent.class.isAssignableFrom(klass)) {
			mMetadataAddedEventNotifier.unregisterListener(listener, klass);
		} else if (MetadataRemovedEvent.class.isAssignableFrom(klass)) {
			mMetadataRemovedEventNotifier.unregisterListener(listener, klass);
		} else {
			mDataModelEventNotifier.unregisterListener(listener, klass);
		}
	}

	// This listener receives events that are raised from within the
	// root ITreeNode (entire tree) of this IPresentation.
	// It simply forwards the received event to the main event bus for this
	// IPresentation (which by default has only one registered listener: the
	// IProject, in order to forward the received event onto the IProject's own
	// main
	// event bus).
	// If needed, application programmers should manually register their
	// listeners by calling
	// IPresentation.registerListener(IEventListener<DataModelChangedEvent>,
	// DataModelChangedEvent.class)), or
	// IPresentation.registerListener(IEventListener<ChildAddedEvent>,
	// ChildAddedEvent.class)), or
	// IPresentation.registerListener(IEventListener<MediaDataChangedEvent>,
	// MediaDataChangedEvent.class)), etc.
	protected IEventListener<DataModelChangedEvent> mBubbleEventListener = new IEventListener<DataModelChangedEvent>() {
		public <K extends DataModelChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			notifyListeners(event);
		}
	};
	// This built-in listener takes care of (de)registering the
	// mBubbleEventListener for TreeNodes when the root node of the
	// IPresentation
	// is changed.
	protected IEventListener<RootNodeChangedEvent> mRootNodeChangedEventListener = new IEventListener<RootNodeChangedEvent>() {
		public <K extends RootNodeChangedEvent> void eventCallback(K event)
				throws MethodParameterIsNullException {
			if (event == null) {
				throw new MethodParameterIsNullException();
			}
			if (event.getSourcePresentation() == Presentation.this) {
				ITreeNode node = event.getPreviousRootNode();
				if (node != null) {
					node.unregisterListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
				node = event.getNewRootNode();
				if (node != null) {
					node.registerListener(mBubbleEventListener,
							DataModelChangedEvent.class);
				}
			} else {
				throw new RuntimeException("WFT ??! This should never happen.");
			}
		}
	};

	/**
	 * 
	 */
	public Presentation() {
		mMetadata = new LinkedList<IMetadata>();
		mRootNodeInitialized = false;
		try {
			registerListener(mRootNodeChangedEventListener,
					RootNodeChangedEvent.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public IProject getProject() throws IsNotInitializedException {
		if (mProject == null) {
			throw new IsNotInitializedException();
		}
		return mProject;
	}

	public void setProject(IProject proj)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (proj == null) {
			throw new MethodParameterIsNullException();
		}
		if (mProject != null) {
			throw new IsAlreadyInitializedException();
		}
		mProject = proj;
	}

	public void setLanguage(String lang)
			throws MethodParameterIsEmptyStringException {
		if (lang.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		String prevLang = mLanguage;
		mLanguage = lang;
		if (mLanguage != prevLang)
			try {
				notifyListeners(new LanguageChangedEvent(this, mLanguage,
						prevLang));
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
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
				throw new RuntimeException("WTF ??!", e);
			}
		}
		List<IMediaData> usedMediaData;
		usedMediaData = getUndoRedoManager().getListOfUsedMediaData();
		for (IManagedMedia mm : collectorVisitor.getListOfCollectedMedia()) {
			if (!usedMediaData.contains(mm.getMediaData()))
				usedMediaData.add(mm.getMediaData());
		}
		List<IDataProvider> usedDataProviders = new LinkedList<IDataProvider>();

		for (IMediaData md : getMediaDataManager().getListOfMediaData()) {
			if (usedMediaData.contains(md)) {
				if (md instanceof WavAudioMediaData) {
					((WavAudioMediaData) md).forceSingleDataProvider();
				}
				for (IDataProvider dp : md.getListOfUsedDataProviders()) {
					if (!usedDataProviders.contains(dp))
						usedDataProviders.add(dp);
				}
			} else {
				md.delete();
			}
		}

		for (IDataProvider dp : getDataProviderManager()
				.getListOfDataProviders()) {
			if (!usedDataProviders.contains(dp)) {
				try {
					dp.delete();
				} catch (OutputStreamIsOpenException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				} catch (InputStreamIsOpenException e) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e);
				}
			}
		}

	}

	public ITreeNode getRootNode() {
		if (!mRootNodeInitialized) {
			try {
				setRootNode(getTreeNodeFactory().create());
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

	public void setRootNode(ITreeNode newRoot)
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
			try {
				ITreeNode prevRoot = mRootNode;
				mRootNode = newRoot;
				mRootNodeChangedEventNotifier
						.notifyListeners(new RootNodeChangedEvent(this,
								mRootNode, prevRoot));
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public TreeNodeFactory getTreeNodeFactory() {
		if (mTreeNodeFactory == null) {
			try {
				setTreeNodeFactory(new TreeNodeFactory());
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

	private void setTreeNodeFactory(TreeNodeFactory factory)
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

	public PropertyFactory getPropertyFactory() {
		if (mPropertyFactory == null) {
			try {
				setPropertyFactory(new PropertyFactory());
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

	private void setPropertyFactory(PropertyFactory factory)
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

	public IUndoRedoManager getUndoRedoManager() {
		if (mUndoRedoManager == null) {
			try {
				setUndoRedoManager(new UndoRedoManager());
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

	private void setUndoRedoManager(IUndoRedoManager man)
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
		man.registerListener(mBubbleEventListener, DataModelChangedEvent.class);
	}

	public CommandFactory getCommandFactory() {
		if (mCommandFactory == null) {
			try {
				setCommandFactory(new CommandFactory());
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

	private void setCommandFactory(CommandFactory factory)
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

	public MediaFactory getMediaFactory() {
		if (mMediaFactory == null) {
			try {
				setMediaFactory(new MediaFactory());
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

	private void setMediaFactory(MediaFactory factory)
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

	public void setRootURI(URI newRootUri)
			throws MethodParameterIsNullException, URISyntaxException {
		if (newRootUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (!newRootUri.isAbsolute()) {
			// TODO fix URI
			new URI("123www");
		}
		URI prev = mRootUri;
		mRootUri = newRootUri;
		if (mRootUri != prev) {
			notifyListeners(new RootUriChangedEvent(this, mRootUri, prev));
		}
	}

	public List<IMedia> getListOfMediaUsedByTreeNode(ITreeNode node)
			throws MethodParameterIsNullException {
		if (node == null) {
			throw new MethodParameterIsNullException();
		}
		List<IMedia> res = new LinkedList<IMedia>();
		for (IProperty prop : node.getListOfProperties()) {
			if (prop instanceof IChannelsProperty) {
				IChannelsProperty chProp = (IChannelsProperty) prop;
				for (IChannel ch : chProp.getListOfUsedChannels()) {
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

	private void collectUsedMedia(ITreeNode node, List<IMedia> collectedMedia) {
		try {
			for (IMedia m : getListOfMediaUsedByTreeNode(node)) {
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

	public List<IMedia> getListOfUsedMedia() {
		List<IMedia> res = new LinkedList<IMedia>();
		if (getRootNode() != null) {
			collectUsedMedia(getRootNode(), res);
		}
		return res;
	}

	public ChannelFactory getChannelFactory() {
		if (mChannelFactory == null) {
			try {
				setChannelFactory(new ChannelFactory());
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

	private void setChannelFactory(ChannelFactory factory)
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

	public IChannelsManager getChannelsManager() {
		if (mChannelsManager == null) {
			try {
				setChannelsManager(new ChannelsManager());
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

	private void setChannelsManager(IChannelsManager man)
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

	public IMediaDataManager getMediaDataManager() {
		if (mMediaDataManager == null) {
			try {
				setMediaDataManager(new MediaDataManager());
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

	private void setMediaDataManager(IMediaDataManager man)
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

	public MediaDataFactory getMediaDataFactory() {
		if (mMediaDataFactory == null) {
			try {
				setMediaDataFactory(new MediaDataFactory());
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

	private void setMediaDataFactory(MediaDataFactory factory)
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

	public IDataProviderManager getDataProviderManager() {
		if (mDataProviderManager == null) {
			try {
				setDataProviderManager(new DataProviderManager());
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

	private void setDataProviderManager(IDataProviderManager man)
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

	public DataProviderFactory getDataProviderFactory() {
		if (mDataProviderFactory == null) {
			try {
				setDataProviderFactory(new DataProviderFactory());
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

	private void setDataProviderFactory(DataProviderFactory factory)
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

	public MetadataFactory getMetadataFactory() {
		if (mMetadataFactory == null) {
			try {
				setMetadataFactory(new MetadataFactory());
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

	private void setMetadataFactory(MetadataFactory factory)
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

	public void addMetadata(IMetadata iMetadata)
			throws MethodParameterIsNullException {
		if (iMetadata == null) {
			throw new MethodParameterIsNullException();
		}
		mMetadata.add(iMetadata);
		try {
			mMetadataAddedEventNotifier.notifyListeners(new MetadataAddedEvent(
					this, iMetadata));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public List<IMetadata> getListOfMetadata() {
		return new LinkedList<IMetadata>(mMetadata);
	}

	public List<IMetadata> getListOfMetadata(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (name == null) {
			throw new MethodParameterIsNullException();
		}
		if (name.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		List<IMetadata> list = new LinkedList<IMetadata>();
		for (IMetadata md : mMetadata) {
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
		for (IMetadata md : getListOfMetadata(name)) {
			try {
				deleteMetadata(md);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public void deleteMetadata(IMetadata iMetadata)
			throws MethodParameterIsNullException {
		if (iMetadata == null) {
			throw new MethodParameterIsNullException();
		}
		mMetadata.remove(iMetadata);
		try {
			mMetadataRemovedEventNotifier
					.notifyListeners(new MetadataRemovedEvent(this, iMetadata));
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
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
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException {
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
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
			if (lang.length() == 0) {
				lang = null;
			}
		}

		try {
			setLanguage(lang);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		// super.xukInAttributes(source);
	}

	/*
	 * protected void xukInXukAbleFromChild(IXmlDataReader source, IXukAble
	 * iXukAble, IProgressHandler ph) throws XukDeserializationFailedException,
	 * ProgressCancelledException { if (ph != null && ph.notifyProgress()) {
	 * throw new ProgressCancelledException(); } if (!source.isEmptyElement()) {
	 * while (source.read()) { if (source.getNodeType() ==
	 * IXmlDataReader.ELEMENT) { if (source.getLocalName() ==
	 * iXukAble.getXukLocalName() && source.getNamespaceURI() == iXukAble
	 * .getXukNamespaceURI()) { try { iXukAble.xukIn(source, ph); } catch
	 * (MethodParameterIsNullException e) { // Should never happen throw new
	 * RuntimeException("WTF ??!", e); } } else { try { super.xukInChild(source,
	 * ph); } catch (MethodParameterIsNullException e) { // Should never happen
	 * throw new RuntimeException("WTF ??!", e); } } } else if
	 * (source.getNodeType() == IXmlDataReader.END_ELEMENT) { break; } if
	 * (source.isEOF()) { throw new XukDeserializationFailedException(); } } } }
	 */

	private void xukInMetadata(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (source.isEmptyElement())
			return;
		while (source.read()) {
			if (source.getNodeType() == IXmlDataReader.ELEMENT) {
				IMetadata newMeta = null;
				try {
					newMeta = getMetadataFactory().create(
							source.getLocalName(), source.getNamespaceURI());
				} catch (MethodParameterIsNullException e1) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e1);
				} catch (MethodParameterIsEmptyStringException e1) {
					// Should never happen
					throw new RuntimeException("WTF ??!", e1);
				}
				if (newMeta != null) {
					mMetadata.add(newMeta);
					try {
						newMeta.xukIn(source, ph);
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				} else {
					try {
						super.xukInChild(source, ph);
					} catch (MethodParameterIsNullException e) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e);
					}
				}
			} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
				break;
			}
			if (source.isEOF()) {
				throw new XukDeserializationFailedException();
			}
		}
	}

	protected void xukInRootNode(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
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
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					ITreeNode newRoot;
					try {
						newRoot = getTreeNodeFactory()
								.create(source.getLocalName(),
										source.getNamespaceURI());
					} catch (MethodParameterIsNullException e1) {
						// Should never happen
						throw new RuntimeException("WTF ??!", e1);
					} catch (MethodParameterIsEmptyStringException e1) {
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
							newRoot.xukIn(source, ph);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					} else {
						try {
							super.xukInChild(source, ph);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF()) {
					throw new XukDeserializationFailedException();
				}
			}
		}
	}

	@Override
	public void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		try {
			// super.xukOutChildren(destination, baseUri);

			getTreeNodeFactory().xukOut(destination, baseUri, ph);
			getPropertyFactory().xukOut(destination, baseUri, ph);

			destination.writeStartElement("ChannelFactory", IXukAble.XUK_NS);
			getChannelFactory().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("mChannelsManager", IXukAble.XUK_NS);
			getChannelsManager().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("MediaFactory", IXukAble.XUK_NS);
			getMediaFactory().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("DataProviderFactory",
					IXukAble.XUK_NS);
			getDataProviderFactory().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("mDataProviderManager",
					IXukAble.XUK_NS);
			getDataProviderManager().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("MediaDataFactory", IXukAble.XUK_NS);
			getMediaDataFactory().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("mMediaDataManager", IXukAble.XUK_NS);
			getMediaDataManager().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("CommandFactory", IXukAble.XUK_NS);
			getCommandFactory().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("mUndoRedoManager", IXukAble.XUK_NS);
			getUndoRedoManager().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("MetadataFactory", IXukAble.XUK_NS);
			getMetadataFactory().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
			destination.writeStartElement("mMetadata", IXukAble.XUK_NS);
			for (IMetadata md : mMetadata) {
				md.xukOut(destination, baseUri, ph);
			}
			destination.writeEndElement();
			destination.writeStartElement("mRootNode", IXukAble.XUK_NS);
			getRootNode().xukOut(destination, baseUri, ph);
			destination.writeEndElement();
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@SuppressWarnings("unused")
	@Override
	public void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
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

	private void xukInXukAbleFromChild(IXmlDataReader source,
			IXukAbleCreator creator, IXukAbleSetter setter, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException {
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		if (!source.isEmptyElement()) {
			boolean foundObj = false;
			while (source.read()) {
				if (source.getNodeType() == IXmlDataReader.ELEMENT) {
					if (foundObj) {
						try {
							super.xukInChild(source, ph);
						} catch (MethodParameterIsNullException e) {
							// Should never happen
							throw new RuntimeException("WTF ??!", e);
						}
					} else {
						IXukAble xuk = creator.createXukAble(source
								.getLocalName(), source.getNamespaceURI());
						if (xuk != null) {
							setter.setXukAble(xuk);
							foundObj = true;
							try {
								xuk.xukIn(source, ph);
							} catch (MethodParameterIsNullException e) {
								// Should never happen
								throw new RuntimeException("WTF ??!", e);
							}
						} else {
							try {
								super.xukInChild(source, ph);
							} catch (MethodParameterIsNullException e) {
								// Should never happen
								throw new RuntimeException("WTF ??!", e);
							}
						}
					}
				} else if (source.getNodeType() == IXmlDataReader.END_ELEMENT) {
					break;
				}
				if (source.isEOF())
					throw new XukDeserializationFailedException();
			}
		}
	}

	@Override
	public void xukInChild(IXmlDataReader source, IProgressHandler ph)
			throws XukDeserializationFailedException,
			ProgressCancelledException, MethodParameterIsNullException {
		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		boolean readItem = false;
		if (source.getNamespaceURI() == IXukAble.XUK_NS) {
			readItem = true;
			String str = source.getLocalName();
			if (str == "TreeNodeFactory") {
				getTreeNodeFactory().xukIn(source, ph);

			} else if (str == "PropertyFactory") {
				getPropertyFactory().xukIn(source, ph);

			} else if (str == "ChannelFactory") {
				getChannelFactory().xukIn(source, ph);

			} else if (str == "ChannelsManager") {
				xukInXukAbleFromChild(source, new IXukAbleCreator() {
					@SuppressWarnings("unused")
					public IXukAble createXukAble(String localName,
							String namespace) {

						return getChannelsManager();

					}
				}, new IXukAbleSetter() {
					@SuppressWarnings("unused")
					public void setXukAble(IXukAble xuk) {
						/**
						 * Does nothing.
						 */
					}
				}, ph);
			} else if (str == "MediaFactory") {
				getMediaFactory().xukIn(source, ph);

			} else if (str == "mMediaDataManager") {
				xukInXukAbleFromChild(source, new IXukAbleCreator() {
					@SuppressWarnings("unused")
					public IXukAble createXukAble(String localName,
							String namespace) {
						return getMediaDataManager();
					}
				}, new IXukAbleSetter() {
					@SuppressWarnings("unused")
					public void setXukAble(IXukAble xuk) {
						/**
						 * Does nothing.
						 */
					}
				}, ph);
			} else if (str == "MediaDataFactory") {
				getMediaDataFactory().xukIn(source, ph);

			} else if (str == "mDataProviderManager") {
				xukInXukAbleFromChild(source, new IXukAbleCreator() {
					@SuppressWarnings("unused")
					public IXukAble createXukAble(String localName,
							String namespace) {
						return getDataProviderManager();
					}
				}, new IXukAbleSetter() {
					@SuppressWarnings("unused")
					public void setXukAble(IXukAble xuk) {
						/**
						 * Does nothing.
						 */
					}
				}, ph);
			} else if (str == "DataProviderFactory") {
				getDataProviderFactory().xukIn(source, ph);

			} else if (str == "mUndoRedoManager") {
				xukInXukAbleFromChild(source, new IXukAbleCreator() {
					@SuppressWarnings("unused")
					public IXukAble createXukAble(String localName,
							String namespace) {
						return getUndoRedoManager();

					}
				}, new IXukAbleSetter() {
					@SuppressWarnings("unused")
					public void setXukAble(IXukAble xuk) {
						/**
						 * Does nothing.
						 */
					}
				}, ph);
			} else if (str == "CommandFactory") {
				getCommandFactory().xukIn(source, ph);

			} else if (str == "MetadataFactory") {
				getMetadataFactory().xukIn(source, ph);

			} else if (str == "mMetadata") {
				xukInMetadata(source, ph);
			} else if (str == "mRootNode") {
				xukInRootNode(source, ph);
			} else {
				readItem = false;
			}
		}
		if (!readItem) {
			try {
				super.xukInChild(source, ph);
			} catch (MethodParameterIsNullException e) {
				// Should never happen
				throw new RuntimeException("WTF ??!", e);
			}
		}
	}

	public boolean ValueEquals(IPresentation other)
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
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		List<IMetadata> thisMetadata = getListOfMetadata();
		List<IMetadata> otherMetadata = other.getListOfMetadata();
		if (thisMetadata.size() != otherMetadata.size())
			return false;
		for (IMetadata m : thisMetadata) {
			boolean found = false;
			try {
				for (IMetadata om : other.getListOfMetadata(m.getName())) {
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
