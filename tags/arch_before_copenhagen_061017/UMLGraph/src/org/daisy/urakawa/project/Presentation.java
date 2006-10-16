package org.daisy.urakawa.project;

import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.coreTree.CoreNode;
import org.daisy.urakawa.properties.channels.ChannelFactory;
import org.daisy.urakawa.properties.channels.ChannelsManager;
import org.daisy.urakawa.properties.PropertyFactory;
import org.daisy.urakawa.coreTree.CoreNodeFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * The presentation.
 *
 * @depend 1 Composition 1 CoreNode
 * @depend - Aggregation 1 ChannelFactory
 * @depend - Aggregation 1 ChannelsManager
 */
public interface Presentation {
    /**
     * @return the root CoreNode of the presentation. Can return null (if the tree is not allocated yet).
     */
    public CoreNode getRootNode();

    /**
     * @return the channel manager for this presentation. Cannot return null.
     */
    public ChannelsManager getChannelsManager();

    /**
     * @return the channel factory for this presentation. Cannot return null;
     */
    public ChannelFactory getChannelFactory();

    /**
     * @return the node factory for this presentation. Cannot return null.
     */
    public CoreNodeFactory getCoreNodeFactory();

    /**
     * @return the property factory for this presentation. Cannot return null.
     */
    public PropertyFactory getPropertyFactory();

    /**
     * @return the media factory for this presentation. Cannot return null.
     */
    public MediaFactory getMediaFactory();

    /**
     * @param node the root CoreNode of the presentation. Can be null.
     */
    public void setRootNode(CoreNode node);

    /**
     * @param man the channel manager for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setChannelsManager(ChannelsManager man) throws MethodParameterIsNullException;

    /**
     * @param fact the channel factory for this presentation. Cannot be null;
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setChannelFactory(ChannelFactory fact) throws MethodParameterIsNullException;

    /**
     * @param fact the node factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setCoreNodeFactory(CoreNodeFactory fact) throws MethodParameterIsNullException;

    /**
     * @param fact the property factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setPropertyFactory(PropertyFactory fact) throws MethodParameterIsNullException;

    /**
     * @param fact the media factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setMediaFactory(MediaFactory fact) throws MethodParameterIsNullException;
}