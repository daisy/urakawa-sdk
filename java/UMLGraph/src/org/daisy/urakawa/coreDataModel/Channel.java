package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;

/**
 * @depend - - - MediaType
 */
public interface Channel {
    /**
     * @return cannot return null or empty string, by contract.
     */
    public String getName();

    /**
     * @param name cannot be null, cannot be empty String
     * @tagvalue Exceptions "MethodParameterIsNullException, MethodParameterIsEmptyStringException"
     */
    public void setName(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * @param mediaType
     * @return true if the media type if supported for this channel.
     * @see org.daisy.urakawa.exceptions.MediaTypeIsIllegalException
     */
    public boolean isMediaTypeSupported(MediaType mediaType);
}
