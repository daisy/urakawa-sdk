package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.xuk.IXukAble;
  

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.property.channel.IChannelsProperty} instances.
 * </p>
 * 
 * @see org.daisy.urakawa.property.PropertyFactory
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.property.channel.IChannelsProperty
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 */
public interface IChannelsPropertyFactory extends IXukAble, IWithPresentation {
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
	public IChannelsProperty createChannelsProperty();
}
