package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaLocation
 */
public interface ExternalMedia extends Media {
    /**
     * Sets the abstract location.
     * Should *only* be used at construction/initialization time (using the Factory).
     * (visibility is "public" because it's mandatory in Interfaces, but it would rather be "package"
     * so that only the Factory can call this method, not the end-user).
     *
     * @param location Cannot be null
     * @stereotype Initialize
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setLocation(MediaLocation location) throws MethodParameterIsNullException;

    /**
     * @return the abstract location. Cannot be NULL.
     */
    public MediaLocation getLocation();
}