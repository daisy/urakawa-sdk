package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting a value.
 * </p>
 * <p>
 * When using this interface (e.g. by using "extend" or "implement"), the host
 * object type should explicitly declare the UML aggregation or composition
 * relationship, in order to clearly state the rules for object instance
 * ownership.
 * <p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithValue
{
    /**
     * The value.
     * 
     * @return Cannot return NULL and cannot return an empty string.
     */
    public String getValue();

    /**
     * The value.
     * 
     * @param newValue
     *        cannot be null, cannot be empty String
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @tagvalue Events "ValueChanged"
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public void setValue(String newValue)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;
}
