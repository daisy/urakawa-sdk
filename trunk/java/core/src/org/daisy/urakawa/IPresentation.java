package org.daisy.urakawa;

import org.daisy.urakawa.command.IWithCommandFactory;
import org.daisy.urakawa.core.IWithTreeNode;
import org.daisy.urakawa.core.IWithTreeNodeFactory;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.media.IMediaPresentation;
import org.daisy.urakawa.media.IWithMediaFactory;
import org.daisy.urakawa.media.data.IWithDataProviderFactory;
import org.daisy.urakawa.media.data.IWithDataProviderManager;
import org.daisy.urakawa.media.data.IWithMediaDataFactory;
import org.daisy.urakawa.media.data.IWithMediaDataManager;
import org.daisy.urakawa.metadata.IWithMetadata;
import org.daisy.urakawa.metadata.IWithMetadataFactory;
import org.daisy.urakawa.property.channel.IWithChannelFactory;
import org.daisy.urakawa.property.channel.IWithChannelsManager;
import org.daisy.urakawa.undo.IWithUndoRedoManager;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is primarily a container for the document tree (made of
 * {@link org.daisy.urakawa.core.ITreeNode} nodes), and a host for various
 * associated factories and managers. It is also the
 * host for {@link org.daisy.urakawa.metadata}.
 * </p>
 * <p>
 * Implementations should make sure to provide constructors that create a
 * default root node, as
 * {@link org.daisy.urakawa.core.IWithTreeNode#getRootNode()} cannot return NULL.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.property.IPropertyFactory
 * @depend - Aggregation 1 org.daisy.urakawa.IProject
 * @depend - Composition 1 org.daisy.urakawa.core.ITreeNode
 * @depend - Composition 1 org.daisy.urakawa.property.channel.IChannelsManager
 * @depend - Composition 1 org.daisy.urakawa.property.channel.IChannelFactory
 * @depend - Composition 1 org.daisy.urakawa.core.ITreeNodeFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.IMediaDataManager
 * @depend - Composition 1 org.daisy.urakawa.media.data.IDataProviderManager
 * @depend - Composition 1 org.daisy.urakawa.media.IMediaFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.IMediaDataFactory
 * @depend - Composition 1 org.daisy.urakawa.undo.CommandFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.IDataProviderFactory
 * @depend - Composition 0..n org.daisy.urakawa.metadata.IMetadata
 * @depend - Composition 1 org.daisy.urakawa.metadata.IMetadataFactory
 * @depend - Composition 1 org.daisy.urakawa.undo.IUndoRedoManager
 * @stereotype IXukAble
 */
public interface IPresentation extends IWithRootURI, IWithPropertyFactory,
		IWithMediaFactory, IWithMediaDataFactory, IWithCommandFactory,
		IWithTreeNode, IWithProject, IMediaPresentation, IWithTreeNodeFactory,
		IWithChannelFactory, IWithChannelsManager, IWithMediaDataManager,
		IWithDataProviderFactory, IWithDataProviderManager,
		IValueEquatable<IPresentation>, IWithMetadataFactory, IWithMetadata,
		IWithLanguage, IWithUndoRedoManager, IXukAble,
		IEventHandler<DataModelChangedEvent> {
	/**
	 * This method analyzes the content of the data model and other data
	 * structures of the authoring session, in order to determine what IMediaData
	 * (and IDataProvider) objects are unused, and therefore can be safely delete
	 * from the Managers (IMediaDataManager and IDataProviderManager). This of
	 * course can potentially remove files from the filesystem, for example in
	 * the case of IFileDataProvider.
	 */
	public void cleanup();

	/**
	 * Convenience method that delegates to the IProject to obtain the
	 * IDataModelFactory.
	 * 
	 * @return the IDataModelFactory
	 * @throws IsNotInitializedException
	 *             when the IProject reference is not initialized yet.
	 */
	public IDataModelFactory getDataModelFactory()
			throws IsNotInitializedException;
}