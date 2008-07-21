package org.daisy.urakawa.command;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting the short and long descriptions.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithShortLongDescription
{
    /**
     * <p>
     * Return a human-readable description of the command (short).
     * </p>
     * 
     * @return cannot be null, or empty string.
     */
    public String getShortDescription();

    /**
     * <p>
     * Return a human-readable description of the command (long).
     * </p>
     * 
     * @return cannot be null, but can return an empty string.
     */
    public String getLongDescription();

    /**
     * @param str cannot be null, cannot be empty string.
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsEmptyStringException Empty string '' method
     *         parameter is forbidden: <b>str</b>
     */
    public void setShortDescription(String str)
            throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * @param str cannot be null, but can be empty string.
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     */
    public void setLongDescription(String str)
            throws MethodParameterIsNullException;
}
