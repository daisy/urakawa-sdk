package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaLocation
 */
public interface ExternalMedia extends Media {
    /**
     * @return the abstract location. Cannot be NULL.
     */
    public MediaLocation getLocation();

    /**
     * Sets the abstract location.
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     * @stereotype Initialize
     * @param location Cannot be null
     * @tagvalue Exceptions MethodParameterIsNull
     */
    public void setLocation(MediaLocation location) throws MethodParameterIsNullException;
}