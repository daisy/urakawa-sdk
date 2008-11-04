package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * Getting and Setting a name for a IChannel.
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface IWithName
{
    /**
     * The human-readable / display name
     * 
     * @param name
     *        cannot be null, cannot be empty String
     * @throws MethodParameterIsEmptyStringException
     *         Empty string '' method parameters are forbidden
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public void setName(String name) throws MethodParameterIsNullException,
            MethodParameterIsEmptyStringException;

    /**
     * The human-readable / display name
     * 
     * @return cannot return null or empty string, by contract.
     * @throws IsNotInitializedException
     */
    public String getName() throws IsNotInitializedException;
}
