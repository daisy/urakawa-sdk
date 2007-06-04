package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Specifies the location of the data resource for a media object.
 * 
 * @todo verify / add comments and exceptions
 */
public interface Located {
	String getSrc();

	/**
	 * 
	 * @param newSrc
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	void setSrc(String newSrc)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}