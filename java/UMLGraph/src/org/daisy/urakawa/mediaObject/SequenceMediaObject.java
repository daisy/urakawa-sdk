package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MethodParameterIsNull;
import org.daisy.urakawa.exceptions.MethodParameterIsValueOutOfBounds;

/**
 * 
 */
public class SequenceMediaObject implements MediaObject {
    /**
     * @param index must be in bounds: [0..sequence.size-1]
     * @return the MediaObject item at a given index.
     */
    public MediaObject getItem(int index) throws MethodParameterIsValueOutOfBounds {
        return null;
    }

    /**
     * Sets the MediaObject at a given index. Replaces the existing MediaObject, and returns it.
     *
     * @param index   must be in bounds: [0..sequence.size-1]
     * @param newItem cannot be null
     * @return zz
     */
    public MediaObject setItem(int index, MediaObject newItem) throws MethodParameterIsNull, MethodParameterIsValueOutOfBounds {
        return null;
    }

    /**
     * Appends a new MediaObject to the end of the sequence.
     *
     * @param newItem cannot be null
     */
    public void appendItem(MediaObject newItem) throws MethodParameterIsNull {
    }

    /**
     * Removes the MediaObject at a given index, and returns it.
     *
     * @param index must be in bounds: [0..sequence.size-1]
     * @return zz
     */
    public MediaObject removeItem(int index) throws MethodParameterIsValueOutOfBounds {
        return null;
    }

    /**
     * @return the number of MediaObject items in the sequence
     */
    public int getCount() {
        return 0;
    }
}