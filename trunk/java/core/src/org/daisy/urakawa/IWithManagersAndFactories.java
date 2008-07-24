package org.daisy.urakawa;

import org.daisy.urakawa.command.CommandFactory;
import org.daisy.urakawa.core.TreeNodeFactory;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProviderFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.metadata.MetadataFactory;
import org.daisy.urakawa.property.PropertyFactory;
import org.daisy.urakawa.property.channel.ChannelFactory;
import org.daisy.urakawa.property.channel.ChannelsManager;
import org.daisy.urakawa.undo.UndoRedoManager;

/**
 * 
 */
public interface IWithManagersAndFactories
{
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
    public UndoRedoManager getUndoRedoManager();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public ChannelsManager getChannelsManager();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public MediaDataManager getMediaDataManager();

    /**
     * @return the manager object. Cannot be null, because an instance is
     *         created lazily.
     */
    public DataProviderManager getDataProviderManager();
}
