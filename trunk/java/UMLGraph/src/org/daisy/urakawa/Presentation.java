package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.properties.channel.ChannelFactory;
import org.daisy.urakawa.properties.channel.ChannelsManager;

/**
 * The presentation.
 *
 * @depend 1 Composition 1 CoreNode
 * @depend - Aggregation 1 ChannelFactory
 * @depend - Aggregation 1 ChannelsManager
 */
public interface Presentation extends CorePresentation {
    /**
     * @return the channel manager for this presentation. Cannot return null.
     */
    public ChannelsManager getChannelsManager();

    /**
     * @return the channel factory for this presentation. Cannot return null;
     */
    public ChannelFactory getChannelFactory();


    /**
     * @return the media factory for this presentation. Cannot return null.
     */
    public MediaFactory getMediaFactory();

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
     * @param fact the media factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setMediaFactory(MediaFactory fact) throws MethodParameterIsNullException;
}