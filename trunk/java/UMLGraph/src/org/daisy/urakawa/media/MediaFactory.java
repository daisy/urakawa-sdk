package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

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
 * @depend - Aggregation 1 MediaPresentation
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaFactory {
	Media createMedia(MediaType type) throws IsNotInitializedException;

	Media createMedia(String xukLocalName, String xukNamespaceUri)
			throws IsNotInitializedException;

	/**
	 * Gets the presentation
	 * 
	 * @return a non-null value
	 * @throws IsNotInitializedException
	 */
	MediaPresentation getPresentation() throws IsNotInitializedException;

	/**
	 * Initializes the presentation
	 * 
	 * @stereotype initialize
	 * @param presentation
	 * @throws IsAlreadyInitializedException
	 *             if this is not the first call to this method
	 * @throws MethodParameterIsNullException
	 *             if presentation is null
	 * @tagvalue Exceptions "MethodParameterIsNull, IsAlreadyInitialized"
	 */
	void setPresentation(MediaPresentation presentation)
			throws IsAlreadyInitializedException,
			MethodParameterIsNullException;
}
