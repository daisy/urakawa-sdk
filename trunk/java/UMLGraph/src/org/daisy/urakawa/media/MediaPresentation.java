package org.daisy.urakawa.media;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.core.TreeNodePresentation;

/**
 * @depend - Composition 1 MediaFactory
 * @todo verify / add comments and exceptions
 */
public interface MediaPresentation extends TreeNodePresentation, WithMediaFactory {
	URI getBaseUri();

	void setBaseUri(URI newBase);

	List<Media> getListOfUsedMedia();
}
