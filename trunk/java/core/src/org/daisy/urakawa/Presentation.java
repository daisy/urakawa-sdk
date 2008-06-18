package org.daisy.urakawa;

import org.daisy.urakawa.command.WithCommandFactory;
import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.core.WithTreeNodeFactory;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.media.WithMediaFactory;
import org.daisy.urakawa.media.data.WithDataProviderFactory;
import org.daisy.urakawa.media.data.WithDataProviderManager;
import org.daisy.urakawa.media.data.WithMediaDataFactory;
import org.daisy.urakawa.media.data.WithMediaDataManager;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.metadata.WithMetadataFactory;
import org.daisy.urakawa.property.channel.WithChannelFactory;
import org.daisy.urakawa.property.channel.WithChannelsManager;
import org.daisy.urakawa.undo.WithUndoRedoManager;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is primarily a container for the document tree (made of
 * {@link org.daisy.urakawa.core.TreeNode} nodes), and a host for various
 * associated factories and managers. It is also the
 * host for {@link org.daisy.urakawa.metadata}.
 * </p>
 * <p>
 * Implementations should make sure to provide constructors that create a
 * default root node, as
 * {@link org.daisy.urakawa.core.WithTreeNode#getRootNode()} cannot return NULL.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.property.PropertyFactory
 * @depend - Aggregation 1 org.daisy.urakawa.Project
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNode
 * @depend - Composition 1 org.daisy.urakawa.property.channel.ChannelsManager
 * @depend - Composition 1 org.daisy.urakawa.property.channel.ChannelFactory
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNodeFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.MediaDataManager
 * @depend - Composition 1 org.daisy.urakawa.media.data.DataProviderManager
 * @depend - Composition 1 org.daisy.urakawa.media.MediaFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.MediaDataFactory
 * @depend - Composition 1 org.daisy.urakawa.undo.CommandFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.DataProviderFactory
 * @depend - Composition 0..n org.daisy.urakawa.metadata.Metadata
 * @depend - Composition 1 org.daisy.urakawa.metadata.MetadataFactory
 * @depend - Composition 1 org.daisy.urakawa.undo.UndoRedoManager
 * @stereotype XukAble
 */
public interface Presentation extends WithRootURI, WithPropertyFactory,
		WithMediaFactory, WithMediaDataFactory, WithCommandFactory,
		WithTreeNode, WithProject, MediaPresentation, WithTreeNodeFactory,
		WithChannelFactory, WithChannelsManager, WithMediaDataManager,
		WithDataProviderFactory, WithDataProviderManager,
		ValueEquatable<Presentation>, WithMetadataFactory, WithMetadata,
		WithLanguage, WithUndoRedoManager, XukAble,
		EventHandler<DataModelChangedEvent> {
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