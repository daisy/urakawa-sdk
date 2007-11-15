package org.daisy.urakawa.media;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.XukAbleObjectFactory;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.ManagedAudioMedia;

/**
 * <p>
 * This is the factory that creates generic
 * {@link org.daisy.urakawa.media.Media} instances.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.Media
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface MediaFactory extends XukAbleObjectFactory, WithPresentation {
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
	 */
	SequenceMedia createSequenceMedia();

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
	 */
	ExternalVideoMedia createVideoMedia();

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
	 */
	ExternalAudioMedia createAudioMedia();

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
	 */
	TextMedia createTextMedia();

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
	 */
	ExternalTextMedia createExternalTextMedia();

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
	 */
	ExternalImageMedia createImageMedia();

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
	 */
	ManagedAudioMedia createManagedAudioMedia();

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
	Media createMedia(String xukLocalName, String xukNamespaceUri)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}
