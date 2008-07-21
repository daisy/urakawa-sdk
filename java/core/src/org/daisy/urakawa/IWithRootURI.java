package org.daisy.urakawa;

import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * The getter and setter for the root URI of the IPresentation.
 */
public interface IWithRootURI
{
    /**
     * Returns the default root URI for the IPresentation or the one specified
     * by setRootURI().
     * 
     * @return The root URI of the IPresentation.
     */
    public URI getRootURI();

    /**
     * Sets the root URI of the IPresentation
     * 
     * @param newRootUri cannot be null
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws URISyntaxException when the given URI is not absolute
     * 
     * @tagvalue Events "RootUriChanged"
     */
    public void setRootURI(URI newRootUri)
            throws MethodParameterIsNullException, URISyntaxException;
}
