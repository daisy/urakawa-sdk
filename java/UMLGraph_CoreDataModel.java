package coreDataModel;

import java.util.List;
import java.util.Map;
import java.util.ArrayList;
import java.util.Arrays;

import mediaObject.*;
import exceptions.*;

/**
 * @view
 * @opt nodefillcolor PaleGreen
 *
 * @match class *
 * @opt hide
 *
 * @match class coreDataModel.*
 * @opt !hide
 *
 */
class ViewCoreDataModel extends ViewBase {}

/**
 * Has methods specific to the URAKARA core model nodes.
 * Regarding properties: there can only be one property of a given property type.
 * 
 */
interface CoreNode extends DOMNode, VisitableNode {

/**
 * @return the Presentation to which the CoreNode belongs (mPresentation). Cannot return null (there is always a presentation for a node).
 */
public Presentation getPresentation() {};

/**
 * @param type 
 * @return the Property of a given PropertyType. can return null if there is not such property instance.
 */
public Property getProperty(PropertyType type) {};

/**
 * Sets a Property.
 * 
 * @param newProp 
 * @return If the CoreNode instance already has a Property of the given type, this Property is overwritten, and the method returns true. If there is no override, returns false.
 */
public boolean setProperty(Property newProp) throws MethodParameterIsNull {};

/**
 * Removes the Property of the given PropertyType.
 * 
 * @param type 
 * @return null if there was no such property, or returns the removed property object.
 */
public Property removeProperty(PropertyType type) {};
}

/**
 * A visitor specialized to tree structures,
 * in particular CoreNode (this is why it's called CoreTreeVisitor).
 * This specifies the business logic action associated to the tree traversal realized by
 * VisitableNode implementations (in this case CoreNodeImpl), via the calls to preVisit()
 * and postVisit() methods before and after recursive traversals of the children nodes, respectively.
 * 
 */
interface CoreTreeVisitor {
	
/**
 * Method called before visiting children nodes of the given CoreNode.
 * Implementations of this interface should define the business logic action
 * to be taken for each traversed node.
 * 
 * @param node cannot be null.
 */
public void preVisit(CoreNode node) throws MethodParameterIsNull {};

/**
 * Method called after visiting children nodes of the given CoreNode.
 *  Implementations of this interface should define the business logic
 * action to be taken for each traversed node.
 * 
 * @param node cannot be null.
 */
public void postVisit(CoreNode node) throws MethodParameterIsNull {};
}

/**
 * A node that is traversable using the visitor pattern.
 * Such a node can be traversed (recursively) either depth or breadth-first.
 * Each method must call the preVisit() and postVisit() methods of the CoreTreeVisitor
 * before and after traversing the children nodes, respectively.
 * The VisitableNode implementation (CoreNodeImpl) only handles the tree traversal,
 * the actual business logic action associated to the node is handled
 * by CoreTreeVisitor implementations.
 * 
 */
interface VisitableNode {
	
/**
 * Depth-first traversal of the Node.
 * Must call the preVisit() and postVisit() methods of the CoreTreeVisitor
 * before and after recursively traversing children, respectively.
 *
 * @param visitor cannot be null.
 */
public void acceptDepthFirst(CoreTreeVisitor visitor) throws MethodParameterIsNull {};

/**
 * Breadth-first traversal of the Node. Must call the preVisit() and postVisit() methods
 * of the CoreTreeVisitor before and after recursively traversing children, respectively.
 * Usually trickier to implement than the more straight-forward depth-first traversal.
 * 
 * @param visitor cannot be null.
 */
public void acceptBreadthFirst(CoreTreeVisitor visitor) throws MethodParameterIsNull {};
}

enum PropertyType {
	STRUCTURE, SMIL, CHANNEL, XML;
}

