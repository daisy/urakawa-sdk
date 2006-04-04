package org.daisy.urakawa.coreDataModel;

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
}
