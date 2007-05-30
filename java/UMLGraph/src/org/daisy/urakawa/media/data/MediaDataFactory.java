package org.daisy.urakawa.media.data;

/**
 * @depend - Create 1 MediaData
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface MediaDataFactory extends WithMediaDataPresentation,
		WithMediaDataManager {
	MediaData createMediaData(String xukLocalName, String xukNamespaceURI);

	MediaData createMediaData(Class<MediaData> mediaType);
}
