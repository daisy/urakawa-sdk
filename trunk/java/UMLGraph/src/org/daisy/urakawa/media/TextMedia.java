package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Just text, no structure (e.g. HTML formating), no time (e.g. SMIL).
 * {@link Media#isContinuous()} should return false for static text fragments like this.
 * {@link Media#getMediaType()} should return MediaType.TEXT
 */
public interface TextMedia extends Media {
    /**
     * @return the text. Cannot be NULL
     */
    public String getText();

    /**
     * @param text Cannot be NULL
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setText(String text) throws MethodParameterIsNullException;
}
