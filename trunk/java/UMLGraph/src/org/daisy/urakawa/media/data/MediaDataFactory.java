package org.daisy.urakawa.media.data;

/**
 * @depend - Create 1 MediaData
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface MediaDataFactory extends WithMediaDataPresentation,
		WithMediaDataManager {
	MediaData createMediaData(String xukLocalName, String xukNamespaceURI);

	MediaData createMediaData(Class<MediaData> mediaType);
}
