package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.ChannelNameAlreadyExist;
import org.daisy.urakawa.exceptions.ChannelNameDoesNotExist;
import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

import java.util.List;

/**
 * The presentation encapsulates the core data tree and the channels list.
 */
public class Presentation implements ChannelManager, ChannelFactory, CoreNodeFactory {
    /**
     * @return the root CoreNode of the presentation (returns mRootNode). Can return null.
     */
    public CoreNode getRootNode() {
        return null;
    }

    /**
     * @return mChannels (well, a list object). returns null if no channels (never returns an empty list).
     */
    public List getChannelsList() {
        return null;
    }

    /**
     * 
     */
    private CoreNode mRootNode;

    public Channel getChannel(String name) throws MethodParameterIsNull, MethodParameterIsEmptyString {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void addChannel(Channel channel) throws MethodParameterIsNull, ChannelNameAlreadyExist {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public void addChannel(String name) throws ChannelNameAlreadyExist, MethodParameterIsNull, MethodParameterIsEmptyString {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public void removeChannel(Channel channel) throws MethodParameterIsNull, ChannelNameDoesNotExist {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public Channel removeChannel(String name) throws MethodParameterIsNull, ChannelNameDoesNotExist, MethodParameterIsEmptyString {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void setChannelName(Channel channel, String name) throws ChannelNameDoesNotExist, MethodParameterIsEmptyString, ChannelNameAlreadyExist {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public Channel createChannel(String name) throws MethodParameterIsNull, MethodParameterIsEmptyString {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public CoreNode createNode() {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }
}