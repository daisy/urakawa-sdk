package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.media.MediaTypeIsIllegalException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * The "name" of a Channel is purely informative, and is not to be considered as
 * a way of uniquely identifying a Channel instance.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - - - MediaType
 */
public interface Channel extends WithChannelsManager, XukAble,
		ValueEquatable<Channel> {
	/**
	 * The human-readable / display name
	 * 
	 * @param name
	 *            cannot be null, cannot be empty String
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
	 */
	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * The human-readable / display name
	 * 
	 * @return cannot return null or empty string, by contract.
	 */
	public String getName();

	/**
	 * @param mediaType
	 * @return true if the media type if supported for this channel.
	 * @see org.daisy.urakawa.media.MediaTypeIsIllegalException
	 */
	public boolean isMediaTypeSupported(MediaType mediaType);

	/**
	 * @param mediaType
	 * @return true if the media type is already supported by this channel (the
	 *         call is just redondant).
	 * @stereotype Initialize
	 * @tagvalue Exceptions "MediaTypeIsIllegal"
	 * @see org.daisy.urakawa.media.MediaTypeIsIllegalException
	 */
	public boolean addSupportedMediaType(MediaType mediaType)
			throws MediaTypeIsIllegalException;

	/**
	 * @return convenience method that delegates to ChannelsManager.
	 * @see ChannelsManager#getUidOfChannel(Channel)
	 */
	public String getUid();
}
