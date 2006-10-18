package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.MethodParameterIsOutOfBoundsException;

/**
 * All the operations (aka "class methods") exposed here
 * have the same "return" value specification:
 * "return true if the operation is allowed in the current context, otherwise false."
 * When a user-agent of this API/Toolkit attempts to call a method "doXXX()" when
 * a corresponding "canDoXXX()" method returns false, then a "OperationNotValid" error should be raised.
 *
 * @see org.daisy.urakawa.exceptions.OperationNotValidException
 * @see SequenceMedia
 */
public interface SequenceMediaValidator {
    /**
     * @param index   must be in bounds: [0..sequence.size]
     * @param newItem cannot be null, and should be of the legal MediaType for this sequence (or any valid type if newItem is the first item to be inserted in the sequence: MediaTypeIsIllegalException exception is not raised).
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsOutOfBounds, MediaTypeIsIllegal"
     * @see SequenceMedia#insertItem(int,Media)
     */
    public boolean canInsertItem(int index, Media newItem) throws MethodParameterIsNullException, MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException;

    /**
     * @param index must be in bounds: [0..sequence.size-1]
     * @return the removed Media.
     * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
     * @see SequenceMedia#removeItem(int)
     */
    public boolean canRemoveItem(int index) throws MethodParameterIsOutOfBoundsException;
}
