package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.ChannelNameAlreadyExistException;
import org.daisy.urakawa.exceptions.ChannelNameDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

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

    public Channel getChannel(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void addChannel(Channel channel) throws MethodParameterIsNullException, ChannelNameAlreadyExistException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public void addChannel(String name) throws ChannelNameAlreadyExistException, MethodParameterIsNullException, MethodParameterIsEmptyStringException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public void removeChannel(Channel channel) throws MethodParameterIsNullException, ChannelNameDoesNotExistException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public Channel removeChannel(String name) throws MethodParameterIsNullException, ChannelNameDoesNotExistException, MethodParameterIsEmptyStringException {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public void setChannelName(Channel channel, String name) throws ChannelNameDoesNotExistException, MethodParameterIsEmptyStringException, ChannelNameAlreadyExistException {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    public Channel createChannel(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public CoreNode createNode() {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }
}