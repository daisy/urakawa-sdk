/**
 * Example implementations:
 * They will not appear in any UML class diagrams,
 * but this Java code can be used to better understand
 * how the model can be used.
 * Ultimately, implementations of the design may vary,
 * so these examples are not even guidelines,
 * they are really just illustrative.
 */
package org.daisy.urakawa.examples;

import org.daisy.urakawa.core.CoreNode;
import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.core.property.PropertyType;
import org.daisy.urakawa.properties.channel.Channel;
import org.daisy.urakawa.properties.channel.ChannelsManager;
import org.daisy.urakawa.properties.channel.ChannelsProperty;
import org.daisy.urakawa.core.visitor.CoreNodeVisitor;

import java.util.ArrayList;
import java.util.List;

/**
 * An example implementation of a visitor for the URAKAWA core data tree.
 * This simply builds an ordered list of all media objects encountered that belong to the given channel.
 * It traverses the tree in depth-first order and extracts references to media objects into a separate data structure.
 */
public class CoreNodeVisitorImpl_MediaOfChannelExtractor implements CoreNodeVisitor {
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
    public CoreNodeVisitorImpl_MediaOfChannelExtractor(Presentation presentation, String channelName) {
        ChannelsManager channelsManager = presentation.getChannelsManager();
        List listOfChannels = null;
        try {
            listOfChannels = channelsManager.getListOfChannels();
        } catch (IsNotInitializedException e) {
            e.printStackTrace();
            return;
        }
        for (int i = 0; i < listOfChannels.size(); i++) {
            Channel channel = (Channel) listOfChannels.get(i);
            String name = channel.getName();
            if (name.equals(channelName)) {
                mChannel = channel;
                break;
            }
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

                    // or use inline anonymous class (delegate in C#, peharps closure in Ruby, etc.):

                    rootNode.acceptDepthFirst(new CoreNodeVisitor() {
                        public void preVisit(CoreNode node) throws MethodParameterIsNullException {
                            CoreNodeVisitorImpl_MediaOfChannelExtractor.this.preVisit(node);
                        }

                        public void postVisit(CoreNode node) throws MethodParameterIsNullException {

                        }
                    });


                } catch (MethodParameterIsNullException methodParameterIsNull) {
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
        ChannelsProperty prop = (ChannelsProperty) node.getProperty(new PropertyType());
        if (prop != null) {
            Media media = null;
            try {
                media = prop.getMedia(mChannel);
            } catch (MethodParameterIsNullException methodParameterIsNull) {
                methodParameterIsNull.printStackTrace();
            } catch (ChannelDoesNotExistException channelDoesNotExist) {
                channelDoesNotExist.printStackTrace();
            }
            if (media != null) {
                mMediaObjectList.add(media);
            }
        }
    }

    /**
     * Unused method (empty).
     * It is implemented though, as it's part of the contract expressed by the VisitableCoreNode interface.
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