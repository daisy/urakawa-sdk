package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;
import org.daisy.urakawa.xuk.XukAble;

/**
 * The "name" of a Channel is purely informative,
 * and is not to be considered as a way of uniquely identifying a Channel instance.
 *
 * @depend - - - MediaType
 */
public interface Channel extends XukAble {
    /**
     * @param name cannot be null, cannot be empty String
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setName(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * @return cannot return null or empty string, by contract.
     */
    public String getName();

    /**
     * @param mediaType
     * @return true if the media type if supported for this channel.
     * @see org.daisy.urakawa.exceptions.MediaTypeIsIllegalException
     */
    public boolean isMediaTypeSupported(MediaType mediaType);

    /**
     * @param mediaType
     * @return true if the media type is already supported by this channel (the call is just redondant).
     * @stereotype Initialize
     * @tagvalue Exceptions "MediaTypeIsIllegal"
     * @see org.daisy.urakawa.exceptions.MediaTypeIsIllegalException
     */
    public boolean addSupportedMediaType(MediaType mediaType) throws MediaTypeIsIllegalException;
}
