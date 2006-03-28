package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsValueOutOfBoundsException;

/**
 * Convenience wrapper for a sequence of Media of the same type.
 * {@link Media#isContinuous()} should return true or false depending on the type of Media wrapped.
 * {@link Media#getType()} should return the appropriate MediaType, depending on the type of Media wrapped.
 * 
 * @depend 1 Composition 1..n Media
 */
public interface SequenceMedia extends Media {
    /**
     * @param index must be in bounds: [0..sequence.size-1]
     * @return the Media item at a given index.
     * @tagvalue Exceptions MethodParameterIsValueOutOfBounds
     */
    public Media getItem(int index) throws MethodParameterIsValueOutOfBoundsException;

    /**
     * Sets the Media at a given index. Replaces the existing Media, and returns it. 
     *
     * @param index   must be in bounds: [0..sequence.size-1]
     * @param newItem cannot be null, and should be of the legal MediaType for this sequence.
     * @return the replaced Media, if any.
     * @tagvalue Exceptions MethodParameterIsNull,MethodParameterIsValueOutOfBounds,MediaTypeIsIllegal
     */
    public Media setItem(int index, Media newItem) throws MethodParameterIsNullException, MethodParameterIsValueOutOfBoundsException, MediaTypeIsIllegalException;

    /**
     * Appends a new Media to the end of the sequence.
     * The very first Media in the sequence has to be added via this method.
     * When it happens, the MediaType of the sequence becomes the MediaType of the inserted Media.
     * From then on, {@link Media#getType()} should return this particular MediaType,
     * until the sequence is reset again (emptied then add the very first item again).
     *
     * @param newItem cannot be null, and should be of the legal MediaType for this sequence.
     * If this is the first item to be inserted in this sequence, the MediaType is just about to determined
     * and therefore the MediaTypeIsIllegalException exception is not raised.
     * @tagvalue Exceptions MethodParameterIsNull,MediaTypeIsIllegal
     */
    public void appendItem(Media newItem) throws MethodParameterIsNullException, MediaTypeIsIllegalException;

    /**
     * Removes the Media at a given index, and returns it.
     *
     * @param index must be in bounds: [0..sequence.size-1]
     * @return the removed Media.
     * @tagvalue Exceptions MethodParameterIsValueOutOfBounds
     */
    public Media removeItem(int index) throws MethodParameterIsValueOutOfBoundsException;

    /**
     * @return the number of Media items in the sequence (>=0)
     */
    public int getCount();
}