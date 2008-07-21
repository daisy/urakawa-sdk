package org.daisy.urakawa.media;

import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Specifies the location of the data resource for a media object.
 */
public interface ILocated
{
    /**
     * @return the location of the media
     */
    String getSrc();

    /**
     * @param newSrc
     * 
     * @tagvalue Events "SrcChanged"
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameters are forbidden
     */
    void setSrc(String newSrc) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * Gets the URI of this location based on getPresentation().getRootURI()
     * 
     * @return a URI
     * @throws URISyntaxException when the value returned by getSrc() is not a
     *         well-formed URI
     */
    public URI getURI() throws URISyntaxException;
}