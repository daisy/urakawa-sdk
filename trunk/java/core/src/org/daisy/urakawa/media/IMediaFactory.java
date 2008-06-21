package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is the factory that creates generic
 * {@link org.daisy.urakawa.media.IMedia} instances.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.ISequenceMedia
 * @depend - Create - org.daisy.urakawa.media.IVideoMedia
 * @depend - Create - org.daisy.urakawa.media.IAudioMedia
 * @depend - Create - org.daisy.urakawa.media.ITextMedia
 * @depend - Create - org.daisy.urakawa.media.IImageMedia
 * @depend - Create - org.daisy.urakawa.media.IMedia
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 */
public interface IMediaFactory extends IXukAble, IWithPresentation {
	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.IMediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	ISequenceMedia createSequenceMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.IMediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	IVideoMedia createVideoMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.IMediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	IAudioMedia createAudioMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.IMediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	ITextMedia createTextMedia() throws FactoryCannotCreateTypeException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * Users of the API can have more control over which type to create by using
	 * the other factory method:
	 * {@link org.daisy.urakawa.media.IMediaFactory#createMedia(String, String)}
	 * </p>
	 * 
	 * @return cannot return null.
	 * @throws FactoryCannotCreateTypeException
	 */
	IImageMedia createImageMedia() throws FactoryCannotCreateTypeException;

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
	IMedia createMedia(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
