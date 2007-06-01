package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;

/**
 * @depend - Create 1 MediaData
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface MediaDataFactory extends WithPresentation,
		WithMediaDataManager {
	MediaData createMediaData(String xukLocalName, String xukNamespaceURI);

	MediaData createMediaData(Class<MediaData> mediaType);
}
