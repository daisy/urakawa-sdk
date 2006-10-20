package org.daisy.urakawa;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.core.CoreNodeFactory;
import org.daisy.urakawa.core.property.PropertyFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.properties.channel.ChannelFactory;
import org.daisy.urakawa.properties.channel.ChannelsManager;

import java.net.URI;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * Generally speaking, an end-user would not need to use this class directly.
 * They would just manipulate the corresponding abstract interface and use the provided
 * default factory implementation to create this class instances transparently.
 * -
 * However, this is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such (it's public after all),
 * or they can sub-class it in order to specialize their application.
 */
public class PresentationImpl implements Presentation {
    /**
     * @hidden
     */
    public CoreNode getRootNode() {
        return null;
    }

    /**
     * @hidden
     */
    public ChannelFactory getChannelFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public ChannelsManager getChannelsManager() {
        return null;
    }

    /**
     * @hidden
     */
    public CoreNodeFactory getCoreNodeFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public PropertyFactory getPropertyFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public MediaFactory getMediaFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public void setRootNode(CoreNode node) {
    }

    /**
     * @hidden
     */
    public void setChannelsManager(ChannelsManager man) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void setChannelFactory(ChannelFactory fact) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void setCoreNodeFactory(CoreNodeFactory fact) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void setPropertyFactory(PropertyFactory fact) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public void setMediaFactory(MediaFactory fact) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public boolean XukIn(URI source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XukOut(URI destination) {
        return false;
    }

    /**
     * @hidden
     */
    public String getXukLocalName() {
        return null;
    }

    /**
     * @hidden
     */
    public String getXukNamespaceURI() {
        return null;
    }
}
