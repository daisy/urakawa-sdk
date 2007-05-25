package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * Specifies the location of the data resource for a media object.
 * 
 * @depend - Composition 1 MediaLocation
 */
public interface Located {
	/**
	 * Gets the location of the media resources. Cannot be null
	 * 
	 * @return a non null value.
	 */
	MediaLocation getLocation();

	/**
	 * Sets the location
	 * 
	 * @param newlocation cannot be null.
	 * @throws MethodParameterIsNullException
	 *             if newlocation is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	void setLocation(MediaLocation newlocation)
			throws MethodParameterIsNullException;
}