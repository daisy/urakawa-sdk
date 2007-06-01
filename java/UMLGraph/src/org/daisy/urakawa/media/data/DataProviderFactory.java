package org.daisy.urakawa.media.data;

/**
 * @depend - Create 1 DataProvider
 * @todo verify / add comments and exceptions
 */
public interface DataProviderFactory extends WithDataProviderManager {
	DataProvider createDataProvider(String mimeType);

	DataProvider createDataProvider(String mimeType, String xukLocalName,
			String xukNamespaceURI);
}
