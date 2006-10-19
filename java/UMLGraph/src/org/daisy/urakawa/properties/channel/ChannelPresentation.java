package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 *
 */
public interface ChannelPresentation {
    /**
     * @return the channel manager for this presentation. Cannot return null.
     */
    public ChannelsManager getChannelsManager();

    /**
     * @return the channel factory for this presentation. Cannot return null;
     */
    public ChannelFactory getChannelFactory();

    /**
     * @param fact the channel factory for this presentation. Cannot be null;
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setChannelFactory(ChannelFactory fact) throws MethodParameterIsNullException;

    /**
     * @param man the channel manager for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setChannelsManager(ChannelsManager man) throws MethodParameterIsNullException;
}
