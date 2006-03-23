package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;


/**
 * The presentation encapsulates the core data tree and the channels list.
 */
public class Presentation implements ChannelManager, ChannelFactory, CoreNodeFactory {

/**
 * @return the root CoreNode of the presentation (returns mRootNode). Can return null.
 */
public CoreNode getRootNode() {} 

/**
 * @return mChannels (well, a list object). returns null if no channels (never returns an empty list).
 */
public List getChannelsList() {} 

/**
 * 
 */
private CoreNode mRootNode;
}