package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.media.MediaType;

/**
 * A channel supporting text content only.
 */
public interface TextChannel extends Channel{

    /**
     * Returns true if and only if mediaType is MediaType.TEXT
     * @hidden
     */
    boolean isMediaTypeSupported(MediaType mediaType);
}
