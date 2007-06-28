package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;

/**
 * <p>
 * Getting and Setting a factory.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithMediaTypes {
	/**
	 * @param mediaType
	 * @return true if the media type if supported for this channel.
	 * @see org.daisy.urakawa.media.MediaTypeIsIllegalException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean isMediaTypeSupported(MediaType mediaType)
			throws MethodParameterIsNullException;

	/**
	 * @param mediaType
	 * @return true if the media type is already supported by this channel (does
	 *         nothing).
	 * @stereotype Initialize
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public boolean addSupportedMediaType(MediaType mediaType)
			throws MethodParameterIsNullException;
}
