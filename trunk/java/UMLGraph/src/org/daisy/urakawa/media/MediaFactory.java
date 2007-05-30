package org.daisy.urakawa.media;


/**
 * Factory for media objects
 * 
 * @depend - Create 1 Media
 * @depend - - - MediaType
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaFactory extends WithMediaPresentation {
	Media createMedia(MediaType type);

	Media createMedia(String xukLocalName, String xukNamespaceUri);
}
