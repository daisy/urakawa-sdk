package org.daisy.urakawa.media;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Factory for media objects
 * 
 * @depend - Create 1 Media
 * @depend - - - MediaType
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface MediaFactory extends WithPresentation {
	/**
	 * 
	 * @param type
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	Media createMedia(MediaType type)throws MethodParameterIsNullException;

	/**
	 * 
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 */
	Media createMedia(String xukLocalName, String xukNamespaceUri)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
