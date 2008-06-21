package org.daisy.urakawa.media.data;

import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.audio.IAudioMediaData;
import org.daisy.urakawa.media.data.audio.codec.WavAudioMediaData;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.media.data.IMediaData} instances.
 * </p>
 * <p>
 * The returned object is managed by its associated manager.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.data.IMediaData
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 */
public interface IMediaDataFactory extends IXukAble, IWithPresentation {
	/**
	 * Convenience method to obtain the manager from the IPresentation instance.
	 * 
	 * @return manager
	 */
	public IMediaDataManager getMediaDataManager();

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
	IMediaData createMediaData(String xukLocalName, String xukNamespaceURI)
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
	IMediaData createMediaData(Class<IMediaData> mediaType)
			throws MethodParameterIsNullException;

	/**
	 * @return an instance of IAudioMediaData
	 */
	public IAudioMediaData createAudioMediaData();

	/**
	 * @return an instance of WavAudioMediaData
	 */
	public WavAudioMediaData createWavAudioMediaData();
}
