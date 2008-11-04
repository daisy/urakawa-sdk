package org.daisy.urakawa.metadata;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * The optional attributes for the IMetadata
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithOptionalAttributes
{
    /**
     * @param key
     *        cannot be null or empty string
     * @return cannot be null but can be empty string
     * @throws MethodParameterIsNullException
     * @throws MethodParameterIsEmptyStringException
     */
    public String getOptionalAttributeValue(String key)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * @param key
     *        cannot be null or empty string
     * @param value
     *        cannot be null but can be empty string
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameter is forbidden for key
     * @tagvalue Events "Metadata"
     */
    public void setOptionalAttributeValue(String key, String value)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * Gets a list of non-empty strings of
     * 
     * @return a non-null list (but can be empty)
     */
    public List<String> getOptionalAttributeNames();
}
