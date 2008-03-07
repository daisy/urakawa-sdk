package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.AudioMediaData;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.media.data.MediaData} instances.
 * </p>
 * <p>
 * The returned object is managed by its associated manager.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.data.MediaData
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface MediaDataFactory extends XukAble, WithPresentation {
	/**
	 * Convenience method to obtain the manager from the Presentation instance.
	 * 
	 * @return manager
	 */
	public MediaDataManager getMediaDataManager();

	/**
	 * <p>
	 * Creates a new media data, managed.
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
	MediaData createMediaData(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a new media data, managed.
	 * </p>
	 * <p>
	 * This factory method takes a single argument to specify the exact type of
	 * object to create.
	 * </p>
	 * 
	 * @param mediaType
	 * @return can return null (in case the given argument does not match any
	 *         supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	MediaData createMediaData(Class<MediaData> mediaType)
			throws MethodParameterIsNullException;

	/**
	 * @return an instance of AudioMediaData
	 */
	public AudioMediaData createAudioMediaData();

	/**
	 * @return an instance of WavAudioMediaData
	 */
	public WavAudioMediaData createWavAudioMediaData();
}
