package org.daisy.urakawa.metadata;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * The optional attributes for the Metadata
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithOptionalAttributeValue {
	/**
	 * @param key
	 *            cannot be null or empty string
	 * @return cannot be null but can be empty string
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public String getOptionalAttributeValue(String key)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param key
	 *            cannot be null or empty string
	 * @param value
	 *            cannot be null but can be empty string
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden for key
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public void setOptionalAttributeValue(String key, String value)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
