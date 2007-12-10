package org.daisy.urakawa;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.TreeNodeHasParentException;
import org.daisy.urakawa.core.TreeNodeIsInDifferentPresentationException;
import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.core.WithTreeNodeFactory;
import org.daisy.urakawa.core.event.TreeNodeChangeManager;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.metadata.WithMetadataFactory;
import org.daisy.urakawa.property.WithGenericPropertyFactory;
import org.daisy.urakawa.property.channel.WithChannelFactory;
import org.daisy.urakawa.property.channel.WithChannelsManager;
import org.daisy.urakawa.property.channel.WithChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.WithXmlPropertyFactory;
import org.daisy.urakawa.undo.WithUndoRedoManager;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is primarily a container for the document tree (made of
 * {@link org.daisy.urakawa.core.TreeNode} nodes), and a host for various
 * associated factories and managers. It is also the central hub for handling
 * tree change events (registering listeners, etc. See
 * {@link org.daisy.urakawa.core.event.TreeNodeChangeManager}). It is also the
 * host for {@link org.daisy.urakawa.metadata}.
 * </p>
 * <p>
 * Implementations should make sure to provide constructors that create a
 * default root node, as
 * {@link org.daisy.urakawa.core.WithTreeNode#getTreeNode()} cannot return NULL.
 * </p>
 * TODO: Add IChangeNotifier
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.PropertyFactory
 * @depend - Aggregation 1 org.daisy.urakawa.Project
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNode
 * @depend - Composition 1 org.daisy.urakawa.property.channel.ChannelsManager
 * @depend - Composition 1 org.daisy.urakawa.property.channel.ChannelFactory
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNodeFactory
 * @depend - "Aggregation\n(subscribed)" 0..n
 *         org.daisy.urakawa.core.event.TreeNodeChangedListener
 * @depend - "Aggregation\n(subscribed)" 0..n
 *         org.daisy.urakawa.core.event.TreeNodeAddedRemovedListener
 * @depend - Composition 1 org.daisy.urakawa.media.data.MediaDataManager
 * @depend - Composition 1 org.daisy.urakawa.media.data.DataProviderManager
 * @depend - Composition 1 org.daisy.urakawa.media.MediaFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.MediaDataFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.DataProviderFactory
 * @depend - Composition 0..n org.daisy.urakawa.metadata.Metadata
 * @depend - Composition 1 org.daisy.urakawa.metadata.MetadataFactory
 * @depend - Composition 0..1 org.daisy.urakawa.undo.UndoRedoManager
 * @stereotype XukAble
 */
public interface Presentation extends WithPropertyFactory, WithTreeNode, WithProject,
		MediaPresentation, TreeNodeChangeManager, WithTreeNodeFactory,
		WithGenericPropertyFactory, WithChannelFactory,
		WithChannelsPropertyFactory, WithChannelsManager,
		WithXmlPropertyFactory, MediaDataPresentation,
		ValueEquatable<Presentation>, WithMetadataFactory, WithMetadata,
		WithLanguage, WithUndoRedoManager, XukAble {
	/**
	 * This method analyzes the content of the data model and other data
	 * structures of the authoring session, in order to determine what MediaData
	 * (and DataProvider) objects are unused, and therefore can be safely delete
	 * from the Managers (MediaDataManager and DataProviderManager). This of
	 * course can potentially remove files from the filesystem, for example in
	 * the case of FileDataProvider.
	 */
	public void cleanup();

	/**
	 * Convenience method that delegates to the Project to obtain the
	 * DataModelFactory.
	 * 
	 * @return the DataModelFactory
	 * @throws IsNotInitializedException
	 *             when the Project reference is not initialized yet.
	 */
	public DataModelFactory getDataModelFactory()
			throws IsNotInitializedException;
}