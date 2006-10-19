package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 *
 */
public interface MediaPresentation {
    /**
     * @return the media factory for this presentation. Cannot return null.
     */
    public MediaFactory getMediaFactory();

    /**
     * @param fact the media factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setMediaFactory(MediaFactory fact) throws MethodParameterIsNullException;
}
