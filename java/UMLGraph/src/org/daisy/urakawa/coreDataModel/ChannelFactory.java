package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;

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
    public Channel createChannel(string name) throws MethodParameterIsNull, MethodParameterIsEmptyString {};
}
