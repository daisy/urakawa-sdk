package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * This sequence only accepts IMedia of the same type, or nested sequences
 * containing media of the same type (recursively). The key method that
 * determines whether or not a IMedia is accepted is
 * {@link #canAcceptMedia(IMedia)}. The default implementation always returns
 * true. This class can be extended and this method overridden. This actually
 * allows filter selection based on criteria more complex than just the type.
 * For example, the canAccept() method can test the contents of the IMedia
 * object. {@link IMedia#isContinuous()} should returns true if all the
 * contained IMedia objects are IContinuous.
 * 
 * @depend - Composition 1..n org.daisy.urakawa.media.IMedia
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface ISequenceMedia extends IMedia
{
    /**
     * @return true if multiple media types are allowed for objects in this
     *         sequence
     */
    public boolean getAllowMultipleTypes();

    /**
     * @param newValue true if multiple media types are allowed for objects in
     *        this sequence
     * @throws SequenceHasMultipleTypesException when newValue is true and the
     *         sequence contains media objects of multiple types.
     */
    public void setAllowMultipleTypes(boolean newValue)
            throws SequenceHasMultipleTypesException;

    /**
     * @param iMedia
     * 
     * @return true if this sequence accepts the given IMedia object.
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @see org.daisy.urakawa.media.DoesNotAcceptMediaException
     */
    public boolean canAcceptMedia(IMedia iMedia)
            throws MethodParameterIsNullException;

    /**
     * Inserts the IMedia at index = {@link ISequenceMedia#getCount()}.
     * 
     * @param newItem cannot be null, and should be a legal IMedia for this
     *        sequence
     * 
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws DoesNotAcceptMediaException if the given IMedia is not accepted
     *         for this sequence
     */
    public void appendItem(IMedia newItem)
            throws MethodParameterIsNullException, DoesNotAcceptMediaException;

    /**
     * Gets a media item at a given index
     * 
     * @param index must be in bounds: [0...{@link ISequenceMedia#getCount()}-1]
     * @return the IMedia object
     * 
     * @throws MethodParameterIsOutOfBoundsException if index is not an allowed
     *         value
     */
    public IMedia getItem(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * Inserts the IMedia at a given index. Increments (+1) all indexes of items
     * on the right. If index == {@link ISequenceMedia#getCount()}, then the
     * item is appended at the end of the sequence.
     * 
     * @param index must be in bounds: [0...{@link ISequenceMedia#getCount()}]
     * @param newItem cannot be null, and should be a legal IMedia for this
     *        sequence
     * 
     *           "MethodParameterIsNull-MethodParameterIsOutOfBounds-DoesNotAcceptMedia"
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws MethodParameterIsOutOfBoundsException if index is not an
     *         authorized value
     * @throws DoesNotAcceptMediaException if the given IMedia is not accepted
     *         for this sequence
     */
    public void insertItem(int index, IMedia newItem)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException, DoesNotAcceptMediaException;

    /**
     * Removes the IMedia at a given index, and returns it.
     * 
     * @param index must be in bounds: [0...{@link ISequenceMedia#getCount()}-1]
     * @return the removed IMedia.
     * 
     * @throws MethodParameterIsOutOfBoundsException if index is not an allowed
     *         value
     */
    public IMedia removeItem(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * Removed the given media from the list, if it exists. Otherwise throws an
     * exception.
     * 
     * @param item
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * 
     * @throws MediaIsNotInSequenceException if the given media does not exist
     *         in the the sequence
     */
    public void removeItem(IMedia item) throws MethodParameterIsNullException,
            MediaIsNotInSequenceException;

    /**
     * Gets the sequence length
     * 
     * @return the number of IMedia items in the sequence (>=0)
     */
    public int getCount();

    /**
     * @return a new non-null but potentially empty list of IMedia items in this
     *         sequence.
     */
    List<IMedia> getListOfItems();
}