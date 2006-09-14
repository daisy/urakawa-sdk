package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.media.MediaType;

/**
 * The "name" of a Channel is purely informative,
 * and is not to be considered as a way of uniquely identifying a Channel instance.
 * @depend - - - MediaType
 */
public interface Channel {
    /**
     * @param name cannot be null, cannot be empty String
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setName(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * Returns the qualified name of this channel, which cannot return null or empty string by contract.
     * @return The qualified name of this channel.
     */
    public String getName();

    /**
     * Returns the localization identifier of this channel.
     * @return the localization identifier of this channel.
     */
    public String getLang();

    /**
     * Cheks if a given media type is supported by this channel.
     * 
     * @param mediaType A media type.
     * @return True if and only if the given media type is supported by this channel.
     */
    public boolean isMediaTypeSupported(MediaType mediaType);

    /**
     * @param mediaType
     * @return true if the media type is already supported by this channel (the call is just redondant).
     * @see org.daisy.urakawa.exceptions.MediaTypeIsIllegalException
     * @stereotype Initialize
     * @tagvalue Exceptions "MediaTypeIsIllegal"
     */
    public boolean addSupportedMediaType(MediaType mediaType) throws MediaTypeIsIllegalException;
}
