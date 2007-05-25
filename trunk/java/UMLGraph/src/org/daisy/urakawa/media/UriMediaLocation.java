package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * A specialized type of media location which points to a resource with a URI.
 * This does not indicate how to interpret the actual pointed media data.
 * 
 * @zdepend - Composition - URI
 */
public interface UriMediaLocation extends SrcMediaLocation {
	/**
	 * Gets the URI locator for the media source
	 * 
	 * @return a non-null value
	 */
	public URI getUrl();

	/**
	 * Sets the URI locator for the media source
	 * 
	 * @param uri
	 *            the location, cannot be null
	 * @throws MethodParameterIsNullException
	 *             if uri is null
	 */
	public void setUrl(URI uri) throws MethodParameterIsNullException;
}
