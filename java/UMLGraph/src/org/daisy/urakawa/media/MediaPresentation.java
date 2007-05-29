package org.daisy.urakawa.media;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @depend - Composition 1 MediaFactory
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaPresentation extends CorePresentation {
	/**
	 * @return the media factory for this presentation. Cannot return null.
	 */
	public MediaFactory getMediaFactory();

	/**
	 * @param fact
	 *            the media factory for this presentation. Cannot be null.
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setMediaFactory(MediaFactory fact)
			throws MethodParameterIsNullException;

	URI getBaseUri();

	void setBaseUri(URI newBase);

	List<Media> getListOfUsedMedia();
}
