package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.WithPresentation;
  

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.property.channel.ChannelsProperty} instances.
 * </p>
 * 
 * @see org.daisy.urakawa.PropertyFactory
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
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
