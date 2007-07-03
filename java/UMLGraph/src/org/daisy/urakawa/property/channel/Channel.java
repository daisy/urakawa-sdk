package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithLanguage;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.xuk.XukAble;

/**
 * The "name" of a Channel is purely informative, and is not to be considered as
 * a way of uniquely identifying a Channel instance.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1..n org.daisy.urakawa.media.MediaType
 * @depend - Aggregation 1 org.daisy.urakawa.properties.channel.ChannelsManager
 * @stereotype XukAble
 */
public interface Channel extends WithChannelsManager, WithName, WithLanguage,
		XukAble, ValueEquatable<Channel> {
	/**
	 * @return convenience method that delegates to ChannelsManager.
	 * @see ChannelsManager#getUidOfChannel(Channel)
	 * @stereotype Convenience
	 */
	public String getUid();

	public boolean isEquivalentTo(Channel otherChannel);

	public Channel exportChannel(Presentation destPres)
			throws FactoryCannotCreateTypeException;

	/**
	 * Tests whether the given Media obejct is accepted by this Channel
	 * @param media
	 * @return
	 * @see org.daisy.urakawa.media.DoesNotAcceptMediaException
	 */
	public boolean canAccept(Media media);
}
