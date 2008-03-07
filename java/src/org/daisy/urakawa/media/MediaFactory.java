package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the factory that creates generic
 * {@link org.daisy.urakawa.media.Media} instances.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.SequenceMedia
 * @depend - Create - org.daisy.urakawa.media.VideoMedia
 * @depend - Create - org.daisy.urakawa.media.AudioMedia
 * @depend - Create - org.daisy.urakawa.media.TextMedia
 * @depend - Create - org.daisy.urakawa.media.ImageMedia
 * @depend - Create - org.daisy.urakawa.media.Media
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface MediaFactory extends XukAble, WithPresentation {
	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.MediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	SequenceMedia createSequenceMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.MediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	VideoMedia createVideoMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.MediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	AudioMedia createAudioMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.MediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	TextMedia createTextMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.MediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	ImageMedia createImageMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * This factory method takes arguments to specify the exact type of object
	 * to create, given by the unique QName (XML Qualified Name) used in the XUK
	 * serialization format. This method can be used to generate instances of
	 * subclasses of the base object type.
	 * </p>
	 * 
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 */
	Media createMedia(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
