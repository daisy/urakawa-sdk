package org.daisy.urakawa.media;

/**
 * Factory for media objects
 * 
 * @depend - Create 1 Media
 * @depend - - - MediaType
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface MediaFactory extends WithMediaPresentation {
	Media createMedia(MediaType type);

	Media createMedia(String xukLocalName, String xukNamespaceUri);
}
