package org.daisy.urakawa.media;

import java.util.List;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;

/**
 * This sequence only accepts Media of the same type, or nested sequences
 * containing media of the same type (recursively). The key method that
 * determines whether or not a Media is accepted is
 * {@link canAcceptMedia(Media)}. The default implementation always returns
 * true. This class can be extended and this method overridden. This actually
 * allows filter selection based on criteria more complex than just the type.
 * For example, the canAccept() method can test the contents of the Media
 * object. {@link Media#isContinuous()} should returns true if all the contained
 * Media objects are Continuous.
 * 
 * @depend - Composition 1..n org.daisy.urakawa.media.Media
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface SequenceMedia extends Media {
	/**
	 * @param media
	 * @return true if this sequence accepts the given Media object.
	 * @see org.daisy.urakawa.media.DoesNotAcceptMediaException
	 */
	public boolean canAcceptMedia(Media media);

	/**
	 * Inserts the Media at index = {@link SequenceMedia#getCount()}.
	 * 
	 * @param newItem
	 *            cannot be null, and should be a legal Media for this sequence
	 * @tagvalue Exceptions "MethodParameterIsNull-DoesNotAcceptMedia"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws DoesNotAcceptMediaException
	 *             if the given Media is not accepted for this sequence
	 */
	public void appendItem(Media newItem)
			throws MethodParameterIsNullException, DoesNotAcceptMediaException;

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
	 * item is appended at the end of the sequence.
	 * 
	 * @param index
	 *            must be in bounds: [0...{@link SequenceMedia#getCount()}]
	 * @param newItem
	 *            cannot be null, and should be a legal Media for this sequence
	 * @tagvalue Exceptions
	 *           "MethodParameterIsNull-MethodParameterIsOutOfBounds-DoesNotAcceptMedia"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsOutOfBoundsException
	 *             if index is not an authorized value
	 * @throws DoesNotAcceptMediaException
	 *             if the given Media is not accepted for this sequence
	 */
	public void insertItem(int index, Media newItem)
			throws MethodParameterIsNullException,
			MethodParameterIsOutOfBoundsException, DoesNotAcceptMediaException;

	/**
	 * Removes the Media at a given index, and returns it.
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
}