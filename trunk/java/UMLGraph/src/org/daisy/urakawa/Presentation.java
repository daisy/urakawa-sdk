package org.daisy.urakawa;

import org.daisy.urakawa.core.WithTreeNode;
import org.daisy.urakawa.core.WithTreeNodeFactory;
import org.daisy.urakawa.core.events.TreeNodeChangeManager;
import org.daisy.urakawa.core.property.WithGenericPropertyFactory;
import org.daisy.urakawa.media.MediaPresentation;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.metadata.WithMetadata;
import org.daisy.urakawa.properties.channel.WithChannelFactory;
import org.daisy.urakawa.properties.channel.WithChannelsManager;
import org.daisy.urakawa.properties.channel.WithChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.WithXmlPropertyFactory;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * A Presentation is a container for the document tree, and all its associated
 * utilities, such as the factories, the managers, etc.
 * </p>
 * <p>
 * This interface is the combination of all the "sub-types" of presentations,
 * which individually support the concepts of channels, xml properties (element
 * and attributes), media object, media data manager, etc.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 org.daisy.urakawa.PropertyFactory
 * @depend - Aggregation 1 org.daisy.urakawa.Project
 * @depend - Composition 1 org.daisy.urakawa.core.TreeNode
 * @depend - Composition 0..n org.daisy.urakawa.metadata.Metadata
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
 */
public interface Presentation extends WithPropertyFactory, WithProject,
		WithMetadata, MediaPresentation, TreeNodeChangeManager, WithTreeNode,
		WithTreeNodeFactory, WithGenericPropertyFactory, WithChannelFactory,
		WithChannelsPropertyFactory, WithChannelsManager,
		WithXmlPropertyFactory, MediaDataPresentation,
		ValueEquatable<Presentation>, XukAble {
}