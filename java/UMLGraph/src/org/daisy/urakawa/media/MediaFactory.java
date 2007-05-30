package org.daisy.urakawa.media;


/**
 * Factory for media objects
 * 
 * @zdepend - Create 1 AudioMedia
 * @zdepend - Create 1 VideoMedia
 * @zdepend - Create 1 ImageMedia
 * @zdepend - Create 1 TextMedia
 * @zdepend - Create 1 SequenceMedia
 * @depend - Create 1 Media
 * @depend - - - MediaType
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaFactory extends WithMediaPresentation {
	Media createMedia(MediaType type);

	Media createMedia(String xukLocalName, String xukNamespaceUri);
}
