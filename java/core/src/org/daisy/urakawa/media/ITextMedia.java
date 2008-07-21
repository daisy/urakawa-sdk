package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * The simple text media type, which stores text data internally (no external
 * location)
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface ITextMedia extends IMedia
{
    /**
     * @return the text. Cannot be NULL
     */
    public String getText();

    /**
     * @param text Cannot be NULL
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * 
     * @tagvalue Events "TextChanged"
     */
    public void setText(String text) throws MethodParameterIsNullException;
}
