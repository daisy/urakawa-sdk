package org.daisy.urakawa;

import org.daisy.urakawa.command.CommandFactory;
import org.daisy.urakawa.core.IWithTreeNode;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.media.IMediaPresentation;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.IDataProviderManager;
import org.daisy.urakawa.media.data.IMediaDataManager;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.metadata.IWithMetadata;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.property.PropertyFactory;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.IChannelsManager;
import org.daisy.urakawa.undo.IUndoRedoManager;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is primarily a container for the document tree (made of
 * {@link org.daisy.urakawa.core.ITreeNode} nodes), and a host for various
 * associated factories and managers. It is also the host for
 * {@link org.daisy.urakawa.metadata}.
 * </p>
 * <p>
 * Implementations should make sure to provide constructors that create a
 * default root node, as
 * {@link org.daisy.urakawa.core.IWithTreeNode#getRootNode()} cannot return
 * NULL.
 * </p>
 * 
 */
public interface IPresentation extends IWithRootURI, IWithTreeNode,
        IWithProject, IMediaPresentation, IValueEquatable<IPresentation>,
        IWithMetadata, IWithLanguage, IXukAble,
        IEventHandler<DataModelChangedEvent>
{
    /**
     * This method analyzes the content of the data model and other data
     * structures of the authoring session, in order to determine what
     * IMediaData (and IDataProvider) objects are unused, and therefore can be
     * safely delete from the Managers (IMediaDataManager and
     * IDataProviderManager). This of course can potentially remove files from
     * the filesystem, for example in the case of IFileDataProvider.
     */
    public void cleanup();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public ChannelFactory getChannelFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public TreeNodeFactory getTreeNodeFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public CommandFactory getCommandFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public PropertyFactory getPropertyFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public MediaFactory getMediaFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public MediaDataFactory getMediaDataFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public MetadataFactory getMetadataFactory();

    /**
     * @return the factory object. Cannot be null, because an instance is
     *         created lazily.
     */
    public DataProviderFactory getDataProviderFactory();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public IUndoRedoManager getUndoRedoManager();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public IChannelsManager getChannelsManager();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public IMediaDataManager getMediaDataManager();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public IDataProviderManager getDataProviderManager();
}