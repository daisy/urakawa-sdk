package org.daisy.urakawa.properties.channel;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting a name.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithName {
	/**
	 * The human-readable / display name
	 * 
	 * @param name
	 *            cannot be null, cannot be empty String
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * The human-readable / display name
	 * 
	 * @return cannot return null or empty string, by contract.
	 */
	public String getName();
}
