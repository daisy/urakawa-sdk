package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * 
 */
public interface ChannelFactory {
    /**
     * Creates a new Channel with a given name, which is not linked to the channels list yet.
     *
     * @param name cannot be null, cannot be empty String
     * @return cannot return null
     */
    public Channel createChannel(String name) throws MethodParameterIsNull, MethodParameterIsEmptyString;
}
