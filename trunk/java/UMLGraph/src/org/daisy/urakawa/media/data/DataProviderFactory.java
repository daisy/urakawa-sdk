package org.daisy.urakawa.media.data;
/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface DataProviderFactory {
	DataProviderManager getDataProviderManager();

	void setDataProviderManager(DataProviderManager mngr);

	DataProvider createDataProvider(String mimeType);

	DataProvider createDataProvider(String mimeType, String xukLocalName, String xukNamespaceURI);
}
