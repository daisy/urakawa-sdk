package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend 1 Aggregation 1 MediaLocation
 */
public interface ExternalMedia extends Media {
    /**
     * @return the abstract location. Cannot be NULL.
     */
    public MediaLocation getLocation();

    /**
     * Sets the abstract location.
     *
     * @param location Cannot be null
     * @tagvalue Exceptions MethodParameterIsNull
     */
    public void setLocation(MediaLocation location) throws MethodParameterIsNullException;
}