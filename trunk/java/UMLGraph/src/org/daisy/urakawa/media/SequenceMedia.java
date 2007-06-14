package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * A sequence of Media of the same type. {@link Media#getMediaType()} should
 * return the appropriate type of media, or a special type if the sequence is
 * empty. {@link Media#isContinuous()} should return true or false depending on
 * the type of Media wrapped.
 * 
 * @depend - Composition 1..n org.daisy.urakawa.media.Media
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface SequenceMedia extends Media {
	/**
	 * Inserts the Media at index = {@link SequenceMedia#getCount()}. The very
	 * first Media added in the sequence determines the MediaType of the other
	 * medias to be added, and therefore of the sequence itself.
	 * {@link Media#getMediaType()} should return a particular type for when the
	 * sequence is empty.
	 * 
	 * @param newItem
	 *            cannot be null, and should be of the legal MediaType for this
	 *            sequence (or any valid type if newItem is the first item to be
	 *            inserted in the sequence) MediaTypeIsIllegalException
	 *            exception is not raised).
	 * @tagvalue Exceptions "MethodParameterIsNull-MediaTypeIsIllegal"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 *             
	 * @throws MediaTypeIsIllegalException
	 *             if newItem.getMediaType() is not valid for this sequence
	 */
	public void appendItem(Media newItem)
			throws MethodParameterIsNullException, MediaTypeIsIllegalException;

	/**
	 * Gets a media item at a given index
	 * 
	 * @param index
	 *            must be in bounds: [0...{@link SequenceMedia#getCount()}-1]
	 * @return the Media object of type given by the sequence
	 *         {@link Media#getMediaType()}
	 * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if index is not an allowed value
	 */
	public Media getItem(int index)
			throws MethodParameterIsOutOfBoundsException;

	/**
	 * Inserts the Media at a given index. Increments (+1) all indexes of items
	 * on the right. If index == {@link SequenceMedia#getCount()}, then the
	 * item is appended at the end of the sequence. The very first Media added
	 * in the sequence determines the MediaType of the other medias to be added,
	 * and therefore of the sequence itself. {@link Media#getMediaType()} should
	 * return a particular type for when the sequence is empty.
	 * 
	 * @param index
	 *            must be in bounds: [0...{@link SequenceMedia#getCount()}]
	 * @param newItem
	 *            cannot be null, and should be of the legal MediaType for this
	 *            sequence (or any valid type if newItem is the first item to be
	 *            inserted in the sequence) MediaTypeIsIllegalException
	 *            exception is not raised).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds-MediaTypeIsIllegal"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if index is not an authorized value
	 * @throws MediaTypeIsIllegalException
	 *             if newItem.getMediaType() is not valid for this sequence
	 */
	public void insertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException;

	/**
	 * Removes the Media at a given index, and returns it. When the last item of
	 * the sequence is removed, the {@link Media#getMediaType()} method should
	 * return the special empty sequence type.
	 * 
	 * @param index
	 *            must be in bounds: [0...{@link SequenceMedia#getCount()}-1]
	 * @return the removed Media.
	 * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if index is not an allowed value
	 */
	public Media removeItem(int index)
			throws MethodParameterIsOutOfBoundsException;

	/**
	 * Gets the sequence length
	 * 
	 * @return the number of Media items in the sequence (>=0)
	 */
	public int getCount();

	List<Media> getListOfItems();

	/**
	 * 
	 * @param index
	 * @param newItem
	 * @return
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 * @throws MediaTypeIsIllegalException
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsOutOfBounds-MediaTypeIsIllegal"
	 */
	public boolean canInsertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, MediaTypeIsIllegalException;

	/**
	 * 
	 * @param index
	 * @return
	 * @throws MethodParameterIsOutOfBoundsException
	 * @tagvalue Exceptions "MethodParameterIsOutOfBounds"
	 */
	public boolean canRemoveItem(int index)
			throws MethodParameterIsOutOfBoundsException;
}