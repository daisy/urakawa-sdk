package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.xuk.XukAble;
  

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.property.channel.ChannelsProperty} instances.
 * </p>
 * 
 * @see org.daisy.urakawa.property.PropertyFactory
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.property.channel.ChannelsProperty
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface ChannelsPropertyFactory extends XukAble, WithPresentation {
	/**
	 * <p>
	 * Creates a new property, not yet associated to a node.
	 * </p>
	 * <p>
	 * This factory method does not take any argument and creates an object of
	 * the default type.
	 * </p>
	 * 
	 * @return cannot be null.
	 */
	public ChannelsProperty createChannelsProperty();
}
