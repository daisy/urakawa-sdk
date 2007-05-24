package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 ChannelsManager
 * @depend - Aggregation 1 ChannelFactory
 */
public interface ChannelPresentation extends CorePresentation {
    /**
     * @return the channel manager for this presentation. Cannot return null.
     */
    public ChannelsManager getChannelsManager();

    /**
     * @return the channel factory for this presentation. Cannot return null;
     */
    public ChannelFactory getChannelFactory();

    /**
     * @return Cannot return null. This is a convenience method for CorePresentation.getPropertyFactory() to avoid explicit cast when writing applications. The returned object instance is the same for both method calls.
     * @see CorePresentation#getPropertyFactory()
     */
    public ChannelsPropertyFactory getChannelsPropertyFactory();

    /**
     * @param fact the channel factory for this presentation. Cannot be null;
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @stereotype initialize
     */
    public void setChannelFactory(ChannelFactory fact) throws MethodParameterIsNullException;

    /**
     * @param man the channel manager for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @stereotype initialize
     */
    public void setChannelsManager(ChannelsManager man) throws MethodParameterIsNullException;

}
