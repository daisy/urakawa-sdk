package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * A specialized type of media location which points to a resource with a simple
 * string. This does not indicate how to process the string format, and how to
 * interpret the actual pointed media data.
 * 
 * @zdepend - Composition - String
 */
public interface SrcMediaLocation extends MediaLocation {
	/**
	 * Gets the string locator for the media source
	 * 
	 * @return a non-null, non-empty string
	 */
	public String getSrc();

	/**
	 * Sets the string locator for the media source
	 * 
	 * @param src
	 *            a string pointing at the data resource. cannot be null or
	 *            empty.
	 * @throws MethodParameterIsNullException
	 *             if src is null
	 * @throws MethodParameterIsEmptyStringException
	 *             if src is an empty string
	 * @tagvalue Exceptions "MethodParameterIsNull,
	 *           MethodParameterIsEmptyString"
	 */
	public void setSrc(String src) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
