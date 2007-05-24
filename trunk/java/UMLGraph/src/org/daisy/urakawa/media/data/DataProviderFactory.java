package org.daisy.urakawa.media.data;

public interface DataProviderFactory {
	DataProviderManager getDataProviderManager();

	void setDataProviderManager(DataProviderManager mngr);

	DataProvider createDataProvider(String mimeType);

	DataProvider createDataProvider(String mimeType, String xukLocalName, String xukNamespaceURI);
}
