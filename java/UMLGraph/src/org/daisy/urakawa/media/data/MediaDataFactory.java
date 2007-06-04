package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create 1 MediaData
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface MediaDataFactory extends WithPresentation,
		WithMediaDataManager {
	/**
	 * 
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 */
	MediaData createMediaData(String xukLocalName, String xukNamespaceURI)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * 
	 * @param mediaType
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	MediaData createMediaData(Class<MediaData> mediaType)throws MethodParameterIsNullException;
}