enum StructureType {
	ELEMENT, TEXT;
}

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
public CoreTreeVisitorImpl_MediaOfChannelExtractor(Presentation presentation, string channelName) {        
    mChannel = presentation.getChannel(channelName);
    if (mChannel == null) {
        return;
    } else {
        CoreNode rootNode = presentation.getRootNode();
        if (rootNode == null) {
            return null;
        } else {
            rootNode.acceptDepthFirst(this);
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
    ChannelsProperty prop = (ChannelsProperty) node.getProperty(PropertyType.ChannelsProperty);
    if (prop != null) {
        MediaObject media = prop.getMediaObject(mChannel);
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



/**
 * The implementation of the CoreNode interface.
 * Having the CoreNode be an interface leaves the model open for more than one possible CoreNode implementation.
 * Example code is given for the relevant parts that need to be made explicit for helping developers
 * apply the principles and benefits of such architecture.
 * The proposed implementation is incomplete and by no means definitive.
 */
class CoreNodeImpl implements CoreNode {

/**
 * Example of how to store children nodes of the current node, using a Java-based list class.
 */
private List mChildrenNodes = new ArrayList();

/**
 * Example implementation of depth-first traversal, see associated source code written in Java.
 * 
 * @param visitor 
 */
public void acceptDepthFirst(CoreTreeVisitor visitor) {        
    visitor.preVisit(this);
    for (int i = 0; i < getChildrenCount(); i++) {
        VisitableNode childNode = (VisitableNode) getChild(i);
        childNode.acceptDepthFirst(visitor);
    }
    visitor.postVisit(this);
} 

/**
 * Example implementation of how to get the number of children nodes,
 * see associated source code written in Java.
 * 
 * @return 
 */
public int getChildrenCount() {        
    return mChildrenNodes.size();
} 

/**
 * Example implementation of how to get a specific child node at the given index,
 * see associated source code written in Java.
 * 
 * @param i 
 * @return 
 */
public CoreNode getChild(int i) {        
    return (CoreNode) mChildrenNodes.get();
} 
}



/**
 * Has methods for DOM like tree navigation and manipulation
 */
interface DOMNode {
	
/**
 * @return the parent of the DOMNode instance. returns NULL is this node is the root.
 */
public DOMNode getParent() {};

/**
 * Appends a new child DOMNode to the end of the list of children.
 * 
 * @param node cannot be null.
 */
public void appendChild(DOMNode node) throws MethodParameterIsNull {};

/**
 * Inserts a new child DOMNode before (sibbling) a given reference child DOMNode.
 * 
 * @param node cannot be null
 * @param anchorNode cannot be null, must exist as a child.
 */
public void insertBefore(DOMNode node, DOMNode anchorNode) throws MethodParameterIsNull, NodeDoesNotExist {};

/**
 * Inserts a new child DOMNode after (sibbling) a given reference child DOMNode.
 * 
 * @param node cannot be null
 * @param anchorNode cannot be null, must exist as a child.
 */
public void insertAfter(DOMNode node, DOMNode anchorNode) throws NodeDoesNotExist, MethodParameterIsNull {};

/**
 * @param index must be in bounds: [0..children.size-1]
 * @return the child DOMNode at a given index. cannot return null, by contract.
 */
public DOMNode getChild(unsigned_int index) throws MethodParameterIsValueOutOfBounds {};

/**
 * @return the number of child DOMNodes, can return 0 if no children.
 */
public unsigned_int getChildCount() {};

/**
 * Gets the index of a given child DOMNode.
 * 
 * @param node cannot be null, must exist as a child
 * @return 
 */
public unsigned_int indexOf(DOMNode node) throws NodeDoesNotExist, MethodParameterIsNull {};

/**
 * Removes a given child DOMNode, of which parent is then NULL.
 * 
 * @param node node must exist as a child, cannot be null
 */
public void removeChild(DOMNode node) throws NodeDoesNotExist, MethodParameterIsNull {};

/**
 * Removes the child DOMNode at a given index.
 * 
 * @param index must be in bounds [0..children.size-1].
 * @return the removed node, which parent is then NULL.
 */
public DOMNode removeChild(unsigned_int index) throws MethodParameterIsValueOutOfBounds {};

/**
 * Replaces a given child DOMNode with a new given DOMNode.
 * the old node's parent is then NULL.
 * 
 * @param node cannot be null.
 * @param oldNode cannot be null, must exist as a child.
 */
public void replaceChild(DOMNode node, DOMNode oldNode) throws NodeDoesNotExist, MethodParameterIsNull {};

/**
 * Replaces the child DOMNode at a given index with a new given DOMNode.
 * 
 * @param node cannot be null.
 * @param index must be in bounds: [0..children.size-1]
 * @return the Node that was replaced, which parent is NULL.
 */
public DOMNode replaceChild(DOMNode node, unsigned_int index) throws MethodParameterIsValueOutOfBounds, MethodParameterIsNull {};

/**
 * Detaches the DOMNode instance from the DOM tree, equivalent to getParent().removeChild(this).
 * After such operation, getParent() must return NULL.
 * 
 * @return itself.
 */
public DOMNode detach() {};
}


/**
 * 
 */
interface CoreNodeFactory {
	
/**
 * Creates a new node, which is not linked to the core data tree yet.
 * @return cannot return null.
 */
public CoreNode createNode() {};
}


/**
 * 
 */
public interface Property {
	
/**
 * Each class that implements the Property interface must have getType() return a different PropertyType.
 * This in Java would have been implemented with explicit class cast and instanceof operator,
 * but for C++ we have a safer type enumeration.
 * 
 * @return the PropertyType of the Property.
 */
public PropertyType getType() {};
}

/**
 * 
 */
public class StructureProperty implements Property {

/**
 * The name of the structure element described by the StructureProperty
 * with DAISY this is used for the name of the XML element in the textual content document.
 * Cannot be NULL and cannot be an empty string.
 */
private string mName;

/**
 * The namespace of the structure element described by the StructureProperty
 * in DAISY this is the namespace of the xml element in the textual content document.
 * Cannot be NULL and can be an empty string.
 */
private string mNamespace;

/**
 * The type of the structure element described by the StructureProperty, one of element and text
 * in DAISY this is the type of xml node in the textual content document.
 * Remark that for a StructureProperty with mType text, mName and mNamespace and mAttributes
 * has no meaning if the StructureProperty describes xml.
 */
private StructureType mType;

/**
 * @return the mName. Cannot return NULL or an empty string, by contract.
 */
public string getName() {return mName;} 

/**
 * @return mNamespace. Cannot return NULL but can be an empty string, by contract.
 */
public string getNamespace() {return mNamespace;} 

/**
 * Sets mName.
 *
 * @param newName cannot be null, cannot be empty String
 */
public void setName(string newName) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Sets mNamespace.
 * 
 * @param newNS cannot be null,
 */
public void setNamespace(string newNS) throws MethodParameterIsNull {} 

/**
 * @param attrName cannot be null, cannot be empty String
 * @return the value of the attribute with a given name.Cannot return NULL and cannot return an empty string.
 */
public string getAttributeValue(string attrName) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @param attrName cannot be null, cannot be empty String
 * @param attrNS cannot be null, cannot be empty String
 * @return the value of the attribute with a given name and namespace. Cannot return NULL and cannot return an empty string.
 */
public string getAttributeValue(string attrName, string attrNS) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Sets the value of the attribute with a given name.
 * 
 * @param attrName cannot be null, cannot be empty
 * @param value cannot be null, cannot be empty
 */
public void setAttributeValue(string attrName, string value) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Sets the value of the attribute with a given name and namespace.
 * 
 * @param attrName cannot be null
 * @param attrNS cannot be null
 * @param value cannot be null
 */
public void setAttributeValue(string attrName, string attrNS, string value) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @return mType
 */
public StructureType getStructureType() {} 

/**
 * Sets mType
 *
 * @param newType 
 */
public void setStructureType(StructureType newType) {} 
}

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



/**
 * This property maintains a mapping from Channel object to Media object.
 * Channels referenced here are actual existing channels in the presentation (see ChannelManager).
 */
public class ChannelsProperty {

/**
 * see class documentation.
 */
private MapChannelToMediaObject mMapChannelToMediaObject;

/**
 * @param channel cannot be null, the channel must exist in the list of current channels, See ChannelManager.
 * @return the MediaObject in a given Channel. returns null if there is no media object for this channel.
 */
public MediaObject getMediaObject(Channel channel) throws MethodParameterIsNull, ChannelNameDoesNotExist {} 

/**
 * Sets the MediaObject in a given Channel.
 * 
 * @param channel cannot be null, the channel must exist in the list of current channels, see ChannelManager.
 * @param mediaObject cannot be null
 */
public void setMediaObject(Channel channel, MediaObject mediaObject) throws MethodParameterIsNull, ChannelNameDoesNotExist  {} 

/**
 * @return the list of channels that are used in this particular property.Can return null (no channels = the property should not really exist, conceptually). will never return an empty list.
 */
public List getUsedChannelsList() {} 
}


/**
 * Implementations of this interface must guarantee that a channel name is unique in the list of current channels.
 * "add" operations are obviously concerned by this.
 * The "remove" operation actually removes the media properties associated with the channel as well.
 * (Note: this can be implemented with a Visitor)
 */
public interface ChannelManager {
	
/**
 * See interface documentation.
 * 
 * @param name cannot be null,cannot be empty String
 * @return the Channel that has the given name. can return null if the channel does not exist.
 */
public Channel getChannel(string name) throws MethodParameterIsNull, MethodParameterIsEmptyString {};

/**
 * Adds an existing Channel to the list.
 * See interface documentation.
 * 
 * @param channel cannot be null, channel name must not already used by another existing channel.
 */
public void addChannel(Channel channel) throws MethodParameterIsNull, ChannelNameAlreadyExist {};

/**
 * Adds a new Channel with a given name. Equivalent to addChannel(createChannel(name));
 * See interface documentation.
 * 
 * @param name cannot be null, cannot be empty String, the channel name must not already used by another existing channel
 */
public void addChannel(string name) throws ChannelNameAlreadyExist, MethodParameterIsNull, MethodParameterIsEmptyString {};

/**
 * Removes a given channel from the Presentation instance.
 * See interface documentation.
 * 
 * @param channel cannot be null, the channel must exist in the list of current channels
 */
public void removeChannel(Channel channel) throws MethodParameterIsNull, ChannelNameDoesNotExist {};

/**
 * Removes the Channel with a given name from the Presentation instance. Equivalent to removeChannel(getChannel(name)).
 * See interface documentation.
 * 
 * @param name cannot be null, cannot be empty String, the channel name must exist in the list of current channels
 * @return the removed channel, or null if was not found.
 */
public Channel removeChannel(string name) throws MethodParameterIsNull, ChannelNameDoesNotExist, MethodParameterIsEmptyString {};

/**
 * Takes care of changing the name of an existing channel in the list, with handling of name unicity.
 * See interface documentation.
 * 
 * @param channel cannot be null, the channel must exist in the list of current channels, if the channel name must not already be used by another existing channel
 * @param name cannot be null, cannot be empty String
 */
public void setChannelName(Channel channel, string name) throws ChannelNameDoesNotExist, MethodParameterIsEmptyString, ChannelNameAlreadyExist {};
}


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
private string mName;

/**
 * See mName documentation.
 * 
 * @return cannot return null or empty string, because by contract there is no way of setting the name to NULL or empty string.
 */
public string getName() {} 

/**
 * See mName documentation.
 * 
 * @param name cannot be null, cannot be empty String
 */
public void setName(string name) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * Constructor which initializes a channel with the given name.
 * Please note that the name can be changed afterwards using setName()
 * 
 * @param name cannot be null, cannot be empty String
 */
public Channel(string name) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 
}



/**
 * <p></p>
 * 
 */
public class Attribute {

/**
 * The name of the Attribute. Cannot be NULL and cannot be an empty string.
 */
private string mName;

/**
 * The value of the Attribute, Cannot be NULL and cannot be an empty string.
 */
private string mValue;

/**
 * The namespace of the Attribute. Cannot be NULL but can be an empty string.
 * 
 */
private string mNamespace;

/**
 * @return mNAme. Cannot return NULL and cannot return an empty string.
 */
public string getName() {} 

/**
 * Sets mName.
 * 
 * @param newName cannot be null, cannot be empty String
 */
public void setName(string newName) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @return mValue. Cannot return NULL and cannot return an empty string.
 */
public string getValue() {} 

/**
 * Sets mValue.
 * 
 * @param newValue cannot be null, cannot be empty String
 */
public void setValue(string newValue) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 

/**
 * @return mnamespace. Cannot return NULL but can return an empty string.
 */
public string getNamespace() {} 

/**
 * Sets mNamespace.
 * 
 * @param newNS cannot be null, cannot be empty String
 */
public void setNamespace(string newNS) throws MethodParameterIsNull, MethodParameterIsEmptyString {} 
}
