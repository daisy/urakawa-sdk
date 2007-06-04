package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * Metadata
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface Metadata extends XukAble, ValueEquatable<Metadata> {
	/**
	 * @return The name of the metadata entry (cannot be null or empty)
	 */
	public String getName();

	/**
	 * Sets the name of the metadata entry
	 * 
	 * @param name
	 *            cannot be null or empty string
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @return convenience method for
	 *         {@link #getOptionalAttributeValue(String name)} for name =
	 *         "content". Cannot be null but can be empty string if no there is
	 *         no value for "content".
	 */
	public String getContent();

	/**
	 * convenience method for
	 * {@link #setOptionalAttributeValue(String name, String content)} for name =
	 * "content".
	 * 
	 * @param content
	 *            Cannot be null but can be empty string if content need to be
	 *            reset.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setContent(String content)
			throws MethodParameterIsNullException;

	/**
	 * @param name
	 *            The name for which to get the value. Cannot be null or empty
	 *            string.
	 * @return The value for the given name. Cannot be null but can be empty
	 *         string if no value is set for the given name.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public String getOptionalAttributeValue(String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @param name
	 *            The name for which to set the value. Cannot be null or empty
	 *            string.
	 * @param content
	 *            The value for the given name. Cannot be null but can be empty
	 *            string if content need to be reset.
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>name</b>
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 */
	public void setOptionalAttributeValue(String name, String content)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * @return the list of all set attribute names (which are strings, non
	 *         empty, non-null). Cannot be null, but can be an empty list.
	 */
	public List<String> getListOfOptionalAttributeNames();
}
