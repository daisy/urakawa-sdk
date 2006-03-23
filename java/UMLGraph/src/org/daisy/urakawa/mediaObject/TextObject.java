package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * 
 */
public class TextObject implements MediaObject {
    /**
     * 
     */
    private String mText;

    /**
     * @return cannot return NULL or empty String
     */
    public String getText() {
        return mText;
    }

    /**
     * @param text cannot be null, cannot be empty String
     */
    public void setText(String text) throws MethodParameterIsNull, MethodParameterIsEmptyString {
        mText = text;
    }
}
