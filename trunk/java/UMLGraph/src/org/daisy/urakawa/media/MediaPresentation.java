package org.daisy.urakawa.media;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.core.CorePresentation;

/**
 * @depend - Composition 1 MediaFactory
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaPresentation extends CorePresentation, WithMediaFactory {
	URI getBaseUri();

	void setBaseUri(URI newBase);

	List<Media> getListOfUsedMedia();
}
