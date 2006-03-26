package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * Just text, no structure (e.g. HTML formating), no time (e.g. SMIL).
 * {@link MediaObject#isContinuous()} should return false for static text fragments like this.
 * {@link MediaObject#getType()} should return MediaObjectType.TEXT
 */
public interface TextObject extends MediaObject {

    /**
     * @return the text. Cannot be NULL or empty String.
     */
    public String getText();

    /**
     * @param text Cannot be NULL or empty String.
     */
    public void setText(String text) throws MethodParameterIsNull, MethodParameterIsEmptyString;
}
