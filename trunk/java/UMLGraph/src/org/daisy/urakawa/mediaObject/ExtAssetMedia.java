package org.daisy.urakawa.mediaObject;
import org.daisy.urakawa.exceptions.*;


/**
 * 
 */
interface ExtAssetMedia extends MediaObject {
	
/**
 * 
 * @return the URI of the external asset. by contract, cannot return NULL or empty String.
 */
public string getURI() {};

/**
 * Sets the URI of the external asset.
 * 
 * @param newURI cannot be null
 */
public void setURI(string newURI) throws MethodParameterIsEmptyString, MethodParameterIsNull {};
}