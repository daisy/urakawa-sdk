package org.daisy.urakawa.media.data;

/**
 * @depend - Create 1 DataProvider
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface DataProviderFactory extends WithDataProviderManager {
	DataProvider createDataProvider(String mimeType);

	DataProvider createDataProvider(String mimeType, String xukLocalName,
			String xukNamespaceURI);
}
