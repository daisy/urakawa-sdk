package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.mediaObject.*;
import org.daisy.urakawa.exceptions.*;

import java.util.ArrayList;
import java.util.List;

/**
 * An example implementation of a visitor for the URAKAWA core data tree.
 * This simply builds an ordered list of all media objects encountered that belong to the given channel.
 * It traverses the tree in depth-first order and extracts references to media objects into a separate data structure.
 * 
 */
class CoreTreeVisitorImpl_MediaOfChannelExtractor implements CoreTreeVisitor {

/**
 * The channel object corresponding to the given channel name.
 */
private Channel mChannel;

/**
 * The data structure which contains the final result of the visit.
 * It is an ordered list of the media objects belonging to the given channel.
 */
private List mMediaObjectList = new ArrayList();

/**
 * The visitor is instanced with the given presentation and channel name to lookup.
 * This constructor also launches the traversal of the root node.
 * The result of the visit can be accessed via the getResultingListOfMediaObjects().
 * 
 * @param presentation
 * @param channelName
 */
public CoreTreeVisitorImpl_MediaOfChannelExtractor(Presentation presentation, String channelName) {
    try {
        mChannel = presentation.getChannel(channelName);
    } catch (MethodParameterIsNull methodParameterIsNull) {
        methodParameterIsNull.printStackTrace();
    } catch (MethodParameterIsEmptyString methodParameterIsEmptyString) {
        methodParameterIsEmptyString.printStackTrace();
    }
    if (mChannel == null) {
        return;
    } else {
        CoreNode rootNode = presentation.getRootNode();
        if (rootNode == null) {
            return;
        } else {
            try {
                rootNode.acceptDepthFirst(this);
            } catch (MethodParameterIsNull methodParameterIsNull) {
                methodParameterIsNull.printStackTrace();
            }
        }
    }
}

/**
 * Visits the node by matching a media object belonging to the desired channel.
 * The depth-first tree traversal gives the order of the resulting list of media objects.
 * 
 * @param node
 */
public void preVisit(CoreNode node) {
    ChannelsProperty prop = (ChannelsProperty) node.getProperty(new Property.PropertyType());
    if (prop != null) {
        MediaObject media = null;
        try {
            media = prop.getMediaObject(mChannel);
        } catch (MethodParameterIsNull methodParameterIsNull) {
            methodParameterIsNull.printStackTrace();
        } catch (ChannelNameDoesNotExist channelNameDoesNotExist) {
            channelNameDoesNotExist.printStackTrace();
        }
        if (media != null) {
            mMediaObjectList.add(media);
        }
    }
}

/**
 * Unused method (empty).
 * It is implemented though, as it's part of the contract expressed by the VisitableNode interface.
 * 
 * @param node
 */
public void postVisit(CoreNode node) {
    // nothing to do here.
}

/**
 * @return the mMediaObjectList member, the result of the tree visit.
 */
public List getResultingListOfMediaObjects() {
    return mMediaObjectList;
}
}