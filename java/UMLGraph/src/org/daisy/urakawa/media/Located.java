package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 MediaLocation
 */
public interface Located {
    /**
     * @param location Cannot be null
     * @tagvalue Exceptions "MethodParameterIsNull"
     */
    public void setLocation(MediaLocation location) throws MethodParameterIsNullException;

    /**
     * @return the abstract location. Cannot be NULL.
     */
    public MediaLocation getLocation();
}