package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Specifies the location of the data resource for a media object.
 * 
 * 
 */
public interface Located {
	String getSrc();

	/**
	 * 
	 * @param newSrc
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	void setSrc(String newSrc)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}