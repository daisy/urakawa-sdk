package org.daisy.urakawa.mediaObject;

import org.daisy.urakawa.exceptions.MediaObjectTypeIsIllegal;
import org.daisy.urakawa.exceptions.MethodParameterIsNull;
import org.daisy.urakawa.exceptions.MethodParameterIsValueOutOfBounds;

/**
 * Convenience wrapper for a sequence of MediaObjects of the same type.
 * {@link MediaObject#isContinuous()} should return true or false depending on the type of MediaObject wrapped.
 * {@link MediaObject#getType()} should return the appropriate MediaObjectType, depending on the type of MediaObject wrapped.
 */
public interface SequenceMediaObject extends MediaObject {
    /**
     * @param index must be in bounds: [0..sequence.size-1]
     * @return the MediaObject item at a given index.
     */
    public MediaObject getItem(int index) throws MethodParameterIsValueOutOfBounds;

    /**
     * Sets the MediaObject at a given index. Replaces the existing MediaObject, and returns it. 
     *
     * @param index   must be in bounds: [0..sequence.size-1]
     * @param newItem cannot be null, and should be of the legal MediaObjectType for this sequence.
     * @return the replaced MediaObject, if any.
     */
    public MediaObject setItem(int index, MediaObject newItem) throws MethodParameterIsNull, MethodParameterIsValueOutOfBounds, MediaObjectTypeIsIllegal;

    /**
     * Appends a new MediaObject to the end of the sequence.
     * The very first MediaObject in the sequence has to be added via this method.
     * When it happens, the MediaObjectType of the sequence becomes the MediaObjectType of the inserted MediaObject.
     * From then on, {@link MediaObject#getType()} should return this particular MediaObjectType,
     * until the sequence is reset again (emptied then add the very first item again).
     *
     * @param newItem cannot be null, and should be of the legal MediaObjectType for this sequence.
     * If this is the first item to be inserted in this sequence, the MediaObjectType is just about to determined
     * and therefore the MediaObjectTypeIsIllegal exception is not raised.
     */
    public void appendItem(MediaObject newItem) throws MethodParameterIsNull, MediaObjectTypeIsIllegal;

    /**
     * Removes the MediaObject at a given index, and returns it.
     *
     * @param index must be in bounds: [0..sequence.size-1]
     * @return the removed MediaObject.
     */
    public MediaObject removeItem(int index) throws MethodParameterIsValueOutOfBounds;

    /**
     * @return the number of MediaObject items in the sequence (>=0)
     */
    public int getCount();
}