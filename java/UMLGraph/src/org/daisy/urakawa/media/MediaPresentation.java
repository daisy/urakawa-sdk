package org.daisy.urakawa.media;

import java.net.URI;
import java.util.List;

/**
 * @depend - Composition 1 MediaFactory
 * @todo verify / add comments and exceptions
 */
public interface MediaPresentation extends WithMediaFactory {
	URI getBaseUri();

	void setBaseUri(URI newBase);

	List<Media> getListOfUsedMedia();
}
