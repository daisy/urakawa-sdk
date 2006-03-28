package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * At this stage a Channel is a container for a simple string name (mandatory, cannot be null).
 * In principle, the name of a channel should be unique, but this class does not put any constraints
 * related to the unicity of the channel names. Therefore names are not ID, and they can be changed
 * after a channel has been instanciated.
 * An encapsulating class should take care of maintaining name unicity. See ChannelManager.
 */
public class Channel {
    /**
     * The name of the Channel. cannot be null. See class documentation.
     */
    private String mName;

    /**
     * See mName documentation.
     *
     * @return cannot return null or empty string, because by contract there is no way of setting the name to NULL or empty string.
     */
    public String getName() {
        return null;
    }

    /**
     * See mName documentation.
     *
     * @param name cannot be null, cannot be empty String
     */
    public void setName(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * Constructor which initializes a channel with the given name.
     * Please note that the name can be changed afterwards using setName()
     *
     * @param name cannot be null, cannot be empty String
     */
    public Channel(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }
}
