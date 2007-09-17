package org.daisy.urakawa;

import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.core.WithTreeNodeFactory;
import org.daisy.urakawa.core.event.TreeNodeChangeManager;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.metadata.WithMetadataFactory;
import org.daisy.urakawa.property.WithGenericPropertyFactory;
import org.daisy.urakawa.property.channel.WithChannelFactory;
import org.daisy.urakawa.property.channel.WithChannelsManager;
import org.daisy.urakawa.property.channel.WithChannelsPropertyFactory;
import org.daisy.urakawa.property.xml.WithXmlPropertyFactory;
import org.daisy.urakawa.undo.UndoRedoTransactionIsNotEndedException;
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
 * By default, a Presentation does not support Undo-Redo. Developers must call
 * the {@link org.daisy.urakawa.Presentation#enableUndoRedo()} method to
 * initialize the {@link org.daisy.urakawa.undo.UndoRedoManager}.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.PropertyFactory
 * @depend - Aggregation 1 org.daisy.urakawa.Project
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNode
 * @depend - Composition 1 org.daisy.urakawa.properties.channel.ChannelsManager
 * @depend - Composition 1 org.daisy.urakawa.properties.channel.ChannelFactory
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNodeFactory
 * @depend - "Aggregation\n(subscribed)" 0..n org.daisy.urakawa.core.event.TreeNodeChangedListener
 * @depend - "Aggregation\n(subscribed)" 0..n org.daisy.urakawa.core.event.TreeNodeAddedRemovedListener
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
public interface Presentation extends WithPropertyFactory, WithProject,
		MediaPresentation, TreeNodeChangeManager, WithTreeNode,
		WithTreeNodeFactory, WithGenericPropertyFactory, WithChannelFactory,
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
	 * <p>
	 * Enables support for undo-redo
	 * </p>
	 * <p>
	 * This method initializes a new
	 * {@link org.daisy.urakawa.undo.UndoRedoManager} with an empty stack
	 * history. If there is currently already an active one, this method does
	 * nothing.
	 * </p>
	 * <p>
	 * When undo-redo is enabled, calls to methods that modify the data model
	 * create a command (or more than one) and execute it via the
	 * UndoRedoManager instead of directly modifying the data. There are
	 * built-in commands in the SDK, for example all the
	 * {@link org.daisy.urakawa.core.TreeNodeWriteOnlyMethods} are undoable.
	 * When an application extends the SDK built-in data model, it is
	 * responsible for defining what operations are undoable. For example,
	 * NamedTreeNode extends TreeNode with setName(String) / getName() methods,
	 * when setName() is called, a new instance of the application-defined (i.e.
	 * not- built-in) TreeNodeNameChangedCommand is created and executed via the
	 * undo-redo manager.
	 * </p>
	 * <p>
	 * Internally, this method may call
	 * {@link org.daisy.urakawa.undo.WithUndoRedoManager#setUndoRedoManager(org.daisy.urakawa.undo.UndoRedoManager)}.
	 * </p>
	 * 
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void enableUndoRedo()
			throws UndoRedoTransactionIsNotEndedException;

	/**
	 * <p>
	 * Disables support for undo-redo
	 * </p>
	 * <p>
	 * This method empties and destroys the current
	 * {@link org.daisy.urakawa.undo.UndoRedoManager}, if any. Otherwise does
	 * nothing.
	 * </p>
	 * <p>
	 * Internally, this method may call
	 * {@link org.daisy.urakawa.undo.WithUndoRedoManager#setUndoRedoManager(org.daisy.urakawa.undo.UndoRedoManager)}.
	 * </p>
	 * 
	 * @throws UndoRedoTransactionIsNotEndedException
	 *             if an undo-redo transaction is currently active.
	 */
	public void disableUndoRedo()
			throws UndoRedoTransactionIsNotEndedException;
}