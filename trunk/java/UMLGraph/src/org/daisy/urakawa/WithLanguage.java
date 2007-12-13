package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;

/**
 * <p>
 * Getting and Setting a language.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithLanguage {
	/**
	 * The language (e.g. "en-US")
	 * 
	 * @param lang
	 *            can be null, but cannot be empty String
	 * @tagvalue Exceptions "MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	public void setLanguage(String lang)
			throws MethodParameterIsEmptyStringException;

	/**
	 * The language (e.g. "en-US")
	 * 
	 * @return can return null but not empty string, by contract.
	 */
	public String getLanguage();
}
