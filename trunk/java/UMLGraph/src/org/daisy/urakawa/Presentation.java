package org.daisy.urakawa;

import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.core.WithTreeNodeFactory;
import org.daisy.urakawa.core.events.TreeNodeChangeManager;
import org.daisy.urakawa.core.property.WithGenericPropertyFactory;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.metadata.WithMetadataFactory;
import org.daisy.urakawa.properties.channel.WithChannelFactory;
import org.daisy.urakawa.properties.channel.WithChannelsManager;
import org.daisy.urakawa.properties.channel.WithChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.WithXmlPropertyFactory;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is primarily a container for the document tree (made of
 * {@link org.daisy.urakawa.core.TreeNode} nodes), and a host for various
 * associated factories and managers. It is also the central hub for handling
 * tree change events (registering listeners, etc. See
 * {@link org.daisy.urakawa.core.events.TreeNodeChangeManager}). It is also the
 * host for {@link org.daisy.urakawa.metadata}.
 * </p>
 * <p>
 * Implementations should make sure to provide constructors that create a
 * default root node, as
 * {@link org.daisy.urakawa.core.WithTreeNode#getTreeNode()} cannot return NULL.
 * </p>
 * <p>
 * Note: this interface assembles a set of other interfaces, but does not
 * introduce new methods itself.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.PropertyFactory
 * @depend - Aggregation 1 org.daisy.urakawa.Project
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNode
 * @depend - Composition 1 org.daisy.urakawa.properties.channel.ChannelsManager
 * @depend - Composition 1 org.daisy.urakawa.properties.channel.ChannelFactory
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNodeFactory
 * @depend - "Aggregation\n(subscribed)" 0..n org.daisy.urakawa.core.events.TreeNodeChangedListener
 * @depend - "Aggregation\n(subscribed)" 0..n org.daisy.urakawa.core.events.TreeNodeAddedRemovedListener
 * @depend - Composition 1 org.daisy.urakawa.media.data.MediaDataManager
 * @depend - Composition 1 org.daisy.urakawa.media.data.DataProviderManager
 * @depend - Composition 1 org.daisy.urakawa.media.MediaFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.MediaDataFactory
 * @depend - Composition 1 org.daisy.urakawa.media.data.DataProviderFactory
 * @depend - Composition 0..n org.daisy.urakawa.metadata.Metadata
 * @depend - Composition 1 org.daisy.urakawa.metadata.MetadataFactory
 * @stereotype XukAble
 */
public interface Presentation extends WithPropertyFactory, WithProject,
		MediaPresentation, TreeNodeChangeManager, WithTreeNode,
		WithTreeNodeFactory, WithGenericPropertyFactory, WithChannelFactory,
		WithChannelsPropertyFactory, WithChannelsManager,
		WithXmlPropertyFactory, MediaDataPresentation,
		ValueEquatable<Presentation>, WithMetadataFactory, WithMetadata,
		WithLanguage, XukAble {
}