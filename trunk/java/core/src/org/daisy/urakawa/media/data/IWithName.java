package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * The name of the IMetadata
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
     * @return cannot be null but can be empty string
     */
    public String getName();

    /**
     * @param name cannot be null but can be empty string
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * 
     * @tagvalue Events "NameChanged"
     */
    public void setName(String name) throws MethodParameterIsNullException;
}
