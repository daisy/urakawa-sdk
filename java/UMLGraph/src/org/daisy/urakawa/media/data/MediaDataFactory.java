package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create - org.daisy.urakawa.media.data.MediaData
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface MediaDataFactory extends WithPresentation {
	/**
	 * 
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
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
