package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Just text, no structure (e.g. HTML formating), no time (e.g. SMIL).
 * {@link Media#isContinuous()} should return false for static text fragments like this.
 * {@link Media#getType()} should return MediaType.TEXT
 */
public interface TextMedia extends Media {
    /**
     * @param text Cannot be NULL or empty String.
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setText(String text) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * @return the text. Cannot be NULL or empty String.
     */
    public String getText();
}
