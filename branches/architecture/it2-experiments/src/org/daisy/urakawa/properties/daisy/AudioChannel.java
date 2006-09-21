package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.media.MediaType;

/**
 * A channel supporting audio content only.
 */
public interface AudioChannel extends Channel{

    /**
     * Returns true if and only if the given type is MediaType.AUDIO.
     * @hidden
     */
    boolean isMediaTypeSupported(MediaType mediaType);
}
