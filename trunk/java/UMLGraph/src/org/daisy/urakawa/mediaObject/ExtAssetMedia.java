package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyString;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;

/**
 * 
 */
public interface ExtAssetMedia extends MediaObject {
    /**
     * @return the URI of the external asset. by contract, cannot return NULL or empty String.
     */
    public String getURI();

    /**
     * Sets the URI of the external asset.
     *
     * @param newURI cannot be null
     */
    public void setURI(String newURI) throws MethodParameterIsEmptyString, MethodParameterIsNull;
}