package org.daisy.urakawa.media.data;

/**
 * @depend - Create 1 FileDataProvider
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface FileDataProviderFactory extends DataProviderFactory,
		WithFileDataProviderManager {
	FileDataProvider createFileDataProvider(String mimeType);

	FileDataProvider createFileDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI);

	public String getExtensionFromMimeType(String mimeType);
}
