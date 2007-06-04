package org.daisy.urakawa.media;

import org.daisy.urakawa.WithPresentation;
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
	Media createMedia(MediaType type)throws MethodParameterIsNullException;

	Media createMedia(String xukLocalName, String xukNamespaceUri)throws MethodParameterIsNullException;
}
