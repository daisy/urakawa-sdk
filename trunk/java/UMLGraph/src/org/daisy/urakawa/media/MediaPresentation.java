package org.daisy.urakawa.media;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - Composition 1 MediaFactory
 */
public interface MediaPresentation extends CorePresentation {

    /**
     * @return the media factory for this presentation. Cannot return null.
     */
    public MediaFactory getMediaFactory();

    /**
     * @param fact the media factory for this presentation. Cannot be null.
     * @tagvalue Exceptions "MethodParameterIsNull"
     * @stereotype initialize
     */
    public void setMediaFactory(MediaFactory fact) throws MethodParameterIsNullException;
}
