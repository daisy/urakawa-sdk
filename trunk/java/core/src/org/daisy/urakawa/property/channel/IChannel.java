package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IPresentation;
import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithLanguage;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.IMedia;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * The "name" of a IChannel is purely informative, and is not to be considered
 * as a way of uniquely identifying a IChannel instance.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1..n org.daisy.urakawa.media.MediaType
 * @depend - Aggregation 1 org.daisy.urakawa.property.channel.IChannelsManager
 * 
 */
public interface IChannel extends IWithPresentation, IWithName, IWithLanguage,
		IXukAble, IValueEquatable<IChannel> {
	/**
	 * @return convenience method that delegates to IChannelsManager.
	 * @see IChannelsManager#getUidOfChannel(IChannel)
	 * 
	 */
	public String getUid();

	/**
	 * Determines if the channel is equivalent to a given other channel,
	 * possibly from another IPresentation
	 * 
	 * @param otherChannel
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 */
	public boolean isEquivalentTo(IChannel otherChannel)
			throws MethodParameterIsNullException;

	/**
	 * @param destPres
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @return can return null in case of failure.
	 * @throws FactoryCannotCreateTypeException
	 * @throws IsNotInitializedException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 */
	public IChannel export(IPresentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException;

	/**
	 * Tests whether the given IMedia object is accepted by this IChannel
	 * 
	 * @param iMedia
	 * @return true or false
	 * @throws MethodParameterIsNullException
	 * @see org.daisy.urakawa.media.DoesNotAcceptMediaException
	 */
	public boolean canAccept(IMedia iMedia)
			throws MethodParameterIsNullException;
}
