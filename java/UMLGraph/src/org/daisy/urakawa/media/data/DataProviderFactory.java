package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create 1 DataProvider
 * @todo verify / add comments and exceptions
 */
public interface DataProviderFactory extends WithDataProviderManager {
	DataProvider createDataProvider(String mimeType)throws MethodParameterIsNullException;

	DataProvider createDataProvider(String mimeType, String xukLocalName,
			String xukNamespaceURI)throws MethodParameterIsNullException;
}
