package org.daisy.urakawa.media.data;

/**
 * @depend - Create 1 FileDataProvider
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface FileDataProviderFactory extends DataProviderFactory,
		WithFileDataProviderManager {
	FileDataProvider createFileDataProvider(String mimeType);

	FileDataProvider createFileDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI);

	public String getExtensionFromMimeType(String mimeType);
}
